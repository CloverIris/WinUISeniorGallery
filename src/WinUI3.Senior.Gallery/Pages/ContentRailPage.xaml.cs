using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class ContentRailPage : Page
{
    public ContentRailPage()
    {
        InitializeComponent();
        Rail.ItemsSource = Enumerable.Range(1, 12).Select(index => $"Content card {index:00}").ToArray();
        Rail.SeeAllCommand = new DelegateCommand(_ => StatusText.Text = "See All invoked by host");
        Rail.SelectionChangedByUser += (_, _) => UpdateStatus();
        Rail.ItemInvoked += (_, args) => StatusText.Text = $"Invoked: {args.Item}";
        UpdateStatus();
    }

    private void OnPreviousClick(object sender, RoutedEventArgs e) => Rail.ScrollPrevious();
    private void OnNextClick(object sender, RoutedEventArgs e) => Rail.ScrollNext();
    private void OnSeeAllClick(object sender, RoutedEventArgs e) => Rail.InvokeSeeAll();
    private void OnWrapChanged(object sender, RoutedEventArgs e) => Rail.IsWrapNavigationEnabled = WrapToggle.IsChecked == true;
    private void UpdateStatus() => StatusText.Text = $"Selected {Rail.SelectedIndex}: {Rail.SelectedItem ?? "none"}";

    private sealed class DelegateCommand(Action<object?> execute) : ICommand
    {
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => execute(parameter);
        public event EventHandler? CanExecuteChanged;
    }
}
