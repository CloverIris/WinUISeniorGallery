using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace WinUI3.Senior.Windowing;

public enum GameBarWidgetInteractionMode
{
    Interactive,
    ClickThrough
}

public enum GameBarWidgetState
{
    Closed,
    Opening,
    Interactive,
    ClickThrough,
    Minimized,
    Closing,
    Failed
}

public sealed class GameBarWidgetInteractionRequestedEventArgs(GameBarWidgetInteractionMode mode, string recoveryHotKey) : EventArgs
{
    public GameBarWidgetInteractionMode Mode { get; } = mode;
    public string RecoveryHotKey { get; } = recoveryHotKey;
    public bool Handled { get; set; }
}

/// <summary>
/// Host-neutral Game Bar style widget coordinator. It requests floating presentation and
/// interaction-mode changes but never changes hit testing or creates a window itself.
/// </summary>
public sealed class GameBarWidgetExperience : ContentControl, IDisposable
{
    private readonly SemaphoreSlim _gate = new(1, 1);
    private IFloatingWidgetHost? _host;
    private FloatingWidgetRequest? _request;
    private long _generation;
    private bool _disposed;

    public static readonly DependencyProperty WidgetIdProperty = DependencyProperty.Register(nameof(WidgetId), typeof(string), typeof(GameBarWidgetExperience), new PropertyMetadata("game-bar-widget"));
    public static readonly DependencyProperty PreferredSizeProperty = DependencyProperty.Register(nameof(PreferredSize), typeof(Size), typeof(GameBarWidgetExperience), new PropertyMetadata(new Size(420, 260)));
    public static readonly DependencyProperty RecoveryHotKeyProperty = DependencyProperty.Register(nameof(RecoveryHotKey), typeof(string), typeof(GameBarWidgetExperience), new PropertyMetadata("Ctrl+Shift+G"));
    public static readonly DependencyProperty IsAlwaysOnTopProperty = DependencyProperty.Register(nameof(IsAlwaysOnTop), typeof(bool), typeof(GameBarWidgetExperience), new PropertyMetadata(true));

    public GameBarWidgetExperience() { }

    public string WidgetId { get => (string)GetValue(WidgetIdProperty); set => SetValue(WidgetIdProperty, value); }
    public Size PreferredSize { get => (Size)GetValue(PreferredSizeProperty); set => SetValue(PreferredSizeProperty, NormalizeSize(value)); }
    public string RecoveryHotKey { get => (string)GetValue(RecoveryHotKeyProperty); set => SetValue(RecoveryHotKeyProperty, string.IsNullOrWhiteSpace(value) ? "Ctrl+Shift+G" : value); }
    public bool IsAlwaysOnTop { get => (bool)GetValue(IsAlwaysOnTopProperty); set => SetValue(IsAlwaysOnTopProperty, value); }
    public GameBarWidgetState State { get; private set; } = GameBarWidgetState.Closed;
    public GameBarWidgetInteractionMode InteractionMode { get; private set; } = GameBarWidgetInteractionMode.Interactive;
    public IFloatingWidgetHost? Host => _host;
    public event EventHandler? StateChanged;
    public event EventHandler<GameBarWidgetInteractionRequestedEventArgs>? InteractionModeRequested;
    public event EventHandler<FloatingWidgetOperationResult>? OperationCompleted;

    public void AttachHost(IFloatingWidgetHost host)
    {
        ArgumentNullException.ThrowIfNull(host);
        DetachHost();
        _host = host;
        host.OwnerClosed += OnOwnerClosed;
    }

    public void DetachHost()
    {
        if (_host is not null) _host.OwnerClosed -= OnOwnerClosed;
        _host = null;
        Interlocked.Increment(ref _generation);
        _request = null;
        SetState(GameBarWidgetState.Closed);
    }

    public async Task<FloatingWidgetOperationResult> OpenAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) return FloatingWidgetOperationResult.Cancelled();
        if (_host is null) return FloatingWidgetOperationResult.Rejected("host-required");
        if (string.IsNullOrWhiteSpace(WidgetId)) return FloatingWidgetOperationResult.Rejected("widget-id-required");
        if (!IsValidSize(PreferredSize)) return FloatingWidgetOperationResult.Rejected("invalid-size");
        try { await _gate.WaitAsync(cancellationToken).ConfigureAwait(true); }
        catch (OperationCanceledException) { return FloatingWidgetOperationResult.Cancelled(); }
        try
        {
            var host = _host;
            if (host is null) return FloatingWidgetOperationResult.Rejected("host-required");
            var generation = _generation;
            _request = new FloatingWidgetRequest(WidgetId, PreferredSize, null, IsAlwaysOnTop, FloatingWidgetOwnerClosePolicy.Close);
            SetState(GameBarWidgetState.Opening);
            FloatingWidgetOperationResult result;
            try { result = await host.OpenAsync(_request, Content, cancellationToken).ConfigureAwait(true); }
            catch (OperationCanceledException) { result = FloatingWidgetOperationResult.Cancelled(); }
            catch (Exception ex) { result = FloatingWidgetOperationResult.Failed("host-exception", ex.Message); }
            if (generation != _generation || !ReferenceEquals(host, _host)) return FloatingWidgetOperationResult.Cancelled();
            SetState(result.IsSuccess ? GameBarWidgetState.Interactive : GameBarWidgetState.Failed);
            OperationCompleted?.Invoke(this, result);
            return result;
        }
        finally { _gate.Release(); }
    }

    public async Task<FloatingWidgetOperationResult> CloseAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) return FloatingWidgetOperationResult.Cancelled();
        if (_host is null || _request is null || State == GameBarWidgetState.Closed) return FloatingWidgetOperationResult.Rejected("not-open");
        try { await _gate.WaitAsync(cancellationToken).ConfigureAwait(true); }
        catch (OperationCanceledException) { return FloatingWidgetOperationResult.Cancelled(); }
        try
        {
            var host = _host;
            var generation = _generation;
            SetState(GameBarWidgetState.Closing);
            var result = await host.CloseAsync(_request.WidgetId, cancellationToken).ConfigureAwait(true);
            if (generation != _generation || !ReferenceEquals(host, _host)) return FloatingWidgetOperationResult.Cancelled();
            if (result.IsSuccess) { _request = null; SetState(GameBarWidgetState.Closed); } else SetState(GameBarWidgetState.Failed);
            OperationCompleted?.Invoke(this, result);
            return result;
        }
        catch (OperationCanceledException)
        {
            if (!_disposed && State == GameBarWidgetState.Closing)
            {
                SetState(InteractionMode == GameBarWidgetInteractionMode.ClickThrough ? GameBarWidgetState.ClickThrough : GameBarWidgetState.Interactive);
            }
            return FloatingWidgetOperationResult.Cancelled();
        }
        catch (Exception ex) { SetState(GameBarWidgetState.Failed); return FloatingWidgetOperationResult.Failed("host-exception", ex.Message); }
        finally { _gate.Release(); }
    }

    public bool SetInteractionMode(GameBarWidgetInteractionMode mode)
    {
        if (State is GameBarWidgetState.Closed or GameBarWidgetState.Opening or GameBarWidgetState.Closing) return false;
        if (mode == GameBarWidgetInteractionMode.ClickThrough && string.IsNullOrWhiteSpace(RecoveryHotKey)) return false;
        if (InteractionMode == mode) return true;
        var args = new GameBarWidgetInteractionRequestedEventArgs(mode, RecoveryHotKey);
        InteractionModeRequested?.Invoke(this, args);
        if (mode == GameBarWidgetInteractionMode.ClickThrough && !args.Handled) return false;
        InteractionMode = mode;
        SetState(mode == GameBarWidgetInteractionMode.ClickThrough ? GameBarWidgetState.ClickThrough : GameBarWidgetState.Interactive);
        return true;
    }

    public void Minimize() { if (State is GameBarWidgetState.Interactive or GameBarWidgetState.ClickThrough) SetState(GameBarWidgetState.Minimized); }
    public void Restore() { if (State == GameBarWidgetState.Minimized) SetState(InteractionMode == GameBarWidgetInteractionMode.ClickThrough ? GameBarWidgetState.ClickThrough : GameBarWidgetState.Interactive); }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        DetachHost();
        // Keep the semaphore alive until in-flight continuations finish. Disposing it here
        // could make a late continuation throw while releasing the gate.
        GC.SuppressFinalize(this);
    }

    private void OnOwnerClosed(object? sender, EventArgs e) { _request = null; SetState(GameBarWidgetState.Closed); }
    private void SetState(GameBarWidgetState state) { if (State == state) return; State = state; StateChanged?.Invoke(this, EventArgs.Empty); }
    private static Size NormalizeSize(Size size) => new(double.IsFinite(size.Width) ? Math.Clamp(size.Width, 160, 4096) : 420, double.IsFinite(size.Height) ? Math.Clamp(size.Height, 90, 4096) : 260);
    private static bool IsValidSize(Size size) => double.IsFinite(size.Width) && double.IsFinite(size.Height) && size.Width > 0 && size.Height > 0;
}
