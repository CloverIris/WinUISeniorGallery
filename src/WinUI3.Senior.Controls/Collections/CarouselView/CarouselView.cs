using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace WinUI3.Senior.Controls;

/// <summary>A virtualizing, input-agnostic selector for one active item and a small adjacent buffer.</summary>
// WinUI exposes Selector as a non-activatable base in C#. ListView is its
// public, activatable Selector implementation and preserves the selection
// contract required by CarouselView.
public sealed partial class CarouselView : ListView
{
    private readonly CarouselStateMachine _stateMachine = new();
    private readonly DispatcherQueueTimer _autoplayTimer;
    private bool _isPointerOver;
    private bool _isKeyboardFocusWithin;
    private bool _isUserInteraction;
    private bool _isLoaded;
    private object? _selectedItemBeforeItemsChange;
    private int _lastSelectedIndex = -1;
    private bool _pendingUserSelection;
    private ItemsRepeater? _repeater;
    private CarouselVirtualizingLayout? _layout;

    public static readonly DependencyProperty NavigationModeProperty = DependencyProperty.Register(
        nameof(NavigationMode), typeof(CarouselNavigationMode), typeof(CarouselView), new PropertyMetadata(CarouselNavigationMode.Bounded, OnCarouselPropertyChanged));

    public static readonly DependencyProperty TransitionProperty = DependencyProperty.Register(
        nameof(Transition), typeof(CarouselTransition), typeof(CarouselView), new PropertyMetadata(CarouselTransition.Slide));

    public static readonly DependencyProperty IsAutoplayEnabledProperty = DependencyProperty.Register(
        nameof(IsAutoplayEnabled), typeof(bool), typeof(CarouselView), new PropertyMetadata(false, OnCarouselPropertyChanged));

    /// <summary>
    /// Gets or sets whether the host window is active. Hosts own this value; CarouselView never discovers or activates a window.
    /// </summary>
    public static readonly DependencyProperty IsHostWindowActiveProperty = DependencyProperty.Register(
        nameof(IsHostWindowActive), typeof(bool), typeof(CarouselView), new PropertyMetadata(true, OnCarouselPropertyChanged));

    public static readonly DependencyProperty AutoplayIntervalProperty = DependencyProperty.Register(
        nameof(AutoplayInterval), typeof(TimeSpan), typeof(CarouselView), new PropertyMetadata(TimeSpan.FromSeconds(5), OnAutoplayIntervalChanged));

    public static readonly DependencyProperty PauseAutoplayOnPointerOverProperty = DependencyProperty.Register(
        nameof(PauseAutoplayOnPointerOver), typeof(bool), typeof(CarouselView), new PropertyMetadata(true, OnCarouselPropertyChanged));

    public static readonly DependencyProperty PauseAutoplayOnKeyboardFocusWithinProperty = DependencyProperty.Register(
        nameof(PauseAutoplayOnKeyboardFocusWithin), typeof(bool), typeof(CarouselView), new PropertyMetadata(true, OnCarouselPropertyChanged));

    public static readonly DependencyProperty RealizationBufferProperty = DependencyProperty.Register(
        nameof(RealizationBuffer), typeof(int), typeof(CarouselView), new PropertyMetadata(1, OnRealizationBufferChanged));

    public static readonly DependencyProperty IsAdjacentPreviewEnabledProperty = DependencyProperty.Register(
        nameof(IsAdjacentPreviewEnabled), typeof(bool), typeof(CarouselView), new PropertyMetadata(false));

    public static readonly DependencyProperty AdjacentPreviewExtentProperty = DependencyProperty.Register(
        nameof(AdjacentPreviewExtent), typeof(double), typeof(CarouselView), new PropertyMetadata(48d, OnAdjacentPreviewExtentChanged));

    public static readonly DependencyProperty ItemInvokedCommandProperty = DependencyProperty.Register(
        nameof(ItemInvokedCommand), typeof(ICommand), typeof(CarouselView), new PropertyMetadata(null));

    public static readonly DependencyProperty AutoplayPauseReasonProperty = DependencyProperty.Register(
        nameof(AutoplayPauseReason), typeof(CarouselAutoplayPauseReason), typeof(CarouselView), new PropertyMetadata(CarouselAutoplayPauseReason.NotVisible));

    /// <summary>Identifies the number of realized carousel elements for Lab diagnostics; it is not a layout guarantee.</summary>
    public static readonly DependencyProperty RealizedElementCountProperty = DependencyProperty.Register(
        nameof(RealizedElementCount), typeof(int), typeof(CarouselView), new PropertyMetadata(0));

    public CarouselView()
    {
        DefaultStyleKey = typeof(CarouselView);
        _autoplayTimer = DispatcherQueue.CreateTimer();
        _autoplayTimer.Interval = AutoplayInterval;
        _autoplayTimer.Tick += OnAutoplayTimerTick;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SelectionChanged += OnBaseSelectionChanged;
        RegisterPropertyChangedCallback(VisibilityProperty, (_, _) => RefreshAutoplay());
        RegisterPropertyChangedCallback(ItemsSourceProperty, (_, _) => UpdateRepeaterItemsSource());
        RegisterPropertyChangedCallback(ItemTemplateProperty, (_, _) => UpdateRepeaterItemTemplate());
        RegisterPropertyChangedCallback(ItemTemplateSelectorProperty, (_, _) => UpdateRepeaterItemTemplate());
    }

    public CarouselNavigationMode NavigationMode
    {
        get => (CarouselNavigationMode)GetValue(NavigationModeProperty);
        set => SetValue(NavigationModeProperty, value);
    }

    public CarouselTransition Transition
    {
        get => (CarouselTransition)GetValue(TransitionProperty);
        set => SetValue(TransitionProperty, value);
    }

    public bool IsAutoplayEnabled
    {
        get => (bool)GetValue(IsAutoplayEnabledProperty);
        set => SetValue(IsAutoplayEnabledProperty, value);
    }

    /// <summary>
    /// Gets or sets the active state supplied by the host window. The default is <see langword="true"/>.
    /// </summary>
    public bool IsHostWindowActive
    {
        get => (bool)GetValue(IsHostWindowActiveProperty);
        set => SetValue(IsHostWindowActiveProperty, value);
    }

    public TimeSpan AutoplayInterval
    {
        get => (TimeSpan)GetValue(AutoplayIntervalProperty);
        set => SetValue(AutoplayIntervalProperty, value);
    }

    public bool PauseAutoplayOnPointerOver
    {
        get => (bool)GetValue(PauseAutoplayOnPointerOverProperty);
        set => SetValue(PauseAutoplayOnPointerOverProperty, value);
    }

    public bool PauseAutoplayOnKeyboardFocusWithin
    {
        get => (bool)GetValue(PauseAutoplayOnKeyboardFocusWithinProperty);
        set => SetValue(PauseAutoplayOnKeyboardFocusWithinProperty, value);
    }

    public int RealizationBuffer
    {
        get => (int)GetValue(RealizationBufferProperty);
        set => SetValue(RealizationBufferProperty, value);
    }

    public bool IsAdjacentPreviewEnabled
    {
        get => (bool)GetValue(IsAdjacentPreviewEnabledProperty);
        set => SetValue(IsAdjacentPreviewEnabledProperty, value);
    }

    public double AdjacentPreviewExtent
    {
        get => (double)GetValue(AdjacentPreviewExtentProperty);
        set => SetValue(AdjacentPreviewExtentProperty, value);
    }

    public ICommand? ItemInvokedCommand
    {
        get => (ICommand?)GetValue(ItemInvokedCommandProperty);
        set => SetValue(ItemInvokedCommandProperty, value);
    }

    /// <summary>Gets the current pause reason using the contractual priority order.</summary>
    public CarouselAutoplayPauseReason AutoplayPauseReason => (CarouselAutoplayPauseReason)GetValue(AutoplayPauseReasonProperty);

    /// <summary>Gets the current realized-element count for diagnostics. This value is not a layout guarantee.</summary>
    public int RealizedElementCount => (int)GetValue(RealizedElementCountProperty);

    /// <summary>Hides Selector's writable surface from the CarouselView API while preserving Selector behavior.</summary>
    public new object? SelectedItem => base.SelectedItem;

    public event EventHandler<CarouselItemInvokedEventArgs>? ItemInvoked;

    public void MoveNext() => MoveToCore(CarouselIndexMapper.Next(SelectedIndex, Items.Count, NavigationMode), false);

    public void MovePrevious() => MoveToCore(CarouselIndexMapper.Previous(SelectedIndex, Items.Count, NavigationMode), false);

    public void MoveTo(int index)
    {
        if (Items.Count == 0)
        {
            return;
        }

        if (index < 0 || index >= Items.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        MoveToCore(index, false);
    }

    protected override void OnItemsChanged(object e)
    {
        // SelectionChanged can be raised by the base implementation. Preserve the identity captured
        // before this collection mutation so an insert/move cannot accidentally retain a replacement item.
        var selectedBeforeMutation = _selectedItemBeforeItemsChange;
        base.OnItemsChanged(e);

        var retainedIndex = FindItemIndex(selectedBeforeMutation);
        if (Items.Count == 0)
        {
            SelectedIndex = -1;
        }
        else if (retainedIndex >= 0)
        {
            SelectedIndex = retainedIndex;
        }
        else if (SelectedIndex < 0)
        {
            SelectedIndex = 0;
        }
        else if (SelectedIndex >= Items.Count)
        {
            SelectedIndex = Items.Count - 1;
        }

        _selectedItemBeforeItemsChange = base.SelectedItem;
        UpdateRepeaterItemsSource();
        UpdateLayoutState();
        RefreshAutoplay();
    }

    internal void SetUserInteractionActive(bool value)
    {
        if (_isUserInteraction == value)
        {
            return;
        }

        _isUserInteraction = value;
        RefreshAutoplay();
    }

    internal void SetPointerOverState(bool value)
    {
        if (_isPointerOver == value)
        {
            return;
        }

        _isPointerOver = value;
        RefreshAutoplay();
    }

    internal void SetKeyboardFocusWithinState(bool value)
    {
        if (_isKeyboardFocusWithin == value)
        {
            return;
        }

        _isKeyboardFocusWithin = value;
        RefreshAutoplay();
    }

    internal void SetWindowActiveState(bool value)
    {
        IsHostWindowActive = value;
    }

    internal void MoveNextFromUser() => MoveToCore(CarouselIndexMapper.Next(SelectedIndex, Items.Count, NavigationMode), true);

    internal void MovePreviousFromUser() => MoveToCore(CarouselIndexMapper.Previous(SelectedIndex, Items.Count, NavigationMode), true);

    internal void MoveToFromUser(int index)
    {
        if (Items.Count > 0 && index is >= 0 && index < Items.Count)
        {
            MoveToCore(index, true);
        }
    }

    internal void InvokeSelectedItem(CarouselInputDeviceKind inputDeviceKind)
    {
        var index = SelectedIndex;
        if (index < 0 || index >= Items.Count)
        {
            return;
        }

        var args = new CarouselItemInvokedEventArgs(Items[index], index, inputDeviceKind);
        if (ItemInvokedCommand?.CanExecute(args) == true)
        {
            ItemInvokedCommand.Execute(args);
        }

        ItemInvoked?.Invoke(this, args);
    }

    partial void OnCarouselSelectionChangedCore(int previousIndex, int currentIndex, bool isUserInitiated);
    partial void OnCarouselPauseReasonChangedCore(CarouselAutoplayPauseReason pauseReason);

    protected override void OnApplyTemplate()
    {
        OnPresentationUnloaded();
        if (_layout is not null)
        {
            _layout.RealizedElementsChanged -= OnRealizedElementsChanged;
        }

        base.OnApplyTemplate();

        _repeater = GetTemplateChild("PART_Repeater") as ItemsRepeater
            ?? throw new InvalidOperationException("CarouselView requires an ItemsRepeater named PART_Repeater.");
        _layout = new CarouselVirtualizingLayout();
        _layout.RealizedElementsChanged += OnRealizedElementsChanged;
        SetValue(RealizedElementCountProperty, 0);
        _repeater.Layout = _layout;
        UpdateRepeaterItemsSource();
        UpdateRepeaterItemTemplate();
        UpdateLayoutState();
        OnPresentationApplyTemplate();
    }

    internal UIElement? GetRealizedElement(int index) => _layout?.GetRealizedElement(index);

    internal bool IsReducedMotion
    {
        get
        {
            try
            {
                return !new Windows.UI.ViewManagement.UISettings().AnimationsEnabled;
            }
            catch
            {
                return true;
            }
        }
    }

    private static void OnCarouselPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var carousel = (CarouselView)sender;
        carousel.UpdateLayoutState();
        carousel.RefreshAutoplay();
    }

    private static void OnAutoplayIntervalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var carousel = (CarouselView)sender;
        var interval = (TimeSpan)args.NewValue;
        if (interval < TimeSpan.FromSeconds(1))
        {
            throw new ArgumentOutOfRangeException(nameof(AutoplayInterval), "AutoplayInterval must be at least one second.");
        }

        carousel._autoplayTimer.Interval = interval;
        carousel.RefreshAutoplay();
    }

    private static void OnRealizationBufferChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if ((int)args.NewValue is < 0 or > 3)
        {
            throw new ArgumentOutOfRangeException(nameof(RealizationBuffer), "RealizationBuffer must be in the range 0 through 3.");
        }
        ((CarouselView)sender).UpdateLayoutState();
    }

    private static void OnAdjacentPreviewExtentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if ((double)args.NewValue < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(AdjacentPreviewExtent));
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs args)
    {
        _isLoaded = true;
        // Unloaded/Loaded can occur without a new template pass (for example when
        // a page is cached). Reattach presentation handlers and recreate the
        // transition controller so navigation still works after returning.
        if (_repeater is not null && _presentationRoot is null)
        {
            OnPresentationApplyTemplate();
        }
        _selectedItemBeforeItemsChange = base.SelectedItem;
        _lastSelectedIndex = SelectedIndex;
        if (Items.Count > 0 && SelectedIndex < 0)
        {
            SelectedIndex = 0;
        }

        RefreshAutoplay();
    }

    private void OnUnloaded(object sender, RoutedEventArgs args)
    {
        _isLoaded = false;
        _autoplayTimer.Stop();
        OnPresentationUnloaded();
        RefreshAutoplay();
    }

    private void OnRealizedElementsChanged(object? sender, EventArgs args) =>
        SetValue(RealizedElementCountProperty, _layout?.RealizedElementCount ?? 0);

    private void OnBaseSelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        _selectedItemBeforeItemsChange = base.SelectedItem;
        var previousIndex = _lastSelectedIndex;
        _lastSelectedIndex = SelectedIndex;
        // Refresh the realization window first so the transition controller can
        // animate the newly selected container instead of observing the old layout.
        UpdateLayoutState();
        OnCarouselSelectionChangedCore(previousIndex, SelectedIndex, _pendingUserSelection);
        _pendingUserSelection = false;
        RefreshAutoplay();
    }

    private void OnAutoplayTimerTick(DispatcherQueueTimer sender, object args)
    {
        _autoplayTimer.Stop();
        if (!CanAutoplay())
        {
            RefreshAutoplay();
            return;
        }

        var next = CarouselIndexMapper.Next(SelectedIndex, Items.Count, NavigationMode);
        if (NavigationMode == CarouselNavigationMode.Bounded && next == SelectedIndex)
        {
            RefreshAutoplay();
            return;
        }

        MoveToCore(next, false);
    }

    private void MoveToCore(int index, bool isUserInitiated)
    {
        if (index < 0 || index == SelectedIndex)
        {
            RefreshAutoplay();
            return;
        }

        var previous = SelectedIndex;
        _pendingUserSelection = isUserInitiated;
        SelectedIndex = index;
        if (previous == index)
        {
            _pendingUserSelection = false;
            RefreshAutoplay();
        }
    }

    private int FindItemIndex(object? item)
    {
        if (item is null)
        {
            return -1;
        }

        for (var index = 0; index < Items.Count; index++)
        {
            if (ReferenceEquals(Items[index], item))
            {
                return index;
            }
        }

        return -1;
    }

    private void RefreshAutoplay()
    {
        var pointerOver = PauseAutoplayOnPointerOver && _isPointerOver;
        var keyboardFocusWithin = PauseAutoplayOnKeyboardFocusWithin && _isKeyboardFocusWithin;
        _stateMachine.Update(Items.Count, _isLoaded, Visibility == Visibility.Visible, IsHostWindowActive, pointerOver, keyboardFocusWithin, _isUserInteraction);
        SetValue(AutoplayPauseReasonProperty, _stateMachine.PauseReason);
        OnCarouselPauseReasonChangedCore(_stateMachine.PauseReason);

        _autoplayTimer.Stop();
        if (CanAutoplay())
        {
            _autoplayTimer.Interval = AutoplayInterval;
            _autoplayTimer.Start();
        }
    }

    private bool CanAutoplay() => IsAutoplayEnabled && _stateMachine.PauseReason == CarouselAutoplayPauseReason.None && _isLoaded && Items.Count > 1;

    private void UpdateLayoutState()
    {
        if (_layout is null)
        {
            return;
        }

        _layout.SelectedIndex = SelectedIndex;
        _layout.RealizationBuffer = RealizationBuffer;
        _layout.NavigationMode = NavigationMode;
        _layout.RefreshLayout();
    }

    private void UpdateRepeaterItemsSource()
    {
        if (_repeater is not null)
        {
            _repeater.ItemsSource = ItemsSource ?? Items;
        }

        UpdateLayoutState();
        RefreshAutoplay();
    }

    private void UpdateRepeaterItemTemplate()
    {
        if (_repeater is not null)
        {
            // ItemsRepeater consumes the ItemsControl template object. A DataTemplateSelector is honoured
            // by the inherited ItemsControl pipeline; this assignment keeps a direct DataTemplate change
            // synchronized without materializing source items.
            _repeater.ItemTemplate = ItemTemplate;
        }
    }
}
