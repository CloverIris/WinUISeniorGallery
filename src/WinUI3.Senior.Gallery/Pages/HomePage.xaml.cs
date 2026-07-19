using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3_Senior_Gallery.Models;
using System.Collections.ObjectModel;

namespace WinUI3_Senior_Gallery.Pages;

/// <summary>Local landing dashboard for the Gallery shell.</summary>
public sealed partial class HomePage : Page
{
    public IReadOnlyList<GalleryFeatureCard> FeatureCards => GalleryData.FeatureCards;
    public IReadOnlyList<DashboardWidget> Widgets => GalleryData.Widgets;
    public ObservableCollection<GalleryFeatureCard> VisibleFeatures { get; } = [];

    public HomePage()
    {
        InitializeComponent();
        DataContext = this;
        RefreshFeatureFilter();
    }

    private void OnFeatureSearchChanged(object sender, TextChangedEventArgs e) => RefreshFeatureFilter();

    private void OnFeatureCategoryChanged(object sender, SelectionChangedEventArgs e) => RefreshFeatureFilter();

    private void RefreshFeatureFilter()
    {
        var query = FeatureSearchBox?.Text?.Trim() ?? string.Empty;
        var category = (FeatureFilterComboBox?.SelectedItem as ComboBoxItem)?.Content?.ToString();
        var filtered = FeatureCards.Where(card =>
            (string.IsNullOrWhiteSpace(query)
             || card.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
             || card.Description.Contains(query, StringComparison.OrdinalIgnoreCase)
             || card.Category.Contains(query, StringComparison.OrdinalIgnoreCase))
            && (string.IsNullOrWhiteSpace(category) || category == "全部分类" || card.Category == category));

        VisibleFeatures.Clear();
        foreach (var card in filtered)
        {
            VisibleFeatures.Add(card);
        }

        if (FeatureCountText is not null)
        {
            FeatureCountText.Text = $"快速入口 · {VisibleFeatures.Count} 个匹配项";
        }
    }

    private void OnFeatureClick(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.Tag is not string route)
        {
            return;
        }

        var pageType = route switch
        {
            "controls/carousel-view" => typeof(CarouselViewPage),
            "media/media-player-chrome" => typeof(MediaPlayerChromePage),
            "media/media-timeline" => typeof(MediaTimelinePage),
            "media/timed-text-view" => typeof(TimedTextViewPage),
            "media/workbench" => typeof(MediaWorkbenchPage),
            "windowing" => typeof(WindowingIndexPage),
            "archaeology" => typeof(ArchaeologyIndexPage),
            _ => typeof(ExperiencesIndexPage),
        };
        Frame?.Navigate(pageType, route);
    }

    private void OnWidgetRefreshClick(object sender, RoutedEventArgs e)
    {
        if ((sender as Button)?.Tag is DashboardWidget widget)
        {
            widget.Refresh();
        }
    }

    private void OnOpenPlaygroundClick(object sender, RoutedEventArgs e) =>
        Frame?.Navigate(typeof(PlaygroundPage), "playground");

    private void OnOpenAboutClick(object sender, RoutedEventArgs e) =>
        Frame?.Navigate(typeof(AboutPage), "about");
}
