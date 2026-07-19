using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace WinUI3.Senior.Windowing;

/// <summary>
/// Coordinates inline playback content with an application-owned floating host.
/// It exposes detach/attach requests and never creates or migrates a Window itself.
/// </summary>
public sealed class DetachablePlayerHost : ContentControl, IDisposable
{
    private readonly SemaphoreSlim _operationGate = new(1, 1);
    private IFloatingWidgetHost? _floatingHost;
    private CancellationTokenSource? _lifetimeCancellation;
    private FloatingWidgetRequest? _activeRequest;
    private long _operationRevision;
    private bool _disposed;

    public static readonly DependencyProperty PlayerIdProperty = DependencyProperty.Register(
        nameof(PlayerId), typeof(string), typeof(DetachablePlayerHost), new PropertyMetadata("player"));
    public static readonly DependencyProperty PreferredSizeProperty = DependencyProperty.Register(
        nameof(PreferredSize), typeof(Size), typeof(DetachablePlayerHost), new PropertyMetadata(new Size(480, 270)));
    public static readonly DependencyProperty OwnerClosePolicyProperty = DependencyProperty.Register(
        nameof(OwnerClosePolicy), typeof(FloatingWidgetOwnerClosePolicy), typeof(DetachablePlayerHost), new PropertyMetadata(FloatingWidgetOwnerClosePolicy.Close));
    public static readonly DependencyProperty IsAlwaysOnTopProperty = DependencyProperty.Register(
        nameof(IsAlwaysOnTop), typeof(bool), typeof(DetachablePlayerHost), new PropertyMetadata(true));

    public DetachablePlayerHost()
    {
        Loaded += (_, _) => IsHostVisible = true;
        Unloaded += (_, _) => IsHostVisible = false;
    }

    public string PlayerId { get => (string)GetValue(PlayerIdProperty); set => SetValue(PlayerIdProperty, value); }
    public Size PreferredSize { get => (Size)GetValue(PreferredSizeProperty); set => SetValue(PreferredSizeProperty, NormalizeSize(value)); }
    public FloatingWidgetOwnerClosePolicy OwnerClosePolicy { get => (FloatingWidgetOwnerClosePolicy)GetValue(OwnerClosePolicyProperty); set => SetValue(OwnerClosePolicyProperty, value); }
    public bool IsAlwaysOnTop { get => (bool)GetValue(IsAlwaysOnTopProperty); set => SetValue(IsAlwaysOnTopProperty, value); }
    public bool IsHostVisible { get; private set; } = true;
    public DetachablePlayerState State { get; private set; } = DetachablePlayerState.Inline;
    public IFloatingWidgetHost? FloatingHost => _floatingHost;
    public FloatingWidgetRequest? ActiveRequest => _activeRequest;
    public string? LastErrorCode { get; private set; }

    public event EventHandler? StateChanged;
    public event EventHandler<DetachablePlayerOperationResult>? OperationCompleted;
    public event EventHandler? HostClosed;

    public void AttachHost(IFloatingWidgetHost host)
    {
        ArgumentNullException.ThrowIfNull(host);
        if (ReferenceEquals(_floatingHost, host)) return;
        DetachHost();
        _floatingHost = host;
        host.OwnerClosed += OnOwnerClosed;
    }

    public void DetachHost()
    {
        if (_floatingHost is not null) _floatingHost.OwnerClosed -= OnOwnerClosed;
        _floatingHost = null;
        Interlocked.Increment(ref _operationRevision);
        _activeRequest = null;
        if (State != DetachablePlayerState.Inline) SetState(DetachablePlayerState.Inline);
    }

    public Task<DetachablePlayerOperationResult> DetachAsync(CancellationToken cancellationToken = default) =>
        ExecuteAsync(DetachablePlayerState.Detaching, static (host, request, content, token) => host.OpenAsync(request, content, token),
            result =>
            {
                if (result.IsSuccess) SetState(DetachablePlayerState.Detached);
                else SetState(DetachablePlayerState.Failed);
            }, cancellationToken);

    public Task<DetachablePlayerOperationResult> AttachAsync(CancellationToken cancellationToken = default) =>
        ExecuteAsync(DetachablePlayerState.Attaching, static (host, request, _, token) => host.CloseAsync(request.WidgetId, token),
            result =>
            {
                if (result.IsSuccess) { _activeRequest = null; SetState(DetachablePlayerState.Inline); }
                else SetState(DetachablePlayerState.Failed);
            }, cancellationToken);

    public async Task<DetachablePlayerOperationResult> ToggleAsync(CancellationToken cancellationToken = default)
    {
        return State == DetachablePlayerState.Detached
            ? await AttachAsync(cancellationToken)
            : await DetachAsync(cancellationToken);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _lifetimeCancellation?.Cancel();
        _lifetimeCancellation?.Dispose();
        _lifetimeCancellation = null;
        DetachHost();
        GC.SuppressFinalize(this);
    }

    private async Task<DetachablePlayerOperationResult> ExecuteAsync(
        DetachablePlayerState transientState,
        Func<IFloatingWidgetHost, FloatingWidgetRequest, object?, CancellationToken, Task<FloatingWidgetOperationResult>> operation,
        Action<DetachablePlayerOperationResult> apply,
        CancellationToken cancellationToken)
    {
        if (_disposed) return DetachablePlayerOperationResult.Cancelled("disposed");
        if (_floatingHost is null) return DetachablePlayerOperationResult.Rejected("host-required");
        if (string.IsNullOrWhiteSpace(PlayerId)) return DetachablePlayerOperationResult.Rejected("player-id-required");
        if (!IsValidSize(PreferredSize)) return DetachablePlayerOperationResult.Rejected("invalid-size");
        if (transientState == DetachablePlayerState.Detaching && State == DetachablePlayerState.Detached)
            return DetachablePlayerOperationResult.Rejected("already-detached");
        if (transientState == DetachablePlayerState.Attaching && State != DetachablePlayerState.Detached)
            return DetachablePlayerOperationResult.Rejected("not-detached");

        try
        {
            await _operationGate.WaitAsync(cancellationToken).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return DetachablePlayerOperationResult.Cancelled("cancelled");
        }
        try
        {
            var host = _floatingHost;
            if (host is null) return DetachablePlayerOperationResult.Rejected("host-required");
            var revision = Interlocked.Increment(ref _operationRevision);
            var request = _activeRequest ?? new FloatingWidgetRequest(PlayerId, PreferredSize, null, IsAlwaysOnTop, OwnerClosePolicy);
            var priorState = State;
            if (transientState == DetachablePlayerState.Detaching) _activeRequest = request;
            SetState(transientState);
            FloatingWidgetOperationResult hostResult;
            try { hostResult = await operation(host, request, Content, cancellationToken).ConfigureAwait(true); }
            catch (OperationCanceledException) { hostResult = FloatingWidgetOperationResult.Cancelled(); }
            catch (Exception ex) { hostResult = FloatingWidgetOperationResult.Failed("host-exception", ex.Message); }

            if (_disposed || revision != Volatile.Read(ref _operationRevision) || !ReferenceEquals(host, _floatingHost))
                return DetachablePlayerOperationResult.Cancelled("stale-operation");

            var result = hostResult.IsSuccess
                ? DetachablePlayerOperationResult.Succeeded(transientState == DetachablePlayerState.Detaching ? DetachablePlayerState.Detached : DetachablePlayerState.Inline)
                : hostResult.Status == WindowingOperationStatus.Cancelled
                    ? DetachablePlayerOperationResult.Cancelled(hostResult.ErrorCode)
                    : hostResult.Status == WindowingOperationStatus.Rejected
                        ? DetachablePlayerOperationResult.Rejected(hostResult.ErrorCode ?? "host-rejected", hostResult.Message)
                        : DetachablePlayerOperationResult.Failed(hostResult.ErrorCode ?? "host-failed", hostResult.Message);
            LastErrorCode = result.IsSuccess ? null : result.ErrorCode;
            if (result.Status == WindowingOperationStatus.Cancelled)
            {
                SetState(priorState);
            }
            else
            {
                apply(result);
            }
            OperationCompleted?.Invoke(this, result);
            return result;
        }
        finally { _operationGate.Release(); }
    }

    private void OnOwnerClosed(object? sender, EventArgs e)
    {
        Interlocked.Increment(ref _operationRevision);
        _activeRequest = null;
        SetState(DetachablePlayerState.Inline);
        HostClosed?.Invoke(this, EventArgs.Empty);
    }

    private void SetState(DetachablePlayerState state)
    {
        if (State == state) return;
        State = state;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    private static Size NormalizeSize(Size size) => new(
        double.IsFinite(size.Width) ? Math.Clamp(size.Width, 160, 4096) : 480,
        double.IsFinite(size.Height) ? Math.Clamp(size.Height, 90, 4096) : 270);

    private static bool IsValidSize(Size size) => double.IsFinite(size.Width) && double.IsFinite(size.Height) && size.Width > 0 && size.Height > 0;
}

public enum DetachablePlayerState
{
    Inline,
    Detaching,
    Detached,
    Attaching,
    Failed
}

public sealed record DetachablePlayerOperationResult(
    WindowingOperationStatus Status,
    DetachablePlayerState State,
    string? ErrorCode = null,
    string? Message = null)
{
    public bool IsSuccess => Status == WindowingOperationStatus.Success;
    public static DetachablePlayerOperationResult Succeeded(DetachablePlayerState state) => new(WindowingOperationStatus.Success, state);
    public static DetachablePlayerOperationResult Rejected(string code, string? message = null) => new(WindowingOperationStatus.Rejected, DetachablePlayerState.Failed, code, message);
    public static DetachablePlayerOperationResult Cancelled(string? code = null) => new(WindowingOperationStatus.Cancelled, DetachablePlayerState.Inline, code ?? "cancelled");
    public static DetachablePlayerOperationResult Failed(string code, string? message = null) => new(WindowingOperationStatus.Failed, DetachablePlayerState.Failed, code, message);
}
