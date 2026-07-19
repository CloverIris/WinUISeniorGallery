using Microsoft.UI.Xaml.Controls;

namespace WinUI3_Senior_Gallery.Pages;

/// <summary>Gallery route: /controls.</summary>
public sealed partial class ControlsIndexPage : Page
{
    public ControlsIndexPage()
    {
        InitializeComponent();
    }

    private void OnOpenCarouselViewClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame.Navigate(typeof(CarouselViewPage), "controls/carousel-view");

    private void OnOpenWorkbenchClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame.Navigate(typeof(ControlsWorkbenchPage), "controls/workbench");

    private void OnOpenAchievementToastClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame.Navigate(typeof(AchievementToastPage), "controls/achievement-toast");

    private void OnOpenOverlayMenuClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame.Navigate(typeof(OverlayMenuPage), "controls/overlay-menu");

    private void OnOpenBigTitleClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame.Navigate(typeof(BigTitlePage), "controls/big-title");

    private void OnOpenContentRailClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) =>
        Frame.Navigate(typeof(ContentRailPage), "controls/content-rail");
}
