using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml;

namespace WinUI3.Senior.Controls;

public sealed partial class CarouselView
{
    private CarouselTransitionController? _presentationTransition;

    protected override AutomationPeer OnCreateAutomationPeer() => new CarouselViewAutomationPeer(this);

    partial void OnCarouselSelectionChangedCore(int previousIndex, int currentIndex, bool isUserInitiated)
    {
        _presentationTransition?.TransitionTo(previousIndex, currentIndex, Transition);
        if (isUserInitiated)
        {
            PublishUserSelectionAnnouncement();
        }
    }

    partial void OnCarouselPauseReasonChangedCore(CarouselAutoplayPauseReason pauseReason)
    {
        var state = pauseReason == CarouselAutoplayPauseReason.UserInteraction ? "Dragging" : "Idle";
        VisualStateManager.GoToState(this, state, useTransitions: !IsReducedMotion);
    }
}
