using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3.Senior.Controls;

public sealed record FontPickerOption(string FamilyName, string DisplayName, string? Category = null)
{
    public string PreviewText => DisplayName;
}

public sealed class FontPickerSelectionChangedEventArgs(FontPickerOption option, bool isUserInitiated) : EventArgs
{
    public FontPickerOption Option { get; } = option;
    public bool IsUserInitiated { get; } = isUserInitiated;
}

public sealed class FontPickerOptionsChangedEventArgs(IReadOnlyList<FontPickerOption> options) : EventArgs
{
    public IReadOnlyList<FontPickerOption> Options { get; } = options;
}

/// <summary>
/// Host-fed font selector with deterministic filtering and in-memory favorites.
/// It does not install fonts, access the registry, or persist user choices.
/// </summary>
public sealed class FontPicker : Control
{
    private readonly ObservableCollection<FontPickerOption> _options = new();
    private readonly ObservableCollection<FontPickerOption> _visibleOptions = new();
    private readonly HashSet<string> _favorites = new(StringComparer.OrdinalIgnoreCase);
    private bool _selectionFromUser;

    public static readonly DependencyProperty SelectedFontFamilyProperty = DependencyProperty.Register(
        nameof(SelectedFontFamily), typeof(string), typeof(FontPicker), new PropertyMetadata(null, OnSelectedFontFamilyChanged));
    public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
        nameof(SearchText), typeof(string), typeof(FontPicker), new PropertyMetadata(string.Empty, OnFilterPropertyChanged));
    public static readonly DependencyProperty IsFavoritesOnlyProperty = DependencyProperty.Register(
        nameof(IsFavoritesOnly), typeof(bool), typeof(FontPicker), new PropertyMetadata(false, OnFilterPropertyChanged));
    public static readonly DependencyProperty IsPreviewEnabledProperty = DependencyProperty.Register(
        nameof(IsPreviewEnabled), typeof(bool), typeof(FontPicker), new PropertyMetadata(true));
    public static readonly DependencyProperty PreviewTextProperty = DependencyProperty.Register(
        nameof(PreviewText), typeof(string), typeof(FontPicker), new PropertyMetadata("Aa — WinUI 3 Senior Gallery"));

    public FontPicker()
    {
        DefaultStyleKey = typeof(FontPicker);
    }

    public string? SelectedFontFamily { get => (string?)GetValue(SelectedFontFamilyProperty); set => SetValue(SelectedFontFamilyProperty, value); }
    public FontPickerOption? SelectedOption => SelectedFontFamily is { Length: > 0 } family
        ? _options.FirstOrDefault(option => StringComparer.OrdinalIgnoreCase.Equals(option.FamilyName, family))
        : null;
    public string SearchText { get => (string?)GetValue(SearchTextProperty) ?? string.Empty; set => SetValue(SearchTextProperty, value ?? string.Empty); }
    public bool IsFavoritesOnly { get => (bool)GetValue(IsFavoritesOnlyProperty); set => SetValue(IsFavoritesOnlyProperty, value); }
    public bool IsPreviewEnabled { get => (bool)GetValue(IsPreviewEnabledProperty); set => SetValue(IsPreviewEnabledProperty, value); }
    public string PreviewText { get => (string?)GetValue(PreviewTextProperty) ?? string.Empty; set => SetValue(PreviewTextProperty, value ?? string.Empty); }
    public IReadOnlyList<FontPickerOption> Options => _options;
    public IReadOnlyList<FontPickerOption> VisibleOptions => _visibleOptions;
    public IReadOnlySet<string> FavoriteFamilies => _favorites;

    public event EventHandler<FontPickerSelectionChangedEventArgs>? SelectionChanged;
    public event EventHandler? OptionsChanged;
    public event EventHandler<FontPickerOptionsChangedEventArgs>? OptionsChangedDetailed;
    public event EventHandler? FavoritesChanged;

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    public void SetOptions(IEnumerable<FontPickerOption> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        var snapshot = options.Where(option => option is not null)
            .GroupBy(option => option.FamilyName, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToArray();
        _options.Clear();
        foreach (var option in snapshot) _options.Add(option);
        _favorites.RemoveWhere(family => !_options.Any(option => StringComparer.OrdinalIgnoreCase.Equals(option.FamilyName, family)));
        if (SelectedFontFamily is not null && !_options.Any(option => StringComparer.OrdinalIgnoreCase.Equals(option.FamilyName, SelectedFontFamily)))
        {
            SelectedFontFamily = null;
        }
        RecomputeVisibleOptions();
        OptionsChanged?.Invoke(this, EventArgs.Empty);
        OptionsChangedDetailed?.Invoke(this, new FontPickerOptionsChangedEventArgs(_options.ToArray()));
    }

    public bool Select(string familyName, bool isUserInitiated = true)
    {
        var option = _options.FirstOrDefault(item => StringComparer.OrdinalIgnoreCase.Equals(item.FamilyName, familyName));
        if (option is null) return false;
        var previous = SelectedFontFamily;
        _selectionFromUser = isUserInitiated;
        try { SelectedFontFamily = option.FamilyName; }
        finally { _selectionFromUser = false; }
        if (string.Equals(previous, option.FamilyName, StringComparison.OrdinalIgnoreCase))
            SelectionChanged?.Invoke(this, new FontPickerSelectionChangedEventArgs(option, isUserInitiated));
        return true;
    }

    public bool ToggleFavorite(string familyName)
    {
        var option = _options.FirstOrDefault(item => StringComparer.OrdinalIgnoreCase.Equals(item.FamilyName, familyName));
        if (option is null) return false;
        if (!_favorites.Add(option.FamilyName)) _favorites.Remove(option.FamilyName);
        RecomputeVisibleOptions();
        OptionsChanged?.Invoke(this, EventArgs.Empty);
        OptionsChangedDetailed?.Invoke(this, new FontPickerOptionsChangedEventArgs(_options.ToArray()));
        FavoritesChanged?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool IsFavorite(string familyName) => _favorites.Contains(familyName);

    private static void OnFilterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs _) => ((FontPicker)sender).RecomputeVisibleOptions();

    private static void OnSelectedFontFamilyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var picker = (FontPicker)sender;
        var option = picker.SelectedOption;
        if (option is not null && !string.Equals(args.OldValue as string, args.NewValue as string, StringComparison.OrdinalIgnoreCase))
            picker.SelectionChanged?.Invoke(picker, new FontPickerSelectionChangedEventArgs(option, picker._selectionFromUser));
    }

    private void RecomputeVisibleOptions()
    {
        var query = SearchText.Trim();
        _visibleOptions.Clear();
        foreach (var option in _options)
        {
            if (IsFavoritesOnly && !_favorites.Contains(option.FamilyName)) continue;
            if (query.Length > 0 && !option.FamilyName.Contains(query, StringComparison.CurrentCultureIgnoreCase) && !option.DisplayName.Contains(query, StringComparison.CurrentCultureIgnoreCase)) continue;
            _visibleOptions.Add(option);
        }
    }
}
