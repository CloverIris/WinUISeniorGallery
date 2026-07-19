using Windows.Foundation;
using WinUI3.Senior.Windowing;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class DetachablePlayerHostPage : Microsoft.UI.Xaml.Controls.Page
{
    private readonly FakeFloatingHost _fakeHost = new();

    public DetachablePlayerHostPage()
    {
        InitializeComponent();
        PlayerHost.AttachHost(_fakeHost);
        UpdateStatus();
    }

    protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        PlayerHost.DetachHost();
        base.OnNavigatedFrom(e);
    }

    private async void OnDetachClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => await PlayerHost.DetachAsync();
    private async void OnAttachClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => await PlayerHost.AttachAsync();
    private async void OnToggleClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => await PlayerHost.ToggleAsync();
    private void OnOwnerCloseClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => _fakeHost.SimulateOwnerClose();
    private void OnStateChanged(object? sender, EventArgs e) => UpdateStatus();
    private void OnOperationCompleted(object? sender, DetachablePlayerOperationResult e) => StatusText.Text = $"{e.Status}: {e.State} ({e.ErrorCode ?? "none"})";
    private void OnHostClosed(object? sender, EventArgs e) => StatusText.Text = "Owner closed: returned inline";
    private void UpdateStatus() => StatusText.Text = $"State: {PlayerHost.State}; host={PlayerHost.FloatingHost is not null}";

    private sealed class FakeFloatingHost : IFloatingWidgetHost
    {
        private bool _isOpen;
        public event EventHandler? OwnerClosed;
        public Task<FloatingWidgetOperationResult> OpenAsync(FloatingWidgetRequest request, object? content, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _isOpen = true;
            return Task.FromResult(FloatingWidgetOperationResult.Succeeded());
        }
        public Task<FloatingWidgetOperationResult> CloseAsync(string widgetId, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            _isOpen = false;
            return Task.FromResult(FloatingWidgetOperationResult.Succeeded());
        }
        public Task<FloatingWidgetOperationResult> RestoreAsync(string widgetId, CancellationToken cancellationToken = default) =>
            Task.FromResult(FloatingWidgetOperationResult.Succeeded());
        public Task<FloatingWidgetOperationResult> MinimizeAsync(string widgetId, CancellationToken cancellationToken = default) =>
            Task.FromResult(FloatingWidgetOperationResult.Succeeded());
        public void SimulateOwnerClose() { if (!_isOpen) return; _isOpen = false; OwnerClosed?.Invoke(this, EventArgs.Empty); }
    }
}
