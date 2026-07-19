using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class AchievementToastPage : Page
{
    private int _sequence;

    public AchievementToastPage()
    {
        InitializeComponent();
        Toast.StateChanged += OnToastStateChanged;
        Toast.ActionRequested += (_, request) => StatusText.Text = $"Action requested: {request.Id}";
    }

    private void OnShowClick(object sender, RoutedEventArgs e) => _ = Toast.ShowAsync(CreateRequest("single", "First steps", "A local achievement request.", AchievementRarity.Rare));

    private void OnQueueClick(object sender, RoutedEventArgs e)
    {
        for (var i = 0; i < 3; i++)
        {
            var index = ++_sequence;
            _ = Toast.ShowAsync(CreateRequest($"queue-{index}", $"Milestone {index}", "Queued in this window only.", i == 2 ? AchievementRarity.Epic : AchievementRarity.Common));
        }
    }

    private void OnDismissClick(object sender, RoutedEventArgs e) => Toast.Dismiss();
    private void OnToastStateChanged(object? sender, AchievementToastEventArgs e) => StatusText.Text = $"{e.State}: {e.Request.Title}";

    private static AchievementToastRequest CreateRequest(string id, string title, string description, AchievementRarity rarity) =>
        new(id, title, description, "★", Progress: 0.8, Rarity: rarity, Duration: TimeSpan.FromSeconds(5));
}
