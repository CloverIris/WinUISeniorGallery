using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3.Senior.Windowing;

/// <summary>
/// State-aware chrome coordinator. It owns no Window; an application supplies an
/// <see cref="IWindowChromeHost"/> adapter and remains responsible for HWND/AppWindow lifetime.
/// </summary>
public sealed class WindowChrome : ContentControl
{
    public static readonly DependencyProperty BackdropProperty = DependencyProperty.Register(
        nameof(Backdrop), typeof(WindowBackdropKind), typeof(WindowChrome), new PropertyMetadata(WindowBackdropKind.None, OnConfigurationChanged));

    public static readonly DependencyProperty ExtendIntoTitleBarProperty = DependencyProperty.Register(
        nameof(ExtendIntoTitleBar), typeof(bool), typeof(WindowChrome), new PropertyMetadata(false, OnConfigurationChanged));

    public static readonly DependencyProperty IsDragRegionEnabledProperty = DependencyProperty.Register(
        nameof(IsDragRegionEnabled), typeof(bool), typeof(WindowChrome), new PropertyMetadata(true, OnConfigurationChanged));

    private IWindowChromeHost? _host;
    private int _operation;

    public WindowChrome()
    {
        Unloaded += (_, _) => _ = DetachAsync();
    }

    public WindowBackdropKind Backdrop
    {
        get => (WindowBackdropKind)GetValue(BackdropProperty);
        set => SetValue(BackdropProperty, value);
    }

    public bool ExtendIntoTitleBar
    {
        get => (bool)GetValue(ExtendIntoTitleBarProperty);
        set => SetValue(ExtendIntoTitleBarProperty, value);
    }

    public bool IsDragRegionEnabled
    {
        get => (bool)GetValue(IsDragRegionEnabledProperty);
        set => SetValue(IsDragRegionEnabledProperty, value);
    }

    public WindowChromeState State { get; private set; } = WindowChromeState.Detached;
    public IWindowChromeHost? Host => _host;
    public string? LastErrorCode { get; private set; }

    public event EventHandler? StateChanged;
    public event EventHandler<WindowingOperationResult>? OperationCompleted;

    public async Task<WindowingOperationResult> AttachAsync(IWindowChromeHost host, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(host);
        if (State is WindowChromeState.Active or WindowChromeState.Attaching)
        {
            return WindowingOperationResult.Rejected("already-attached", "WindowChrome is already attached to a host.");
        }

        var operation = Interlocked.Increment(ref _operation);
        SetState(WindowChromeState.Attaching);
        _host = host;
        host.Closed += OnHostClosed;

        try
        {
            var result = await host.ConfigureChromeAsync(CreateConfiguration(), cancellationToken).ConfigureAwait(true);
            if (operation != _operation)
            {
                return WindowingOperationResult.Cancelled();
            }

            if (result.IsSuccess)
            {
                LastErrorCode = null;
                SetState(WindowChromeState.Active);
            }
            else
            {
                LastErrorCode = result.ErrorCode;
                SetState(WindowChromeState.Failed);
                host.Closed -= OnHostClosed;
                _host = null;
            }

            OperationCompleted?.Invoke(this, result);
            return result;
        }
        catch (OperationCanceledException)
        {
            SetState(WindowChromeState.Detached);
            host.Closed -= OnHostClosed;
            _host = null;
            var result = WindowingOperationResult.Cancelled();
            OperationCompleted?.Invoke(this, result);
            return result;
        }
        catch (Exception ex)
        {
            LastErrorCode = "host-exception";
            SetState(WindowChromeState.Failed);
            host.Closed -= OnHostClosed;
            _host = null;
            var result = WindowingOperationResult.Failed("host-exception", ex.Message);
            OperationCompleted?.Invoke(this, result);
            return result;
        }
    }

    public async Task<WindowingOperationResult> ReconfigureAsync(CancellationToken cancellationToken = default)
    {
        if (_host is null || State != WindowChromeState.Active)
        {
            return WindowingOperationResult.Rejected("not-attached");
        }

        var operation = Interlocked.Increment(ref _operation);
        SetState(WindowChromeState.Reconfiguring);
        try
        {
            var result = await _host.ConfigureChromeAsync(CreateConfiguration(), cancellationToken).ConfigureAwait(true);
            if (operation != _operation)
            {
                return WindowingOperationResult.Cancelled();
            }
            if (result.IsSuccess)
            {
                LastErrorCode = null;
                SetState(WindowChromeState.Active);
            }
            else
            {
                LastErrorCode = result.ErrorCode;
                SetState(WindowChromeState.Failed);
            }
            OperationCompleted?.Invoke(this, result);
            return result;
        }
        catch (OperationCanceledException)
        {
            SetState(WindowChromeState.Active);
            var result = WindowingOperationResult.Cancelled();
            OperationCompleted?.Invoke(this, result);
            return result;
        }
        catch (Exception ex)
        {
            LastErrorCode = "host-exception";
            SetState(WindowChromeState.Failed);
            var result = WindowingOperationResult.Failed("host-exception", ex.Message);
            OperationCompleted?.Invoke(this, result);
            return result;
        }
    }

    public Task<WindowingOperationResult> DetachAsync()
    {
        Interlocked.Increment(ref _operation);
        if (_host is not null)
        {
            _host.Closed -= OnHostClosed;
            _host = null;
        }

        if (State is not WindowChromeState.Detached and not WindowChromeState.Closed)
        {
            SetState(WindowChromeState.Detached);
        }
        LastErrorCode = null;
        return Task.FromResult(WindowingOperationResult.Succeeded());
    }

    private WindowChromeConfiguration CreateConfiguration() => new(Backdrop, ExtendIntoTitleBar, IsDragRegionEnabled);

    private static void OnConfigurationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is WindowChrome chrome && chrome.State == WindowChromeState.Active)
        {
            _ = chrome.ReconfigureAsync();
        }
    }

    private void OnHostClosed(object? sender, EventArgs args)
    {
        Interlocked.Increment(ref _operation);
        _host = null;
        SetState(WindowChromeState.Closed);
    }

    private void SetState(WindowChromeState state)
    {
        if (State == state)
        {
            return;
        }
        State = state;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
