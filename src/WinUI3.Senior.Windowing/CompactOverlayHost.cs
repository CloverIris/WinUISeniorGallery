using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace WinUI3.Senior.Windowing;

/// <summary>Inline/overlay presenter switcher backed by an application-owned window adapter.</summary>
public sealed class CompactOverlayHost : ContentControl
{
    public static readonly DependencyProperty PreferredOverlayWidthProperty = DependencyProperty.Register(
        nameof(PreferredOverlayWidth), typeof(double), typeof(CompactOverlayHost), new PropertyMetadata(480d));

    public static readonly DependencyProperty PreferredOverlayHeightProperty = DependencyProperty.Register(
        nameof(PreferredOverlayHeight), typeof(double), typeof(CompactOverlayHost), new PropertyMetadata(270d));

    private readonly SemaphoreSlim _gate = new(1, 1);
    private ICompactOverlayHost? _host;
    private int _generation;

    public double PreferredOverlayWidth
    {
        get => (double)GetValue(PreferredOverlayWidthProperty);
        set => SetValue(PreferredOverlayWidthProperty, Math.Clamp(value, 160, 4096));
    }

    public double PreferredOverlayHeight
    {
        get => (double)GetValue(PreferredOverlayHeightProperty);
        set => SetValue(PreferredOverlayHeightProperty, Math.Clamp(value, 90, 4096));
    }

    public CompactOverlayMode ConfirmedMode { get; private set; } = CompactOverlayMode.Inline;
    public CompactOverlayMode RequestedMode { get; private set; } = CompactOverlayMode.Inline;
    public ICompactOverlayHost? Host => _host;
    public string? LastErrorCode { get; private set; }
    public bool IsTransitioning => RequestedMode is CompactOverlayMode.EnteringOverlay or CompactOverlayMode.ExitingOverlay;

    public event EventHandler? ModeChanged;
    public event EventHandler<CompactOverlayOperationResult>? TransitionCompleted;

    public void AttachHost(ICompactOverlayHost host)
    {
        ArgumentNullException.ThrowIfNull(host);
        DetachHost();
        _host = host;
        LastErrorCode = null;
    }

    public void DetachHost()
    {
        Interlocked.Increment(ref _generation);
        _host = null;
        RequestedMode = ConfirmedMode;
    }

    public Task<CompactOverlayOperationResult> EnterOverlayAsync(CancellationToken cancellationToken = default) => RequestModeAsync(CompactOverlayMode.Overlay, cancellationToken);
    public Task<CompactOverlayOperationResult> ExitOverlayAsync(CancellationToken cancellationToken = default) => RequestModeAsync(CompactOverlayMode.Inline, cancellationToken);

    public async Task<CompactOverlayOperationResult> RequestModeAsync(CompactOverlayMode mode, CancellationToken cancellationToken = default)
    {
        if (mode is not CompactOverlayMode.Inline and not CompactOverlayMode.Overlay)
        {
            return CompactOverlayOperationResult.Rejected(ConfirmedMode, "invalid-mode");
        }
        if (_host is null)
        {
            return CompactOverlayOperationResult.Rejected(ConfirmedMode, "host-required");
        }
        if (mode == ConfirmedMode)
        {
            return CompactOverlayOperationResult.Succeeded(ConfirmedMode);
        }
        if (!double.IsFinite(PreferredOverlayWidth) || !double.IsFinite(PreferredOverlayHeight) || PreferredOverlayWidth <= 0 || PreferredOverlayHeight <= 0)
        {
            return CompactOverlayOperationResult.Rejected(ConfirmedMode, "invalid-size");
        }

        try
        {
            await _gate.WaitAsync(cancellationToken).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            return CompactOverlayOperationResult.Cancelled(ConfirmedMode);
        }
        try
        {
            var generation = _generation;
            var host = _host;
            if (host is null)
            {
                return CompactOverlayOperationResult.Rejected(ConfirmedMode, "host-required");
            }
            if (mode == ConfirmedMode)
            {
                RequestedMode = ConfirmedMode;
                return CompactOverlayOperationResult.Succeeded(ConfirmedMode);
            }
            RequestedMode = mode;
            var transient = mode == CompactOverlayMode.Overlay ? CompactOverlayMode.EnteringOverlay : CompactOverlayMode.ExitingOverlay;
            RequestedMode = transient;
            var prior = ConfirmedMode;
            var result = await host.RequestModeAsync(new CompactOverlayRequest(mode, new Size(PreferredOverlayWidth, PreferredOverlayHeight)), cancellationToken).ConfigureAwait(true);
            if (generation != _generation || !ReferenceEquals(host, _host))
            {
                RequestedMode = ConfirmedMode;
                var stale = CompactOverlayOperationResult.Cancelled(ConfirmedMode);
                TransitionCompleted?.Invoke(this, stale);
                return stale;
            }
            if (result.IsSuccess && result.ConfirmedMode == mode)
            {
                ConfirmedMode = mode;
                RequestedMode = mode;
                ModeChanged?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                ConfirmedMode = prior;
                RequestedMode = prior;
                LastErrorCode = result.ErrorCode;
            }
            _ = transient; // transient value documents the state transition for host presenters.
            TransitionCompleted?.Invoke(this, result);
            return result;
        }
        catch (OperationCanceledException)
        {
            RequestedMode = ConfirmedMode;
            var result = CompactOverlayOperationResult.Cancelled(ConfirmedMode);
            TransitionCompleted?.Invoke(this, result);
            return result;
        }
        catch (Exception ex)
        {
            RequestedMode = ConfirmedMode;
            LastErrorCode = "host-exception";
            var result = CompactOverlayOperationResult.Failed(ConfirmedMode, "host-exception", ex.Message);
            TransitionCompleted?.Invoke(this, result);
            return result;
        }
        finally
        {
            _gate.Release();
        }
    }
}
