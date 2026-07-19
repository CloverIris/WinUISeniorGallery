using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Core;

namespace WinUI3.Senior.Media;

public sealed record MediaCenterItem(string Id, string Title, string? Subtitle = null, object? Thumbnail = null, object? Tag = null)
{
    public MediaCenterItem Normalize() => string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(Title)
        ? throw new ArgumentException("Media center items require non-empty Id and Title.")
        : this;
}

public sealed record MediaCenterCategory(string Id, string Header, IReadOnlyList<MediaCenterItem>? Items = null, object? Tag = null)
{
    public MediaCenterCategory Normalize()
    {
        if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(Header)) throw new ArgumentException("Media center categories require non-empty Id and Header.");
        var items = (Items ?? Array.Empty<MediaCenterItem>()).Select(item => item.Normalize()).GroupBy(item => item.Id, StringComparer.Ordinal).Select(group => group.First()).ToArray();
        return this with { Items = items };
    }
}

public enum MediaCenterExperienceState
{
    Empty,
    Browse,
    Details,
    Playback,
    Error
}

public sealed class MediaCenterSelectionRequestedEventArgs(MediaCenterCategory category, MediaCenterItem item) : EventArgs
{
    public MediaCenterCategory Category { get; } = category ?? throw new ArgumentNullException(nameof(category));
    public MediaCenterItem Item { get; } = item ?? throw new ArgumentNullException(nameof(item));
}

[TemplatePart(Name = "PART_Grid", Type = typeof(MediaCenterGrid))]
[TemplatePart(Name = "PART_DetailOverlay", Type = typeof(Border))]
[TemplatePart(Name = "PART_DetailTitle", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_DetailSubtitle", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_CloseDetailsButton", Type = typeof(Button))]
[TemplatePart(Name = "PART_PlayButton", Type = typeof(Button))]
public sealed class MediaCenterExperience : Control
{
    private readonly ObservableCollection<MediaCenterCategory> _categories = new();
    private MediaCenterGrid? _grid;
    private Border? _detailOverlay;
    private TextBlock? _detailTitle;
    private TextBlock? _detailSubtitle;
    private Button? _closeDetailsButton;
    private Button? _playButton;
    private int _selectedCategoryIndex = -1;
    private MediaCenterItem? _selectedItem;

    public static readonly DependencyProperty IsDetailsOpenProperty = DependencyProperty.Register(nameof(IsDetailsOpen), typeof(bool), typeof(MediaCenterExperience), new PropertyMetadata(false, OnVisualStateChanged));
    public static readonly DependencyProperty StateProperty = DependencyProperty.Register(nameof(State), typeof(MediaCenterExperienceState), typeof(MediaCenterExperience), new PropertyMetadata(MediaCenterExperienceState.Empty, OnVisualStateChanged));
    public static readonly DependencyProperty IsTenFootModeProperty = DependencyProperty.Register(nameof(IsTenFootMode), typeof(bool), typeof(MediaCenterExperience), new PropertyMetadata(true));

    public MediaCenterExperience()
    {
        DefaultStyleKey = typeof(MediaCenterExperience);
        _categories.CollectionChanged += (_, _) => ApplyCategory();
    }

    public ObservableCollection<MediaCenterCategory> Categories => _categories;
    public int SelectedCategoryIndex => _selectedCategoryIndex;
    public MediaCenterCategory? SelectedCategory => _selectedCategoryIndex >= 0 && _selectedCategoryIndex < _categories.Count ? _categories[_selectedCategoryIndex] : null;
    public MediaCenterItem? SelectedItem => _selectedItem;
    public bool IsDetailsOpen { get => (bool)GetValue(IsDetailsOpenProperty); private set => SetValue(IsDetailsOpenProperty, value); }
    public MediaCenterExperienceState State { get => (MediaCenterExperienceState)GetValue(StateProperty); private set => SetValue(StateProperty, value); }
    public bool IsTenFootMode { get => (bool)GetValue(IsTenFootModeProperty); set => SetValue(IsTenFootModeProperty, value); }
    public event EventHandler<MediaCenterSelectionRequestedEventArgs>? SelectionRequested;
    public event EventHandler? DetailsClosed;
    public event EventHandler? CategoryChanged;

    public void SetCategories(IEnumerable<MediaCenterCategory> categories)
    {
        ArgumentNullException.ThrowIfNull(categories);
        var normalized = categories.Select(category => category.Normalize()).GroupBy(category => category.Id, StringComparer.Ordinal).Select(group => group.First()).ToArray();
        _categories.Clear();
        foreach (var category in normalized) _categories.Add(category);
        _selectedCategoryIndex = _categories.Count == 0 ? -1 : 0;
        ApplyCategory();
    }

    public bool SelectCategory(int index)
    {
        if (index < 0 || index >= _categories.Count) return false;
        _selectedCategoryIndex = index;
        IsDetailsOpen = false;
        State = MediaCenterExperienceState.Browse;
        ApplyCategory();
        CategoryChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool SelectItem(string id)
    {
        var category = SelectedCategory;
        var item = category?.Items?.FirstOrDefault(candidate => string.Equals(candidate.Id, id, StringComparison.Ordinal));
        if (category is null || item is null) return false;
        _selectedItem = item;
        IsDetailsOpen = true;
        State = MediaCenterExperienceState.Details;
        UpdateDetailVisuals();
        SelectionRequested?.Invoke(this, new MediaCenterSelectionRequestedEventArgs(category, item));
        return true;
    }

    public void CloseDetails()
    {
        if (!IsDetailsOpen) return;
        IsDetailsOpen = false;
        State = SelectedCategory is null ? MediaCenterExperienceState.Empty : MediaCenterExperienceState.Browse;
        DetailsClosed?.Invoke(this, EventArgs.Empty);
        UpdateDetailVisuals();
    }

    public void RequestPlayback()
    {
        if (_selectedItem is null || SelectedCategory is null) return;
        State = MediaCenterExperienceState.Playback;
        SelectionRequested?.Invoke(this, new MediaCenterSelectionRequestedEventArgs(SelectedCategory, _selectedItem));
    }

    protected override void OnApplyTemplate()
    {
        DetachTemplateHandlers();
        base.OnApplyTemplate();
        _grid = GetTemplateChild("PART_Grid") as MediaCenterGrid;
        _detailOverlay = GetTemplateChild("PART_DetailOverlay") as Border;
        _detailTitle = GetTemplateChild("PART_DetailTitle") as TextBlock;
        _detailSubtitle = GetTemplateChild("PART_DetailSubtitle") as TextBlock;
        _closeDetailsButton = GetTemplateChild("PART_CloseDetailsButton") as Button;
        _playButton = GetTemplateChild("PART_PlayButton") as Button;
        if (_grid is not null) _grid.SelectionChanged += OnGridSelectionChanged;
        if (_closeDetailsButton is not null) _closeDetailsButton.Click += OnCloseDetailsClick;
        if (_playButton is not null) _playButton.Click += OnPlayClick;
        ApplyCategory();
    }

    private static void OnVisualStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs _) => ((MediaCenterExperience)d).UpdateDetailVisuals();
    private void ApplyCategory()
    {
        if (_grid is null) return;
        var category = SelectedCategory;
        _grid.ItemsSource = category?.Items?.ToArray() ?? Array.Empty<MediaCenterItem>();
        if (category is null) { State = MediaCenterExperienceState.Empty; _selectedItem = null; }
        else if (!IsDetailsOpen) State = MediaCenterExperienceState.Browse;
        UpdateDetailVisuals();
    }

    private void OnGridSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_grid?.SelectedItem is MediaCenterItem item) SelectItem(item.Id);
    }

    private void OnCloseDetailsClick(object sender, RoutedEventArgs e) => CloseDetails();
    private void OnPlayClick(object sender, RoutedEventArgs e) => RequestPlayback();
    private void UpdateDetailVisuals()
    {
        if (_detailOverlay is not null) _detailOverlay.Visibility = IsDetailsOpen ? Visibility.Visible : Visibility.Collapsed;
        if (_detailTitle is not null) _detailTitle.Text = _selectedItem?.Title ?? string.Empty;
        if (_detailSubtitle is not null) _detailSubtitle.Text = _selectedItem?.Subtitle ?? string.Empty;
    }

    private void DetachTemplateHandlers()
    {
        if (_grid is not null) _grid.SelectionChanged -= OnGridSelectionChanged;
        if (_closeDetailsButton is not null) _closeDetailsButton.Click -= OnCloseDetailsClick;
        if (_playButton is not null) _playButton.Click -= OnPlayClick;
        _grid = null; _detailOverlay = null; _detailTitle = null; _detailSubtitle = null; _closeDetailsButton = null; _playButton = null;
    }
}
