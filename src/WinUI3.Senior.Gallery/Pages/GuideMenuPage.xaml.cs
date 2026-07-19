using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class GuideMenuPage : Microsoft.UI.Xaml.Controls.Page
{
    public GuideMenuPage()
    {
        InitializeComponent();
        Guide.SetNodes(
        [
            new GuideNode("home", "Home", "⌂", [new GuideNode("resume", "Quick Resume"), new GuideNode("settings", "Settings", "⚙", [new GuideNode("audio", "Audio"), new GuideNode("display", "Display")])]),
            new GuideNode("friends", "Friends", "◎", [new GuideNode("invite", "Invite player"), new GuideNode("messages", "Messages")]),
            new GuideNode("achievements", "Achievements", "★"),
        ]);
        Guide.Open();
        UpdateStatus();
    }

    private void OnOpenClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Guide.Open();
    private void OnBackClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Guide.NavigateBack();
    private void OnDismissChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Guide.IsDismissOnLeafInvoke = DismissToggle.IsChecked == true;
    private void OnNodeInvoked(object? sender, GuideNodeInvokedEventArgs e) { StatusText.Text = $"Invoked: {e.Node.Label}"; }
    private void OnNavigationChanged(object? sender, EventArgs e) => UpdateStatus();
    private void OnClosed(object? sender, EventArgs e) => StatusText.Text = "Closed (host owns navigation)";
    private void UpdateStatus() => StatusText.Text = $"Depth: {Guide.NavigationPath.Count}; items: {Guide.CurrentItems.Count}; open={Guide.IsOpen}";
}
