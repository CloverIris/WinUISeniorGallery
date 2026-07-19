using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUI3_Senior_Gallery.Pages;

namespace WinUI3_Senior_Gallery;

/// <summary>Gallery shell and the single owner of top-level navigation.</summary>
public sealed partial class MainWindow : Window
{
    private static readonly IReadOnlyDictionary<string, Type> Routes = new Dictionary<string, Type>(StringComparer.Ordinal)
    {
        ["home"] = typeof(HomePage),
        ["controls"] = typeof(ControlsIndexPage),
        ["controls/carousel-view"] = typeof(CarouselViewPage),
        ["controls/achievement-toast"] = typeof(AchievementToastPage),
        ["controls/overlay-menu"] = typeof(OverlayMenuPage),
        ["controls/big-title"] = typeof(BigTitlePage),
        ["controls/content-rail"] = typeof(ContentRailPage),
        ["controls/workbench"] = typeof(ControlsWorkbenchPage),
        ["media"] = typeof(MediaIndexPage),
        ["media/media-player-chrome"] = typeof(MediaPlayerChromePage),
        ["media/media-timeline"] = typeof(MediaTimelinePage),
        ["media/timed-text-view"] = typeof(TimedTextViewPage),
        ["media/media-center-grid"] = typeof(MediaCenterGridPage),
        ["media/workbench"] = typeof(MediaWorkbenchPage),
        ["windowing"] = typeof(WindowingIndexPage),
        ["experiences"] = typeof(ExperiencesIndexPage),
        ["experiences/immersive-now-playing"] = typeof(ImmersiveNowPlayingPage),
        ["experiences/focus-session"] = typeof(FocusSessionPage),
        ["experiences/tabbed-shell"] = typeof(TabbedShellPage),
        ["experiences/quick-resume"] = typeof(QuickResumePage),
        ["experiences/immersive-reader"] = typeof(ImmersiveReaderPage),
        ["experiences/hub-panorama"] = typeof(HubPanoramaPage),
        ["experiences/people-card"] = typeof(PeopleCardPage),
        ["experiences/file-card"] = typeof(FileCardPage),
        ["experiences/editor-canvas"] = typeof(EditorCanvasPage),
        ["experiences/mixview"] = typeof(MixviewPage),
        ["experiences/guide-menu"] = typeof(GuideMenuPage),
        ["experiences/detachable-player-host"] = typeof(DetachablePlayerHostPage),
        ["experiences/game-bar-widget"] = typeof(GameBarWidgetPage),
        ["experiences/media-center"] = typeof(MediaCenterExperiencePage),
        ["experiences/captions-translation"] = typeof(CaptionsTranslationPage),
        ["archaeology"] = typeof(ArchaeologyIndexPage),
        ["playground"] = typeof(PlaygroundPage),
        ["settings"] = typeof(SettingsPage),
        ["about"] = typeof(AboutPage),
    };

    private bool _isWindowActive = true;

    internal nint WindowHandle => WinRT.Interop.WindowNative.GetWindowHandle(this);

    public MainWindow()
    {
        InitializeComponent();
        AppWindow.SetIcon("Assets/AppIcon.ico");
        Activated += OnWindowActivated;
        RootNavigationView.SelectedItem = RootNavigationView.MenuItems[0];
        NavigateTo("home");
    }

    private void OnNavigationSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem { Tag: string route })
        {
            NavigateTo(route);
        }
    }

    private void OnNavigationBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
    {
        if (ContentFrame.CanGoBack)
        {
            ContentFrame.GoBack();
        }
    }

    private void OnContentFrameNavigated(object sender, NavigationEventArgs args)
    {
        RootNavigationView.IsBackEnabled = ContentFrame.CanGoBack;
        SelectNavigationItem(args.Parameter as string);
        UpdateHostWindowActivation();
    }

    private void NavigateTo(string route)
    {
        if (Routes.TryGetValue(route, out var pageType) && ContentFrame.CurrentSourcePageType != pageType)
        {
            ContentFrame.Navigate(pageType, route);
        }
    }

    private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        _isWindowActive = args.WindowActivationState != WindowActivationState.Deactivated;
        UpdateHostWindowActivation();
    }

    private void UpdateHostWindowActivation()
    {
        if (ContentFrame.Content is CarouselViewPage carouselPage)
        {
            carouselPage.SetHostWindowActive(_isWindowActive);
        }
    }

    private void SelectNavigationItem(string? route)
    {
        if (string.IsNullOrEmpty(route))
        {
            return;
        }

        var item = FindNavigationItem(RootNavigationView.MenuItems, route)
            ?? FindNavigationItem(RootNavigationView.FooterMenuItems, route);
        if (item is not null && !ReferenceEquals(RootNavigationView.SelectedItem, item))
        {
            RootNavigationView.SelectedItem = item;
        }
    }

    private static NavigationViewItem? FindNavigationItem(IList<object> items, string route)
    {
        foreach (var entry in items)
        {
            if (entry is not NavigationViewItem item)
            {
                continue;
            }

            if (string.Equals(item.Tag as string, route, StringComparison.Ordinal))
            {
                return item;
            }

            var nested = FindNavigationItem(item.MenuItems, route);
            if (nested is not null)
            {
                return nested;
            }
        }

        return null;
    }
}
