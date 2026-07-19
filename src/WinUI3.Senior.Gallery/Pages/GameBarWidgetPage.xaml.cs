using WinUI3.Senior.Windowing;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class GameBarWidgetPage : Microsoft.UI.Xaml.Controls.Page
{
    private readonly FakeFloatingHost _fakeHost = new();

    public GameBarWidgetPage()
    {
        InitializeComponent();
        Widget.AttachHost(_fakeHost);
        UpdateStatus();
    }

    protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        Widget.DetachHost();
        base.OnNavigatedFrom(e);
    }

    private async void OnOpenClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => await Widget.OpenAsync();
    private async void OnCloseClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => await Widget.CloseAsync();
    private void OnClickThroughClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Widget.SetInteractionMode(GameBarWidgetInteractionMode.ClickThrough);
    private void OnInteractiveClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Widget.SetInteractionMode(GameBarWidgetInteractionMode.Interactive);
    private void OnMinimizeClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Widget.Minimize();
    private void OnInteractionRequested(object? sender, GameBarWidgetInteractionRequestedEventArgs e) { e.Handled = true; StatusText.Text = $"Host accepted {e.Mode} ({e.RecoveryHotKey})"; }
    private void OnStateChanged(object? sender, EventArgs e) => UpdateStatus();
    private void UpdateStatus() => StatusText.Text = $"State: {Widget.State}; mode={Widget.InteractionMode}";

    private sealed class FakeFloatingHost : IFloatingWidgetHost
    {
        public event EventHandler? OwnerClosed;
        public Task<FloatingWidgetOperationResult> OpenAsync(FloatingWidgetRequest request, object? content, CancellationToken cancellationToken = default) { cancellationToken.ThrowIfCancellationRequested(); return Task.FromResult(FloatingWidgetOperationResult.Succeeded()); }
        public Task<FloatingWidgetOperationResult> CloseAsync(string widgetId, CancellationToken cancellationToken = default) { cancellationToken.ThrowIfCancellationRequested(); return Task.FromResult(FloatingWidgetOperationResult.Succeeded()); }
        public Task<FloatingWidgetOperationResult> RestoreAsync(string widgetId, CancellationToken cancellationToken = default) => Task.FromResult(FloatingWidgetOperationResult.Succeeded());
        public Task<FloatingWidgetOperationResult> MinimizeAsync(string widgetId, CancellationToken cancellationToken = default) => Task.FromResult(FloatingWidgetOperationResult.Succeeded());
        public void CloseOwner() => OwnerClosed?.Invoke(this, EventArgs.Empty);
    }
}
