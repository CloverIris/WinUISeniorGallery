using WinUI3.Senior.Media;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class MediaCenterExperiencePage : Microsoft.UI.Xaml.Controls.Page
{
    public MediaCenterExperiencePage()
    {
        InitializeComponent();
        Experience.SetCategories(
        [
            new MediaCenterCategory("movies", "Movies", [new MediaCenterItem("movie-1", "Northern Lights", "Synthetic feature"), new MediaCenterItem("movie-2", "The Long Road", "Synthetic feature"), new MediaCenterItem("movie-3", "City Signals", "Synthetic feature")]),
            new MediaCenterCategory("music", "Music", [new MediaCenterItem("music-1", "Live Session", "Synthetic album"), new MediaCenterItem("music-2", "Night Drive", "Synthetic album")]),
        ]);
        UpdateStatus();
    }

    private void OnMoviesClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) { Experience.SelectCategory(0); UpdateStatus(); }
    private void OnMusicClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) { Experience.SelectCategory(1); UpdateStatus(); }
    private void OnSelectionRequested(object? sender, MediaCenterSelectionRequestedEventArgs e) => StatusText.Text = $"Intent: {e.Category.Header} / {e.Item.Title} (host owns playback)";
    private void OnDetailsClosed(object? sender, EventArgs e) => UpdateStatus();
    private void UpdateStatus() => StatusText.Text = $"State: {Experience.State}; category={Experience.SelectedCategory?.Header ?? "none"}; details={Experience.IsDetailsOpen}";
}
