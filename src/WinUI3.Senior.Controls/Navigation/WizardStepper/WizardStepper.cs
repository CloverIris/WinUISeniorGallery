using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WinUI3.Senior.Controls;

public enum WizardStepperOrientation { Horizontal, Vertical }
public enum WizardNavigationMode { Linear, Free }
public enum WizardStepState { Upcoming, Current, Completed, Invalid, Skipped }

public sealed class WizardStep
{
    public WizardStep(string id, string title, object? content = null)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("A step id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("A step title is required.", nameof(title));
        Id = id; Title = title; Content = content;
    }
    public string Id { get; }
    public string Title { get; }
    public string? Description { get; set; }
    public object? Content { get; set; }
    public bool IsOptional { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsCompleted { get; internal set; }
    public WizardStepState State { get; internal set; } = WizardStepState.Upcoming;
}

public readonly record struct WizardValidationResult(bool IsValid, string? ErrorMessage = null)
{
    public static WizardValidationResult Valid => new(true);
    public static WizardValidationResult Invalid(string message) => new(false, message);
}

public sealed class WizardStepChangingEventArgs(WizardStep current, WizardStep next, bool isForward) : EventArgs
{
    public WizardStep Current { get; } = current;
    public WizardStep Next { get; } = next;
    public bool IsForward { get; } = isForward;
    public bool Cancel { get; set; }
}

public sealed class WizardStepChangedEventArgs(WizardStep previous, WizardStep current) : EventArgs
{
    public WizardStep Previous { get; } = previous;
    public WizardStep Current { get; } = current;
}

public sealed class WizardValidationFailedEventArgs(WizardStep step, string message) : EventArgs
{
    public WizardStep Step { get; } = step;
    public string Message { get; } = message;
    public bool Handled { get; set; }
}

public sealed class WizardCompletedEventArgs(IReadOnlyList<WizardStep> steps) : EventArgs
{
    public IReadOnlyList<WizardStep> Steps { get; } = steps;
}

/// <summary>
/// A validation-aware multi-step flow. The control owns transition ordering and cancellation;
/// page content and validation remain host-owned.
/// </summary>
public sealed class WizardStepper : Control
{
    private readonly ObservableCollection<WizardStep> _steps = new();
    private readonly SemaphoreSlim _transitionGate = new(1, 1);
    private CancellationTokenSource? _validationCts;
    private ItemsControl? _stepRepeater;
    private ContentPresenter? _contentPresenter;
    private bool _internalSelection;
    private bool _completionRaised;

    public WizardStepper()
    {
        DefaultStyleKey = typeof(WizardStepper);
        _steps.CollectionChanged += OnStepsChanged;
        NextCommand = new DelegateCommand(_ => _ = NextAsync());
        BackCommand = new DelegateCommand(_ => _ = BackAsync());
        CancelCommand = new DelegateCommand(_ => Cancelled?.Invoke(this, EventArgs.Empty));
        KeyDown += OnKeyDown;
        IsTabStop = true;
    }

    public static readonly DependencyProperty CurrentIndexProperty = DependencyProperty.Register(
        nameof(CurrentIndex), typeof(int), typeof(WizardStepper), new PropertyMetadata(-1, OnCurrentIndexChanged));
    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation), typeof(WizardStepperOrientation), typeof(WizardStepper), new PropertyMetadata(WizardStepperOrientation.Horizontal, OnVisualPropertyChanged));
    public static readonly DependencyProperty NavigationModeProperty = DependencyProperty.Register(
        nameof(NavigationMode), typeof(WizardNavigationMode), typeof(WizardStepper), new PropertyMetadata(WizardNavigationMode.Linear));
    public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
        nameof(IsBusy), typeof(bool), typeof(WizardStepper), new PropertyMetadata(false, OnVisualPropertyChanged));
    public static readonly DependencyProperty CurrentStepProperty = DependencyProperty.Register(
        nameof(CurrentStep), typeof(WizardStep), typeof(WizardStepper), new PropertyMetadata(null));
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header), typeof(object), typeof(WizardStepper), new PropertyMetadata(null));

    public WizardStepperOrientation Orientation { get => (WizardStepperOrientation)GetValue(OrientationProperty); set => SetValue(OrientationProperty, value); }
    public WizardNavigationMode NavigationMode { get => (WizardNavigationMode)GetValue(NavigationModeProperty); set => SetValue(NavigationModeProperty, value); }
    public int CurrentIndex { get => (int)GetValue(CurrentIndexProperty); private set => SetValue(CurrentIndexProperty, value); }
    public bool IsBusy { get => (bool)GetValue(IsBusyProperty); private set => SetValue(IsBusyProperty, value); }
    public WizardStep? CurrentStep { get => (WizardStep?)GetValue(CurrentStepProperty); private set => SetValue(CurrentStepProperty, value); }
    public object? Header { get => GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
    public IReadOnlyList<WizardStep> Steps => _steps;
    public ICommand NextCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand CancelCommand { get; }
    public Func<WizardStep, CancellationToken, Task<WizardValidationResult>>? ValidateStep { get; set; }

    public event EventHandler<WizardStepChangingEventArgs>? StepChanging;
    public event EventHandler<WizardStepChangedEventArgs>? StepChanged;
    public event EventHandler<WizardValidationFailedEventArgs>? ValidationFailed;
    public event EventHandler<WizardCompletedEventArgs>? Completed;
    public event EventHandler? Cancelled;

    protected override AutomationPeer OnCreateAutomationPeer() => new WizardStepperAutomationPeer(this);

    public void SetSteps(IEnumerable<WizardStep> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);
        var replacement = steps.ToArray();
        if (replacement.GroupBy(step => step.Id, StringComparer.Ordinal).Any(group => group.Count() != 1)) throw new ArgumentException("Step ids must be unique.", nameof(steps));
        _steps.Clear();
        foreach (var step in replacement) _steps.Add(step);
        RepairCurrentStep();
    }

    public async Task<bool> NextAsync(CancellationToken cancellationToken = default)
    {
        if (_steps.Count == 0 || CurrentIndex < 0) return false;
        if (CurrentIndex == _steps.Count - 1)
        {
            await _transitionGate.WaitAsync(cancellationToken).ConfigureAwait(true);
            try
            {
                if (IsBusy || _completionRaised) return _completionRaised;
                var result = await ValidateCurrentAsync(cancellationToken).ConfigureAwait(true);
                if (!result.IsValid) return false;
                _completionRaised = true;
                Completed?.Invoke(this, new WizardCompletedEventArgs(_steps.ToArray()));
                return true;
            }
            finally { _transitionGate.Release(); }
        }
        return await MoveToAsync(CurrentIndex + 1, isForward: true, cancellationToken).ConfigureAwait(true);
    }

    public Task<bool> BackAsync(CancellationToken cancellationToken = default)
    {
        if (_steps.Count == 0 || CurrentIndex <= 0) return Task.FromResult(false);
        return MoveToAsync(CurrentIndex - 1, isForward: false, cancellationToken);
    }

    public Task<bool> GoToAsync(int index, CancellationToken cancellationToken = default)
    {
        if (index < 0 || index >= _steps.Count || !_steps[index].IsEnabled) return Task.FromResult(false);
        if (NavigationMode == WizardNavigationMode.Linear && index > CurrentIndex + 1) return Task.FromResult(false);
        return MoveToAsync(index, index > CurrentIndex, cancellationToken);
    }

    public void Reset()
    {
        _validationCts?.Cancel();
        _completionRaised = false;
        foreach (var step in _steps) { step.IsCompleted = false; step.State = WizardStepState.Upcoming; }
        CurrentIndex = _steps.Count == 0 ? -1 : 0;
        RepairCurrentStep();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _stepRepeater = GetTemplateChild("PART_StepRepeater") as ItemsControl;
        _contentPresenter = GetTemplateChild("PART_ContentPresenter") as ContentPresenter;
        if (_stepRepeater is null) throw new InvalidOperationException("WizardStepper template must provide PART_StepRepeater.");
        _stepRepeater.ItemsSource = _steps;
        UpdateVisualState();
    }

    private async Task<bool> MoveToAsync(int targetIndex, bool isForward, CancellationToken cancellationToken)
    {
        if (targetIndex < 0 || targetIndex >= _steps.Count || targetIndex == CurrentIndex || !_steps[targetIndex].IsEnabled) return false;
        await _transitionGate.WaitAsync(cancellationToken).ConfigureAwait(true);
        try
        {
            if (IsBusy) return false;
            var current = CurrentStep;
            var target = _steps[targetIndex];
            if (current is not null && isForward && !await ValidateCurrentAsync(cancellationToken).ConfigureAwait(true)) return false;
            if (current is not null)
            {
                var changing = new WizardStepChangingEventArgs(current, target, isForward);
                StepChanging?.Invoke(this, changing);
                if (changing.Cancel) return false;
            }
            var previous = current ?? target;
            _internalSelection = true;
            try { CurrentIndex = targetIndex; } finally { _internalSelection = false; }
            if (targetIndex != _steps.Count - 1)
                _completionRaised = false;
            if (current is not null) current.State = current.IsCompleted ? WizardStepState.Completed : WizardStepState.Upcoming;
            target.State = WizardStepState.Current;
            CurrentStep = target;
            _contentPresenter?.SetValue(ContentPresenter.ContentProperty, target.Content);
            StepChanged?.Invoke(this, new WizardStepChangedEventArgs(previous, target));
            UpdateVisualState();
            return true;
        }
        finally { _transitionGate.Release(); }
    }

    private async Task<bool> ValidateCurrentAsync(CancellationToken cancellationToken)
    {
        var current = CurrentStep;
        if (current is null || ValidateStep is null || current.IsOptional) { if (current is not null) current.IsCompleted = true; return true; }
        _validationCts?.Cancel();
        _validationCts?.Dispose();
        _validationCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        IsBusy = true;
        current.State = WizardStepState.Current;
        try
        {
            var result = await ValidateStep(current, _validationCts.Token).ConfigureAwait(true);
            if (result.IsValid)
            {
                current.IsCompleted = true;
                current.State = WizardStepState.Completed;
                return true;
            }
            current.State = WizardStepState.Invalid;
            var args = new WizardValidationFailedEventArgs(current, result.ErrorMessage ?? "This step is invalid.");
            ValidationFailed?.Invoke(this, args);
            AutomationProperties.SetHelpText(this, args.Message);
            return false;
        }
        catch (OperationCanceledException) when (_validationCts?.IsCancellationRequested == true) { return false; }
        finally { IsBusy = false; UpdateVisualState(); }
    }

    private static void OnCurrentIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (WizardStepper)d;
        if (!owner._internalSelection) owner.RepairCurrentStep();
    }
    private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((WizardStepper)d).UpdateVisualState();
    private void OnStepsChanged(object? sender, NotifyCollectionChangedEventArgs e) => RepairCurrentStep();
    private void RepairCurrentStep()
    {
        if (_steps.Count == 0) { CurrentStep = null; CurrentIndex = -1; return; }
        var index = Math.Clamp(CurrentIndex, 0, _steps.Count - 1);
        if (CurrentIndex != index) { _internalSelection = true; try { CurrentIndex = index; } finally { _internalSelection = false; } }
        CurrentStep = _steps[index];
        for (var i = 0; i < _steps.Count; i++) _steps[i].State = i == index ? WizardStepState.Current : (_steps[i].IsCompleted ? WizardStepState.Completed : WizardStepState.Upcoming);
        _contentPresenter?.SetValue(ContentPresenter.ContentProperty, CurrentStep.Content);
        UpdateVisualState();
    }
    private void OnKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (IsBusy) return;
        var forward = FlowDirection == FlowDirection.RightToLeft ? Windows.System.VirtualKey.Left : Windows.System.VirtualKey.Right;
        var backward = FlowDirection == FlowDirection.RightToLeft ? Windows.System.VirtualKey.Right : Windows.System.VirtualKey.Left;
        if (e.Key == forward) { _ = NextAsync(); e.Handled = true; }
        else if (e.Key == backward) { _ = BackAsync(); e.Handled = true; }
        else if (e.Key == Windows.System.VirtualKey.Enter) { _ = NextAsync(); e.Handled = true; }
        else if (e.Key == Windows.System.VirtualKey.Escape) { Cancelled?.Invoke(this, EventArgs.Empty); e.Handled = true; }
    }
    private void UpdateVisualState()
    {
        VisualStateManager.GoToState(this, Orientation.ToString(), true);
        VisualStateManager.GoToState(this, IsBusy ? "Validating" : "Idle", true);
        AutomationProperties.SetLiveSetting(this, AutomationLiveSetting.Polite);
    }

    private sealed class DelegateCommand(Action<object?> execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => execute(parameter);
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

internal sealed class WizardStepperAutomationPeer(WizardStepper owner) : FrameworkElementAutomationPeer(owner)
{
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.List;
    protected override string GetClassNameCore() => nameof(WizardStepper);
    protected override string GetNameCore() => AutomationProperties.GetName(Owner) is { Length: > 0 } name ? name : "Wizard steps";
    protected override string GetHelpTextCore() => Owner is WizardStepper wizard && wizard.CurrentStep is { } step ? $"Step {wizard.CurrentIndex + 1} of {wizard.Steps.Count}: {step.Title}" : base.GetHelpTextCore();
    protected override bool IsKeyboardFocusableCore() => Owner is WizardStepper wizard && wizard.IsEnabled && wizard.IsTabStop;
}
