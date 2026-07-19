using Microsoft.UI.Xaml;
using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class FileCardPage : Microsoft.UI.Xaml.Controls.Page
{
    public FileCardPage()
    {
        InitializeComponent();
        Card.File = new FileCardDescriptor("file-demo", "Quarterly roadmap.pdf", "PDF", 2_400_000, DateTimeOffset.UnixEpoch.AddDays(20_000), "PDF", "Shared with Senior Gallery", new[] { new FileCardAction("preview", "Preview"), new FileCardAction("share", "Share") });
        Card.ActionInvoked += OnActionInvoked;
        Card.PreviewRequested += OnPreviewRequested;
        StatusText.Text = "Ready";
    }

    private void OnPreviewClick(object sender, RoutedEventArgs e) => Card.RequestPreview();
    private void OnShareClick(object sender, RoutedEventArgs e) => Card.InvokeAction("share");
    private void OnActionInvoked(object? sender, FileCardActionInvokedEventArgs e) => StatusText.Text = $"{e.Action.Label}: {e.File.Name}";
    private void OnPreviewRequested(object? sender, FileCardPreviewRequestedEventArgs e) { e.Handled = true; StatusText.Text = $"Preview requested: {e.File.Name}"; }
}
