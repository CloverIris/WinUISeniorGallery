using System;
using Microsoft.UI.Xaml;

namespace WinUI3.Senior.Windowing;

/// <summary>Host-neutral connected-transition coordinator. Templates decide how to render the key.</summary>
public sealed class ConnectedTransitionController
{
    private long _sequence;

    public string? TransitionKey { get; private set; }
    public bool IsRunning => State == ConnectedTransitionState.Running;
    public ConnectedTransitionState State { get; private set; } = ConnectedTransitionState.Idle;
    public TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(220);
    public bool IsReducedMotion { get; set; }
    public double Progress { get; private set; }
    public long Sequence => _sequence;

    public event EventHandler? Started;
    public event EventHandler? Completed;
    public event EventHandler? Cancelled;
    public event EventHandler<ConnectedTransitionEventArgs>? ProgressChanged;

    public void Begin(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("A stable transition key is required.", nameof(key));
        }
        if (IsRunning)
        {
            Cancel();
        }
        TransitionKey = key;
        Progress = 0;
        _sequence++;
        State = ConnectedTransitionState.Running;
        Started?.Invoke(this, EventArgs.Empty);
        if (IsReducedMotion || Duration <= TimeSpan.Zero)
        {
            Complete();
        }
    }

    public void Advance(double progress)
    {
        if (!IsRunning || TransitionKey is null)
        {
            return;
        }
        var normalized = Math.Clamp(progress, 0, 1);
        if (Math.Abs(normalized - Progress) < double.Epsilon)
        {
            return;
        }
        Progress = normalized;
        ProgressChanged?.Invoke(this, new ConnectedTransitionEventArgs(TransitionKey, Sequence, Progress));
        if (Progress >= 1)
        {
            Complete();
        }
    }

    public void Complete()
    {
        if (!IsRunning)
        {
            return;
        }
        Progress = 1;
        State = ConnectedTransitionState.Completed;
        if (TransitionKey is not null)
        {
            ProgressChanged?.Invoke(this, new ConnectedTransitionEventArgs(TransitionKey, Sequence, Progress));
        }
        Completed?.Invoke(this, EventArgs.Empty);
    }

    public void Cancel()
    {
        if (!IsRunning)
        {
            return;
        }
        State = ConnectedTransitionState.Cancelled;
        Cancelled?.Invoke(this, EventArgs.Empty);
    }

    public void Reset()
    {
        State = ConnectedTransitionState.Idle;
        TransitionKey = null;
        Progress = 0;
    }
}

public static class ConnectedTransitionBehavior
{
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled", typeof(bool), typeof(ConnectedTransitionBehavior), new PropertyMetadata(false, OnEnabledChanged));

    public static readonly DependencyProperty TransitionKeyProperty = DependencyProperty.RegisterAttached(
        "TransitionKey", typeof(string), typeof(ConnectedTransitionBehavior), new PropertyMetadata(null));

    public static readonly DependencyProperty DurationProperty = DependencyProperty.RegisterAttached(
        "Duration", typeof(TimeSpan), typeof(ConnectedTransitionBehavior), new PropertyMetadata(TimeSpan.FromMilliseconds(220), OnControllerSettingChanged));

    public static readonly DependencyProperty IsReducedMotionProperty = DependencyProperty.RegisterAttached(
        "IsReducedMotion", typeof(bool), typeof(ConnectedTransitionBehavior), new PropertyMetadata(false, OnControllerSettingChanged));

    private static readonly DependencyProperty ControllerProperty = DependencyProperty.RegisterAttached(
        "Controller", typeof(ConnectedTransitionController), typeof(ConnectedTransitionBehavior), new PropertyMetadata(null));

    public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);
    public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);
    public static void SetTransitionKey(DependencyObject obj, string? value) => obj.SetValue(TransitionKeyProperty, value);
    public static string? GetTransitionKey(DependencyObject obj) => (string?)obj.GetValue(TransitionKeyProperty);
    public static void SetDuration(DependencyObject obj, TimeSpan value) => obj.SetValue(DurationProperty, value);
    public static TimeSpan GetDuration(DependencyObject obj) => (TimeSpan)obj.GetValue(DurationProperty);
    public static void SetIsReducedMotion(DependencyObject obj, bool value) => obj.SetValue(IsReducedMotionProperty, value);
    public static bool GetIsReducedMotion(DependencyObject obj) => (bool)obj.GetValue(IsReducedMotionProperty);

    public static ConnectedTransitionController? GetController(DependencyObject obj)
        => obj.ReadLocalValue(ControllerProperty) as ConnectedTransitionController;

    /// <summary>Starts a best-effort visual-state transition and returns whether it was accepted.</summary>
    public static bool Begin(FrameworkElement element, string? stateName = "Connected")
    {
        ArgumentNullException.ThrowIfNull(element);
        var key = GetTransitionKey(element);
        if (!GetIsEnabled(element) || string.IsNullOrWhiteSpace(key))
        {
            return false;
        }
        var controller = GetController(element);
        if (controller is null)
        {
            controller = new ConnectedTransitionController();
            element.SetValue(ControllerProperty, controller);
        }
        controller.Duration = GetDuration(element);
        controller.IsReducedMotion = GetIsReducedMotion(element);
        var accepted = VisualStateManager.GoToState(element, stateName ?? "Connected", !controller.IsReducedMotion);
        if (accepted || controller.IsReducedMotion)
        {
            controller.Begin(key!);
            return true;
        }
        return false;
    }

    public static void Complete(DependencyObject element) => GetController(element)?.Complete();
    public static void Cancel(DependencyObject element) => GetController(element)?.Cancel();

    private static void OnControllerSettingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not FrameworkElement element || element.ReadLocalValue(ControllerProperty) is not ConnectedTransitionController controller)
        {
            return;
        }
        controller.Duration = GetDuration(element);
        controller.IsReducedMotion = GetIsReducedMotion(element);
    }

    private static void OnEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not FrameworkElement element || (bool)args.NewValue)
        {
            return;
        }
        if (element.ReadLocalValue(ControllerProperty) is ConnectedTransitionController controller)
        {
            controller.Cancel();
            controller.Reset();
            element.ClearValue(ControllerProperty);
        }
    }
}
