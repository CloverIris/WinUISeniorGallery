using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Media;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class MediaCenterGridPage : Page
{
    public MediaCenterGridPage()
    {
        InitializeComponent();
        GridView.ItemsSource = Enumerable.Range(1, 18).Select(index => $"Poster {index:00}").ToArray();
        GridView.FocusChanged += (_, args) => UpdateStatus(args.Index, args.Item);
        GridView.ItemInvoked += (_, args) => StatusText.Text = $"Invoked {args.Item} · {args.InputDeviceKind}";
        UpdateStatus(GridView.SelectedIndex, GridView.SelectedItem);
    }

    private void OnTenFootChanged(object sender, RoutedEventArgs e) => GridView.IsTenFootMode = TenFootToggle.IsChecked == true;
    private void OnLeftClick(object sender, RoutedEventArgs e) => GridView.MoveFocus(MediaCenterNavigationDirection.Left);
    private void OnRightClick(object sender, RoutedEventArgs e) => GridView.MoveFocus(MediaCenterNavigationDirection.Right);
    private void OnUpClick(object sender, RoutedEventArgs e) => GridView.MoveFocus(MediaCenterNavigationDirection.Up);
    private void OnDownClick(object sender, RoutedEventArgs e) => GridView.MoveFocus(MediaCenterNavigationDirection.Down);
    private void UpdateStatus(int index, object? item) => StatusText.Text = $"Focused {index}: {item ?? "none"}";
}
