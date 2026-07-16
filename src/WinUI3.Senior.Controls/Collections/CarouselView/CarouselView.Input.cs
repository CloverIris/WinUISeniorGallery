using System;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;
using Windows.UI.Core;

namespace WinUI3.Senior.Controls;

public sealed partial class CarouselView
{
    private const double SwipeDistanceRatio = 0.20;
    private const float SwipeVelocityThreshold = 500f;

    private ButtonBase? _previousButton;
    private ButtonBase? _nextButton;
    private FrameworkElement? _presentationRoot;
    private TextBlock? _liveRegion;
    private bool _isFocusWithin;
    private bool _isManipulating;
    private double _manipulationTranslation;

    /// <summary>Called by the core template path after its required repeater validation.</summary>
    internal void OnPresentationApplyTemplate()
    {
        DetachPresentationHandlers();

        _presentationRoot = GetTemplateChild("PART_Root") as FrameworkElement;
        _previousButton = GetTemplateChild("PART_PreviousButton") as ButtonBase;
        _nextButton = GetTemplateChild("PART_NextButton") as ButtonBase;
        _liveRegion = GetTemplateChild("PART_LiveRegion") as TextBlock;

        if (_presentationRoot is not null)
        {
            _presentationRoot.PointerEntered += OnPresentationPointerEntered;
            _presentationRoot.PointerExited += OnPresentationPointerExited;
            _presentationRoot.PointerWheelChanged += OnPresentationPointerWheelChanged;
            _presentationRoot.ManipulationStarted += OnPresentationManipulationStarted;
            _presentationRoot.ManipulationDelta += OnPresentationManipulationDelta;
            _presentationRoot.ManipulationCompleted += OnPresentationManipulationCompleted;
            _presentationRoot.ManipulationMode = ManipulationModes.TranslateX;
        }

        _presentationTransition ??= new CarouselTransitionController(this);

        GotFocus += OnCarouselGotFocus;
        LostFocus += OnCarouselLostFocus;
        KeyDown += OnCarouselKeyDown;

        if (_previousButton is not null)
        {
            _previousButton.Click += OnPreviousButtonClick;
        }

        if (_nextButton is not null)
        {
            _nextButton.Click += OnNextButtonClick;
        }

        if (_liveRegion is not null)
        {
            AutomationProperties.SetLiveSetting(_liveRegion, AutomationLiveSetting.Polite);
        }
    }

    internal void OnPresentationUnloaded()
    {
        DetachPresentationHandlers();
        _presentationTransition?.Dispose();
    }

    private void DetachPresentationHandlers()
    {
        if (_presentationRoot is not null)
        {
            _presentationRoot.PointerEntered -= OnPresentationPointerEntered;
            _presentationRoot.PointerExited -= OnPresentationPointerExited;
            _presentationRoot.PointerWheelChanged -= OnPresentationPointerWheelChanged;
            _presentationRoot.ManipulationStarted -= OnPresentationManipulationStarted;
            _presentationRoot.ManipulationDelta -= OnPresentationManipulationDelta;
            _presentationRoot.ManipulationCompleted -= OnPresentationManipulationCompleted;
        }

        if (_previousButton is not null)
        {
            _previousButton.Click -= OnPreviousButtonClick;
        }

        if (_nextButton is not null)
        {
            _nextButton.Click -= OnNextButtonClick;
        }

        GotFocus -= OnCarouselGotFocus;
        LostFocus -= OnCarouselLostFocus;
        KeyDown -= OnCarouselKeyDown;
        _presentationRoot = null;
        _previousButton = null;
        _nextButton = null;
        _liveRegion = null;
    }

    private void OnPreviousButtonClick(object sender, RoutedEventArgs e) =>
        MovePreviousFromUser();

    private void OnNextButtonClick(object sender, RoutedEventArgs e) =>
        MoveNextFromUser();

    private void OnCarouselKeyDown(object sender, KeyRoutedEventArgs e)
    {
        var isRtl = FlowDirection == FlowDirection.RightToLeft;
        switch (e.Key)
        {
            case VirtualKey.Left:
            case VirtualKey.GamepadDPadLeft:
                NavigateBy(isRtl ? 1 : -1, e.Key == VirtualKey.Left ? CarouselInputDeviceKind.Keyboard : CarouselInputDeviceKind.GameController);
                e.Handled = true;
                break;
            case VirtualKey.Right:
            case VirtualKey.GamepadDPadRight:
                NavigateBy(isRtl ? -1 : 1, e.Key == VirtualKey.Right ? CarouselInputDeviceKind.Keyboard : CarouselInputDeviceKind.GameController);
                e.Handled = true;
                break;
            case VirtualKey.Home:
                MoveToFromUser(0);
                e.Handled = true;
                break;
            case VirtualKey.End:
                MoveToFromUser(Math.Max(0, Items.Count - 1));
                e.Handled = true;
                break;
            case VirtualKey.Enter:
            case VirtualKey.Space:
            case VirtualKey.GamepadA:
                InvokeSelectedItem(e.Key == VirtualKey.GamepadA ? CarouselInputDeviceKind.GameController : CarouselInputDeviceKind.Keyboard);
                e.Handled = true;
                break;
        }
    }

    private void OnPresentationPointerWheelChanged(object sender, PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint(_presentationRoot);
        var horizontal = point.Properties.IsHorizontalMouseWheel ||
            InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down);
        if (!horizontal || point.Properties.MouseWheelDelta == 0)
        {
            return;
        }

        // A precision touchpad is intentionally reported as Unknown: WinUI does not reliably distinguish it here.
        if (point.Properties.MouseWheelDelta < 0)
        {
            MoveNextFromUser();
        }
        else
        {
            MovePreviousFromUser();
        }
        e.Handled = true;
    }

    private void OnPresentationPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        SetPointerOverState(true);
    }

    private void OnPresentationPointerExited(object sender, PointerRoutedEventArgs e)
    {
        SetPointerOverState(false);
    }

    private void OnCarouselGotFocus(object sender, RoutedEventArgs e)
    {
        _isFocusWithin = true;
        SetKeyboardFocusWithinState(true);
    }

    private void OnCarouselLostFocus(object sender, RoutedEventArgs e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            var focused = FocusManager.GetFocusedElement(XamlRoot) as DependencyObject;
            _isFocusWithin = focused is not null && IsVisualDescendantOf(focused, this);
            SetKeyboardFocusWithinState(_isFocusWithin);
        });
    }

    private static bool IsVisualDescendantOf(DependencyObject child, DependencyObject ancestor)
    {
        for (DependencyObject? current = child; current is not null; current = VisualTreeHelper.GetParent(current))
        {
            if (ReferenceEquals(current, ancestor))
            {
                return true;
            }
        }

        return false;
    }

    private void OnPresentationManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
    {
        if (!IsSwipeEnabled)
        {
            return;
        }

        _isManipulating = true;
        _manipulationTranslation = 0;
        SetUserInteractionActive(true);
    }

    private void OnPresentationManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
    {
        if (!_isManipulating)
        {
            return;
        }

        _manipulationTranslation += e.Delta.Translation.X;
        _presentationTransition?.SetDragOffset(_manipulationTranslation);
    }

    private void OnPresentationManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
    {
        if (!_isManipulating)
        {
            return;
        }

        _isManipulating = false;
        var width = Math.Max(1, ActualWidth);
        var isSwipe = Math.Abs(_manipulationTranslation) >= width * SwipeDistanceRatio ||
            Math.Abs(e.Velocities.Linear.X) >= SwipeVelocityThreshold;
        if (isSwipe)
        {
            // A left physical swipe advances in LTR and reverses in RTL.
            var physicalDirection = _manipulationTranslation < 0 ? 1 : -1;
            if (FlowDirection == FlowDirection.RightToLeft ? -physicalDirection > 0 : physicalDirection > 0)
            {
                MoveNextFromUser();
            }
            else
            {
                MovePreviousFromUser();
            }
        }

        _presentationTransition?.SettleDrag();
        SetUserInteractionActive(false);
    }

    private void NavigateBy(int delta, CarouselInputDeviceKind inputDeviceKind)
    {
        if (Items.Count == 0 || delta == 0)
        {
            return;
        }

        if (delta > 0)
        {
            MoveNextFromUser();
        }
        else
        {
            MovePreviousFromUser();
        }
    }

    /// <summary>Only user-driven navigation calls this method; autoplay must not announce.</summary>
    internal void PublishUserSelectionAnnouncement()
    {
        if (_liveRegion is null || SelectedIndex < 0 || Items.Count == 0)
        {
            return;
        }

        _liveRegion.Text = $"Item {SelectedIndex + 1} of {Items.Count}";
        var peer = FrameworkElementAutomationPeer.FromElement(_liveRegion) ?? FrameworkElementAutomationPeer.CreatePeerForElement(_liveRegion);
        peer?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
    }
}
