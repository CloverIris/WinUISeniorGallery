using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Media;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class QuickResumePage : Page
{
    public QuickResumePage()
    {
        InitializeComponent();
        var now = DateTimeOffset.UnixEpoch.AddDays(20000);
        Picker.SetEntries(
        [
            new QuickResumeEntry("movie", "Synthetic movie", TimeSpan.FromMinutes(12), TimeSpan.FromMinutes(90), now.AddMinutes(-2)),
            new QuickResumeEntry("album", "Synthetic album", TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(42), now.AddMinutes(-8)),
            new QuickResumeEntry("show", "Synthetic series", TimeSpan.FromMinutes(28), TimeSpan.FromMinutes(30), now.AddMinutes(-15)),
            new QuickResumeEntry("finished", "Completed item", TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10), now.AddMinutes(-30)),
        ]);
    }

    protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        Picker.Items.Clear();
        base.OnNavigatedFrom(e);
    }

    private void OnResumeClick(object sender, RoutedEventArgs e) =>
        StatusText.Text = Picker.InvokeSelected() ? $"Restore requested for {Picker.SelectedEntry?.Id}." : "The selected card cannot resume.";

    private void OnRemoveClick(object sender, RoutedEventArgs e) =>
        StatusText.Text = Picker.RemoveSelected() ? "Removed from the local lab queue." : "Removal was rejected.";

    private void OnItemInvoked(object? sender, QuickResumeItemInvokedEventArgs e) => StatusText.Text = $"Host received resume request for {e.Entry.Id}; no player was created.";

    private void OnRemoveRequested(object? sender, QuickResumeRemoveRequestedEventArgs e)
    {
        if (e.Entry.Id == "finished")
        {
            e.Cancel = true;
            StatusText.Text = "The synthetic completed item is protected in this demo.";
        }
    }
}
