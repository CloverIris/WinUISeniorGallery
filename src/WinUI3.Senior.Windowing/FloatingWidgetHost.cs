using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3.Senior.Windowing;

/// <summary>
/// Coordinates a host-created floating widget. It never creates an AppWindow itself.
/// </summary>
public sealed class FloatingWidgetHost : ContentControl
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IFloatingWidgetHost? _host;
    private FloatingWidgetRequest? _request;
    private int _generation;

    public FloatingWidgetState State { get; private set; } = FloatingWidgetState.Closed;
    public string? WidgetId => _request?.WidgetId;
    public FloatingWidgetRequest? Request => _request;
    public IFloatingWidgetHost? Host => _host;
    public string? LastErrorCode { get; private set; }

    public event EventHandler? StateChanged;
    public event EventHandler<FloatingWidgetOperationResult>? OperationCompleted;

    public void AttachHost(IFloatingWidgetHost host)
    {
        ArgumentNullException.ThrowIfNull(host);
        if (ReferenceEquals(_host, host))
        {
            return;
        }
        DetachHost();
        _host = host;
        _host.OwnerClosed += OnOwnerClosed;
    }

    public void DetachHost()
    {
        if (_host is not null)
        {
            _host.OwnerClosed -= OnOwnerClosed;
            _host = null;
        }
        Interlocked.Increment(ref _generation);
        _request = null;
        if (State is not FloatingWidgetState.Closed)
        {
            SetState(FloatingWidgetState.Closed);
        }
    }

    public async Task<FloatingWidgetOperationResult> OpenAsync(FloatingWidgetRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.WidgetId))
        {
            return FloatingWidgetOperationResult.Rejected("widget-id-required");
        }
        if (!IsValidSize(request.PreferredSize))
        {
            return FloatingWidgetOperationResult.Rejected("invalid-size", "Preferred widget size must be finite and positive.");
        }
        if (_host is null)
        {
            return FloatingWidgetOperationResult.Rejected("host-required", "Attach an application-owned widget host first.");
        }

        try
        {
            await _gate.WaitAsync(cancellationToken).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return FloatingWidgetOperationResult.Cancelled();
        }
        try
        {
            if (State is FloatingWidgetState.Visible or FloatingWidgetState.Creating)
            {
                return FloatingWidgetOperationResult.Rejected("already-open");
            }
            var generation = _generation;
            var host = _host;
            if (host is null)
            {
                return FloatingWidgetOperationResult.Rejected("host-required");
            }
            _request = request;
            SetState(FloatingWidgetState.Creating);
            FloatingWidgetOperationResult result;
            try
            {
                result = await host.OpenAsync(request, Content, cancellationToken).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                result = FloatingWidgetOperationResult.Cancelled();
            }
            catch (Exception ex)
            {
                result = FloatingWidgetOperationResult.Failed("host-exception", ex.Message);
            }
            if (generation != _generation || !ReferenceEquals(host, _host))
            {
                return FloatingWidgetOperationResult.Cancelled();
            }
            if (result.Status == WindowingOperationStatus.Cancelled)
            {
                _request = null;
                SetState(FloatingWidgetState.Closed);
            }
            else if (result.IsSuccess)
            {
                LastErrorCode = null;
                SetState(FloatingWidgetState.Visible);
            }
            else
            {
                LastErrorCode = result.ErrorCode;
                SetState(FloatingWidgetState.Failed);
            }
            OperationCompleted?.Invoke(this, result);
            return result;
        }
        finally
        {
            _gate.Release();
        }
    }

    public Task<FloatingWidgetOperationResult> CloseAsync(CancellationToken cancellationToken = default) => ExecuteHostOperationAsync(
        FloatingWidgetState.Closing,
        static (host, id, token) => host.CloseAsync(id, token),
        result =>
        {
            if (result.IsSuccess)
            {
                SetState(FloatingWidgetState.Closed);
            }
            else
            {
                LastErrorCode = result.ErrorCode;
                SetState(FloatingWidgetState.Failed);
            }
        }, cancellationToken);

    public Task<FloatingWidgetOperationResult> RestoreAsync(CancellationToken cancellationToken = default) => ExecuteHostOperationAsync(
        FloatingWidgetState.Creating,
        static (host, id, token) => host.RestoreAsync(id, token),
        result =>
        {
            if (!result.IsSuccess) LastErrorCode = result.ErrorCode;
            SetState(result.IsSuccess ? FloatingWidgetState.Visible : FloatingWidgetState.Failed);
        }, cancellationToken);

    public Task<FloatingWidgetOperationResult> MinimizeAsync(CancellationToken cancellationToken = default) => ExecuteHostOperationAsync(
        FloatingWidgetState.Closing,
        static (host, id, token) => host.MinimizeAsync(id, token),
        result =>
        {
            if (!result.IsSuccess) LastErrorCode = result.ErrorCode;
            SetState(result.IsSuccess ? FloatingWidgetState.Minimized : FloatingWidgetState.Failed);
        }, cancellationToken);

    private async Task<FloatingWidgetOperationResult> ExecuteHostOperationAsync(
        FloatingWidgetState transientState,
        Func<IFloatingWidgetHost, string, CancellationToken, Task<FloatingWidgetOperationResult>> operation,
        Action<FloatingWidgetOperationResult> apply,
        CancellationToken cancellationToken)
    {
        if (_host is null || _request is null || State == FloatingWidgetState.Closed)
        {
            return FloatingWidgetOperationResult.Rejected("not-open");
        }
        try
        {
            await _gate.WaitAsync(cancellationToken).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return FloatingWidgetOperationResult.Cancelled();
        }
        try
        {
            var generation = _generation;
            var host = _host;
            var request = _request;
            if (host is null || request is null)
            {
                return FloatingWidgetOperationResult.Rejected("not-open");
            }
            var priorState = State;
            SetState(transientState);
            FloatingWidgetOperationResult result;
            try
            {
                result = await operation(host, request.WidgetId, cancellationToken).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                result = FloatingWidgetOperationResult.Cancelled();
            }
            catch (Exception ex)
            {
                result = FloatingWidgetOperationResult.Failed("host-exception", ex.Message);
            }
            if (generation != _generation || !ReferenceEquals(host, _host))
            {
                return FloatingWidgetOperationResult.Cancelled();
            }
            if (result.Status == WindowingOperationStatus.Cancelled)
            {
                SetState(priorState);
            }
            else
            {
                if (result.IsSuccess) LastErrorCode = null;
                apply(result);
            }
            OperationCompleted?.Invoke(this, result);
            return result;
        }
        finally
        {
            _gate.Release();
        }
    }

    private void OnOwnerClosed(object? sender, EventArgs args)
    {
        Interlocked.Increment(ref _generation);
        _request = null;
        SetState(FloatingWidgetState.Closed);
    }

    private static bool IsValidSize(Windows.Foundation.Size size)
        => double.IsFinite(size.Width) && double.IsFinite(size.Height) && size.Width > 0 && size.Height > 0;

    private void SetState(FloatingWidgetState state)
    {
        if (State == state)
        {
            return;
        }
        State = state;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
