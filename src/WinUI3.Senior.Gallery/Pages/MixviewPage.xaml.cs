using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class MixviewPage : Microsoft.UI.Xaml.Controls.Page
{
    public MixviewPage()
    {
        InitializeComponent();
        Mixview.SetNodes(
        [
            new MixNode("artist", "Artist", "Root", ["album", "related"]),
            new MixNode("album", "Album", "Album", ["track", "related"]),
            new MixNode("track", "Track", "Track", ["lyrics", "artist"]),
            new MixNode("lyrics", "Lyrics", "Text", ["track"]),
            new MixNode("related", "Related", "Recommendation", ["artist", "album"]),
        ]);
        Mixview.Open("artist");
        UpdateStatus("artist");
    }

    private void OnOpenClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Mixview.Open();
    private void OnCloseClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Mixview.Close();
    private void OnRootClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Mixview.SelectNode("artist");
    private void OnReducedMotionChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Mixview.IsReducedMotion = ReducedMotionToggle.IsChecked == true;
    private void OnNodeSelected(object? sender, MixNodeSelectedEventArgs e) => UpdateStatus(e.Node.Id);
    private void OnClosed(object? sender, EventArgs e) => StatusText.Text = "Closed (host owns navigation)";
    private void UpdateStatus(string id) => StatusText.Text = $"Selected: {id}; open={Mixview.IsOpen}; reducedMotion={Mixview.IsReducedMotion}";
}
