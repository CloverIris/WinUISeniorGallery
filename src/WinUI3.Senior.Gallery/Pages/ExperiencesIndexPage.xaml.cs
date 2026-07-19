using Microsoft.UI.Xaml.Controls;
using WinUI3_Senior_Gallery.Models;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class ExperiencesIndexPage : Page
{
    public IReadOnlyList<ExperienceDefinition> Experiences => GalleryData.Experiences;

    public ExperiencesIndexPage()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void OnOpenImmersiveNowPlayingClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(ImmersiveNowPlayingPage), "experiences/immersive-now-playing");

    private void OnOpenFocusSessionClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(FocusSessionPage), "experiences/focus-session");

    private void OnOpenTabbedShellClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(TabbedShellPage), "experiences/tabbed-shell");

    private void OnOpenQuickResumeClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(QuickResumePage), "experiences/quick-resume");

    private void OnOpenImmersiveReaderClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(ImmersiveReaderPage), "experiences/immersive-reader");

    private void OnOpenEditorCanvasClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(EditorCanvasPage), "experiences/editor-canvas");

    private void OnOpenHubPanoramaClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(HubPanoramaPage), "experiences/hub-panorama");

    private void OnOpenPeopleCardClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(PeopleCardPage), "experiences/people-card");

    private void OnOpenFileCardClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(FileCardPage), "experiences/file-card");

    private void OnOpenMixviewClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(MixviewPage), "experiences/mixview");

    private void OnOpenGuideMenuClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(GuideMenuPage), "experiences/guide-menu");

    private void OnOpenDetachablePlayerHostClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(DetachablePlayerHostPage), "experiences/detachable-player-host");

    private void OnOpenGameBarWidgetClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(GameBarWidgetPage), "experiences/game-bar-widget");

    private void OnOpenMediaCenterClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(MediaCenterExperiencePage), "experiences/media-center");

    private void OnOpenCaptionsTranslationClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame?.Navigate(typeof(CaptionsTranslationPage), "experiences/captions-translation");
}
