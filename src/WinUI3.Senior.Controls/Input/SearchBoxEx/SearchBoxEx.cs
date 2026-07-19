using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace WinUI3.Senior.Controls;

public enum SearchBoxState
{
    Idle,
    Loading,
    Results,
    Error
}

public sealed record SearchSuggestion(string Text, string? Description = null, string? Category = null, object? Tag = null)
{
    public string DisplayText => Text;
}

public interface ISearchSuggestionProvider
{
    Task<IReadOnlyList<SearchSuggestion>> GetSuggestionsAsync(string query, CancellationToken cancellationToken);
}

public sealed class SearchQuerySubmittedEventArgs(string query, SearchSuggestion? suggestion) : EventArgs
{
    public string Query { get; } = query;
    public SearchSuggestion? Suggestion { get; } = suggestion;
}

public sealed class SearchSuggestionInvokedEventArgs(SearchSuggestion suggestion, int index) : EventArgs
{
    public SearchSuggestion Suggestion { get; } = suggestion;
    public int Index { get; } = index;
}

public sealed class SearchErrorEventArgs(string query, Exception error) : EventArgs
{
    public string Query { get; } = query;
    public Exception Error { get; } = error;
    public bool Handled { get; set; }
}

/// <summary>
/// A privacy-first search surface with cancellable, debounced suggestions.
/// It owns query state and keyboard semantics while providers remain host-owned.
/// </summary>
public sealed class SearchBoxEx : Control
{
    private readonly ObservableCollection<SearchSuggestion> _suggestions = new();
    private readonly List<string> _history = new();
    private readonly DispatcherQueueTimer _debounceTimer;
    private TextBox? _editBox;
    private ListView? _suggestionList;
    private ProgressRing? _progressRing;
    private CancellationTokenSource? _requestCts;
    private int _requestVersion;
    private bool _syncingText;
    private bool _hasFocus;
    private bool _isUnloaded;
    private string _pendingQuery = string.Empty;

    public SearchBoxEx()
    {
        DefaultStyleKey = typeof(SearchBoxEx);
        _debounceTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _debounceTimer.IsRepeating = false;
        _debounceTimer.Tick += OnDebounceTick;
        IsTabStop = true;
        KeyDown += OnControlKeyDown;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(SearchBoxEx), new PropertyMetadata(string.Empty, OnTextChanged));
    public static readonly DependencyProperty PlaceholderTextProperty = DependencyProperty.Register(
        nameof(PlaceholderText), typeof(string), typeof(SearchBoxEx), new PropertyMetadata("Search"));
    public static readonly DependencyProperty IsHistoryEnabledProperty = DependencyProperty.Register(
        nameof(IsHistoryEnabled), typeof(bool), typeof(SearchBoxEx), new PropertyMetadata(false));
    public static readonly DependencyProperty DebounceProperty = DependencyProperty.Register(
        nameof(Debounce), typeof(TimeSpan), typeof(SearchBoxEx), new PropertyMetadata(TimeSpan.FromMilliseconds(250)));
    public static readonly DependencyProperty MinimumPrefixLengthProperty = DependencyProperty.Register(
        nameof(MinimumPrefixLength), typeof(int), typeof(SearchBoxEx), new PropertyMetadata(1));
    public static readonly DependencyProperty QueryCommandProperty = DependencyProperty.Register(
        nameof(QueryCommand), typeof(ICommand), typeof(SearchBoxEx), new PropertyMetadata(null));
    public static readonly DependencyProperty SuggestionProviderProperty = DependencyProperty.Register(
        nameof(SuggestionProvider), typeof(ISearchSuggestionProvider), typeof(SearchBoxEx), new PropertyMetadata(null, OnSuggestionProviderChanged));
    public static readonly DependencyProperty SearchStateProperty = DependencyProperty.Register(
        nameof(SearchState), typeof(SearchBoxState), typeof(SearchBoxEx), new PropertyMetadata(SearchBoxState.Idle));
    public static readonly DependencyProperty IsSuggestionListOpenProperty = DependencyProperty.Register(
        nameof(IsSuggestionListOpen), typeof(bool), typeof(SearchBoxEx), new PropertyMetadata(false, OnOpenChanged));

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value ?? string.Empty); }
    public string PlaceholderText { get => (string)GetValue(PlaceholderTextProperty); set => SetValue(PlaceholderTextProperty, value); }
    public bool IsHistoryEnabled { get => (bool)GetValue(IsHistoryEnabledProperty); set => SetValue(IsHistoryEnabledProperty, value); }
    public TimeSpan Debounce { get => (TimeSpan)GetValue(DebounceProperty); set => SetValue(DebounceProperty, value < TimeSpan.Zero ? TimeSpan.Zero : value); }
    public int MinimumPrefixLength { get => (int)GetValue(MinimumPrefixLengthProperty); set => SetValue(MinimumPrefixLengthProperty, Math.Max(0, value)); }
    public ICommand? QueryCommand { get => (ICommand?)GetValue(QueryCommandProperty); set => SetValue(QueryCommandProperty, value); }
    public ISearchSuggestionProvider? SuggestionProvider { get => (ISearchSuggestionProvider?)GetValue(SuggestionProviderProperty); set => SetValue(SuggestionProviderProperty, value); }
    public SearchBoxState SearchState { get => (SearchBoxState)GetValue(SearchStateProperty); private set => SetValue(SearchStateProperty, value); }
    public bool IsSuggestionListOpen { get => (bool)GetValue(IsSuggestionListOpenProperty); private set => SetValue(IsSuggestionListOpenProperty, value); }
    public IReadOnlyList<SearchSuggestion> Suggestions => _suggestions;
    public IReadOnlyList<string> SearchHistory => _history;

    public event EventHandler<SearchQuerySubmittedEventArgs>? QuerySubmitted;
    public event EventHandler<SearchSuggestionInvokedEventArgs>? SuggestionInvoked;
    public event EventHandler<SearchErrorEventArgs>? SearchError;
    public event EventHandler? SuggestionsChanged;

    protected override AutomationPeer OnCreateAutomationPeer() => new SearchBoxExAutomationPeer(this);

    public void ClearHistory()
    {
        if (_history.Count == 0) return;
        _history.Clear();
        if (SuggestionProvider is null && IsHistoryEnabled && _pendingQuery.Length >= MinimumPrefixLength)
            _ = RefreshSuggestionsAsync();
    }

    public void FocusSearch()
    {
        _editBox?.Focus(FocusState.Programmatic);
        Focus(FocusState.Programmatic);
    }

    public async Task SubmitAsync(SearchSuggestion? suggestion = null, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested) return;
        var query = (suggestion?.Text ?? Text).Trim();
        if (query.Length == 0) return;
        _debounceTimer.Stop();
        IsSuggestionListOpen = false;
        if (IsHistoryEnabled)
        {
            _history.Remove(query);
            _history.Insert(0, query);
            if (_history.Count > 20) _history.RemoveRange(20, _history.Count - 20);
        }

        var args = new SearchQuerySubmittedEventArgs(query, suggestion);
        QuerySubmitted?.Invoke(this, args);
        if (QueryCommand?.CanExecute(query) == true) QueryCommand.Execute(query);
        await Task.CompletedTask;
    }

    public async Task RefreshSuggestionsAsync(CancellationToken cancellationToken = default)
    {
        if (_isUnloaded) return;
        var query = Text.Trim();
        _pendingQuery = query;
        if (query.Length < MinimumPrefixLength)
        {
            _requestCts?.Cancel();
            _suggestions.Clear();
            SearchState = SearchBoxState.Idle;
            IsSuggestionListOpen = false;
            SuggestionsChanged?.Invoke(this, EventArgs.Empty);
            return;
        }

        // History is deliberately an in-memory, opt-in fallback. A provider is
        // still preferred, and no filesystem/account store is ever touched.
        if (SuggestionProvider is null && IsHistoryEnabled)
        {
            _suggestions.Clear();
            foreach (var item in _history.Where(item => item.StartsWith(query, StringComparison.CurrentCultureIgnoreCase)).Take(8))
            {
                _suggestions.Add(new SearchSuggestion(item, "Recent search", "History"));
            }
            SearchState = _suggestions.Count == 0 ? SearchBoxState.Idle : SearchBoxState.Results;
            IsSuggestionListOpen = _suggestions.Count > 0 && _hasFocus;
            SuggestionsChanged?.Invoke(this, EventArgs.Empty);
            return;
        }
        if (SuggestionProvider is null)
        {
            var hadSuggestions = _suggestions.Count > 0;
            _suggestions.Clear();
            SearchState = SearchBoxState.Idle;
            IsSuggestionListOpen = false;
            _progressRing?.SetValue(ProgressRing.IsActiveProperty, false);
            if (hadSuggestions) SuggestionsChanged?.Invoke(this, EventArgs.Empty);
            return;
        }

        var version = ++_requestVersion;
        _requestCts?.Cancel();
        _requestCts?.Dispose();
        _requestCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = _requestCts.Token;
        SearchState = SearchBoxState.Loading;
        _progressRing?.SetValue(ProgressRing.IsActiveProperty, true);
        try
        {
            var result = await SuggestionProvider.GetSuggestionsAsync(query, token).ConfigureAwait(true);
            if (token.IsCancellationRequested || version != _requestVersion || !string.Equals(query, _pendingQuery, StringComparison.Ordinal)) return;
            _suggestions.Clear();
            foreach (var suggestion in result ?? Array.Empty<SearchSuggestion>())
            {
                if (suggestion is not null) _suggestions.Add(suggestion);
            }
            SearchState = _suggestions.Count == 0 ? SearchBoxState.Idle : SearchBoxState.Results;
            IsSuggestionListOpen = _suggestions.Count > 0 && _hasFocus;
            SuggestionsChanged?.Invoke(this, EventArgs.Empty);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested) { }
        catch (Exception error)
        {
            if (version != _requestVersion) return;
            SearchState = SearchBoxState.Error;
            IsSuggestionListOpen = false;
            var args = new SearchErrorEventArgs(query, error);
            SearchError?.Invoke(this, args);
            if (!args.Handled) AutomationProperties.SetHelpText(this, error.Message);
        }
        finally
        {
            if (version == _requestVersion) _progressRing?.SetValue(ProgressRing.IsActiveProperty, false);
        }
    }

    protected override void OnApplyTemplate()
    {
        DetachTemplateHandlers();
        base.OnApplyTemplate();
        _editBox = GetTemplateChild("PART_EditBox") as TextBox;
        _suggestionList = GetTemplateChild("PART_SuggestionList") as ListView;
        _progressRing = GetTemplateChild("PART_ProgressRing") as ProgressRing;
        if (_editBox is null) throw new InvalidOperationException("SearchBoxEx template must provide PART_EditBox.");
        _editBox.Text = Text;
        _editBox.PlaceholderText = PlaceholderText;
        if (_suggestionList is not null)
        {
            _suggestionList.ItemsSource = _suggestions;
            _suggestionList.SelectionChanged += OnSuggestionSelectionChanged;
            _suggestionList.KeyDown += OnSuggestionListKeyDown;
        }
        _editBox.TextChanged += OnEditBoxTextChanged;
        _editBox.GotFocus += OnEditBoxGotFocus;
        _editBox.LostFocus += OnEditBoxLostFocus;
        _editBox.KeyDown += OnEditBoxKeyDown;
        UpdateVisualState();
        if (!_isUnloaded) ScheduleSuggestions();
    }

    private void DetachTemplateHandlers()
    {
        if (_editBox is not null)
        {
            _editBox.TextChanged -= OnEditBoxTextChanged;
            _editBox.GotFocus -= OnEditBoxGotFocus;
            _editBox.LostFocus -= OnEditBoxLostFocus;
            _editBox.KeyDown -= OnEditBoxKeyDown;
        }
        if (_suggestionList is not null)
        {
            _suggestionList.SelectionChanged -= OnSuggestionSelectionChanged;
            _suggestionList.KeyDown -= OnSuggestionListKeyDown;
        }
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (SearchBoxEx)d;
        if (owner._syncingText) return;
        if (owner._editBox is not null)
        {
            owner._syncingText = true;
            try { owner._editBox.Text = (string)e.NewValue; } finally { owner._syncingText = false; }
        }
        owner.ScheduleSuggestions();
    }
    private static void OnOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((SearchBoxEx)d).UpdateVisualState();
    private static void OnSuggestionProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var owner = (SearchBoxEx)d;
        owner._requestVersion++;
        owner._requestCts?.Cancel();
        owner._requestCts?.Dispose();
        owner._requestCts = null;
        if (owner.SuggestionProvider is null)
        {
            owner._suggestions.Clear();
            owner.SearchState = SearchBoxState.Idle;
            owner.IsSuggestionListOpen = false;
            owner.SuggestionsChanged?.Invoke(owner, EventArgs.Empty);
        }
        else if (owner.Text.Trim().Length >= owner.MinimumPrefixLength)
        {
            owner.ScheduleSuggestions();
        }
    }
    private void OnEditBoxTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_syncingText)
        {
            _syncingText = true;
            try { Text = _editBox?.Text ?? string.Empty; } finally { _syncingText = false; }
        }
        ScheduleSuggestions();
    }
    private void ScheduleSuggestions()
    {
        _pendingQuery = Text.Trim();
        _debounceTimer.Stop();
        if (_pendingQuery.Length < MinimumPrefixLength)
        {
            _ = RefreshSuggestionsAsync();
            return;
        }
        _debounceTimer.Interval = Debounce;
        if (Debounce == TimeSpan.Zero) _ = RefreshSuggestionsAsync();
        else _debounceTimer.Start();
    }
    private async void OnDebounceTick(DispatcherQueueTimer sender, object args) => await RefreshSuggestionsAsync();
    private void OnEditBoxGotFocus(object sender, RoutedEventArgs e) { _hasFocus = true; IsSuggestionListOpen = _suggestions.Count > 0; }
    private void OnEditBoxLostFocus(object sender, RoutedEventArgs e) { _hasFocus = false; DispatcherQueue.TryEnqueue(() => IsSuggestionListOpen = false); }
    private void OnSuggestionSelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateVisualState();
    private async void OnSuggestionListKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter && _suggestionList?.SelectedItem is SearchSuggestion selected)
        {
            var index = _suggestions.IndexOf(selected);
            SuggestionInvoked?.Invoke(this, new SearchSuggestionInvokedEventArgs(selected, index));
            await SubmitAsync(selected);
            e.Handled = true;
        }
        else if (e.Key == Windows.System.VirtualKey.Escape) { IsSuggestionListOpen = false; _editBox?.Focus(FocusState.Programmatic); e.Handled = true; }
    }
    private async void OnEditBoxKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var selected = _suggestionList?.SelectedItem as SearchSuggestion;
            if (selected is not null) SuggestionInvoked?.Invoke(this, new SearchSuggestionInvokedEventArgs(selected, _suggestions.IndexOf(selected)));
            await SubmitAsync(selected);
            e.Handled = true;
        }
        else if (e.Key == Windows.System.VirtualKey.Down && IsSuggestionListOpen && _suggestions.Count > 0)
        {
            _suggestionList!.SelectedIndex = Math.Min(_suggestionList.SelectedIndex + 1, _suggestions.Count - 1);
            _suggestionList.Focus(FocusState.Keyboard); e.Handled = true;
        }
        else if (e.Key == Windows.System.VirtualKey.Escape) { IsSuggestionListOpen = false; e.Handled = true; }
    }
    private void OnControlKeyDown(object sender, KeyRoutedEventArgs e) { if (e.Key == Windows.System.VirtualKey.F6) FocusSearch(); }
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _isUnloaded = false;
        if (Text.Trim().Length >= MinimumPrefixLength) ScheduleSuggestions();
    }
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _isUnloaded = true;
        _debounceTimer.Stop();
        _requestVersion++;
        _requestCts?.Cancel();
        _requestCts?.Dispose();
        _requestCts = null;
        _progressRing?.SetValue(ProgressRing.IsActiveProperty, false);
        if (SearchState == SearchBoxState.Loading) SearchState = SearchBoxState.Idle;
    }
    private void UpdateVisualState()
    {
        VisualStateManager.GoToState(this, SearchState.ToString(), true);
        VisualStateManager.GoToState(this, IsSuggestionListOpen ? "SuggestionsOpen" : "SuggestionsClosed", true);
        if (_suggestionList is not null) AutomationProperties.SetLiveSetting(_suggestionList, AutomationLiveSetting.Polite);
    }
}

internal sealed class SearchBoxExAutomationPeer(SearchBoxEx owner) : FrameworkElementAutomationPeer(owner)
{
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Edit;
    protected override string GetClassNameCore() => nameof(SearchBoxEx);
    protected override string GetNameCore() => AutomationProperties.GetName(Owner) is { Length: > 0 } name ? name : "Search";
    protected override string GetHelpTextCore() => Owner is SearchBoxEx search && search.SearchState == SearchBoxState.Error ? AutomationProperties.GetHelpText(Owner) : base.GetHelpTextCore();
    protected override bool IsKeyboardFocusableCore() => Owner is SearchBoxEx search && search.IsEnabled && search.IsTabStop;
}
