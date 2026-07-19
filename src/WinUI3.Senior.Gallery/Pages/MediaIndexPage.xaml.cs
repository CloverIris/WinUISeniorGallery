using Microsoft.UI.Xaml.Controls;

namespace WinUI3_Senior_Gallery.Pages;

/// <summary>Gallery route: /media.</summary>
public sealed partial class MediaIndexPage : Page
{
    public MediaIndexPage()
    {
        InitializeComponent();
    }

    private void OnChromeClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Frame.Navigate(typeof(MediaPlayerChromePage));

    private void OnTimelineClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Frame.Navigate(typeof(MediaTimelinePage));

    private void OnTimedTextClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Frame.Navigate(typeof(TimedTextViewPage));
    private void OnMediaCenterGridClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Frame.Navigate(typeof(MediaCenterGridPage), "media/media-center-grid");
}
