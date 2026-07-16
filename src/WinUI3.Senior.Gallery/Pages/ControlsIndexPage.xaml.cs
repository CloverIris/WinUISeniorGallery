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
}
