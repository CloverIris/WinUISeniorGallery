using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WinUI3.Senior.Controls;

/// <summary>
/// CommandBar with an explicit compact/expanded state and host-visible overflow semantics.
/// It does not own navigation or execute commands outside the CommandBar contract.
/// </summary>
public sealed class ExpandableCommandBar : CommandBar
{
    private bool _autoCollapsed;
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded), typeof(bool), typeof(ExpandableCommandBar), new PropertyMetadata(true, OnExpandedChanged));
    public static readonly DependencyProperty IsExpansionToggleEnabledProperty = DependencyProperty.Register(
        nameof(IsExpansionToggleEnabled), typeof(bool), typeof(ExpandableCommandBar), new PropertyMetadata(true));
    public static readonly DependencyProperty CollapseThresholdProperty = DependencyProperty.Register(
        nameof(CollapseThreshold), typeof(double), typeof(ExpandableCommandBar), new PropertyMetadata(560d));
    public static readonly DependencyProperty IsContextualProperty = DependencyProperty.Register(
        nameof(IsContextual), typeof(bool), typeof(ExpandableCommandBar), new PropertyMetadata(false, OnContextualChanged));

    public ExpandableCommandBar()
    {
        DefaultStyleKey = typeof(ExpandableCommandBar);
        SizeChanged += OnSizeChanged;
        KeyDown += OnKeyDown;
    }

    public bool IsExpanded { get => (bool)GetValue(IsExpandedProperty); set => SetValue(IsExpandedProperty, value); }
    public bool IsExpansionToggleEnabled { get => (bool)GetValue(IsExpansionToggleEnabledProperty); set => SetValue(IsExpansionToggleEnabledProperty, value); }
    public double CollapseThreshold { get => (double)GetValue(CollapseThresholdProperty); set => SetValue(CollapseThresholdProperty, value); }
    public bool IsContextual { get => (bool)GetValue(IsContextualProperty); set => SetValue(IsContextualProperty, value); }
    public event EventHandler? ExpansionChanged;
    public event EventHandler? OverflowRequested;

    public void ToggleExpansion()
    {
        if (IsExpansionToggleEnabled)
        {
            _autoCollapsed = false;
            IsExpanded = !IsExpanded;
        }
    }

    public void Expand() { _autoCollapsed = false; IsExpanded = true; }
    public void Collapse() { _autoCollapsed = false; IsExpanded = false; }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        UpdateVisualState();
    }

    private static void OnExpandedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs _)
    {
        var bar = (ExpandableCommandBar)sender;
        bar.UpdateVisualState();
        bar.ExpansionChanged?.Invoke(bar, EventArgs.Empty);
    }

    private static void OnContextualChanged(DependencyObject sender, DependencyPropertyChangedEventArgs _)
    {
        ((ExpandableCommandBar)sender).UpdateVisualState();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs args)
    {
        if (IsExpansionToggleEnabled && args.NewSize.Width > 0 && args.NewSize.Width < CollapseThreshold)
        {
            if (IsExpanded)
            {
                _autoCollapsed = true;
                IsExpanded = false;
            }
        }
        else if (_autoCollapsed && args.NewSize.Width >= CollapseThreshold)
        {
            _autoCollapsed = false;
            IsExpanded = true;
        }
        UpdateVisualState();
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs args)
    {
        if (args.Key == Windows.System.VirtualKey.F10 && IsExpansionToggleEnabled)
        {
            ToggleExpansion();
            args.Handled = true;
        }
        else if (args.Key == Windows.System.VirtualKey.Escape && IsExpanded)
        {
            Collapse();
            args.Handled = true;
        }
    }

    private void UpdateVisualState()
    {
        IsOpen = IsExpanded;
        DisplayMode = IsExpanded ? CommandBarDisplayMode.Default : CommandBarDisplayMode.Compact;
        VisualStateManager.GoToState(this, IsExpanded ? "Expanded" : "Collapsed", false);
        VisualStateManager.GoToState(this, IsContextual ? "Contextual" : "Normal", false);
    }
}
