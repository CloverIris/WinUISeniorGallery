using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace WinUI3.Senior.Controls;

public sealed record PeopleCardAction(string Id, string Label, object? Icon = null, bool IsEnabled = true, object? Tag = null)
{
    public PeopleCardAction Normalize()
    {
        if (string.IsNullOrWhiteSpace(Id)) throw new ArgumentException("An action id is required.", nameof(Id));
        if (string.IsNullOrWhiteSpace(Label)) throw new ArgumentException("An action label is required.", nameof(Label));
        return this;
    }
}

public sealed record PersonCardData(
    string Id,
    string DisplayName,
    object? Avatar = null,
    IReadOnlyDictionary<string, string>? Fields = null,
    IReadOnlyList<PeopleCardAction>? Actions = null,
    string? Presence = null,
    object? Tag = null)
{
    public PersonCardData Normalize()
    {
        if (string.IsNullOrWhiteSpace(Id)) throw new ArgumentException("A person id is required.", nameof(Id));
        if (string.IsNullOrWhiteSpace(DisplayName)) throw new ArgumentException("A display name is required.", nameof(DisplayName));
        return this with
        {
            Fields = Fields ?? new Dictionary<string, string>(StringComparer.Ordinal),
            Actions = (Actions ?? Array.Empty<PeopleCardAction>()).Where(action => action is not null).Select(action => action.Normalize()).GroupBy(action => action.Id, StringComparer.Ordinal).Select(group => group.First()).ToArray()
        };
    }
}

public sealed class PeopleCardActionInvokedEventArgs(PersonCardData person, PeopleCardAction action) : EventArgs
{
    public PersonCardData Person { get; } = person ?? throw new ArgumentNullException(nameof(person));
    public PeopleCardAction Action { get; } = action ?? throw new ArgumentNullException(nameof(action));
}

[TemplatePart(Name = "PART_Root", Type = typeof(FrameworkElement))]
[TemplatePart(Name = "PART_Avatar", Type = typeof(ContentPresenter))]
[TemplatePart(Name = "PART_DisplayName", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_Presence", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_Fields", Type = typeof(ItemsControl))]
[TemplatePart(Name = "PART_Actions", Type = typeof(ItemsControl))]
public sealed class PeopleCard : Control
{
    private FrameworkElement? _root;
    private ContentPresenter? _avatar;
    private TextBlock? _displayName;
    private TextBlock? _presence;
    private ItemsControl? _fields;
    private ItemsControl? _actions;
    private readonly ObservableCollection<KeyValuePair<string, string>> _fieldItems = new();
    private readonly ObservableCollection<PeopleCardAction> _actionItems = new();

    public static readonly DependencyProperty PersonProperty = DependencyProperty.Register(nameof(Person), typeof(PersonCardData), typeof(PeopleCard), new PropertyMetadata(null, OnPersonChanged));
    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(PeopleCard), new PropertyMetadata(true, OnVisualPropertyChanged));
    public static readonly DependencyProperty IsActionsVisibleProperty = DependencyProperty.Register(nameof(IsActionsVisible), typeof(bool), typeof(PeopleCard), new PropertyMetadata(true, OnVisualPropertyChanged));

    public PeopleCard() { DefaultStyleKey = typeof(PeopleCard); }
    public PersonCardData? Person { get => (PersonCardData?)GetValue(PersonProperty); set => SetValue(PersonProperty, value); }
    public bool IsExpanded { get => (bool)GetValue(IsExpandedProperty); set => SetValue(IsExpandedProperty, value); }
    public bool IsActionsVisible { get => (bool)GetValue(IsActionsVisibleProperty); set => SetValue(IsActionsVisibleProperty, value); }
    public event EventHandler<PeopleCardActionInvokedEventArgs>? ActionInvoked;
    public event EventHandler? Closed;
    public event EventHandler? Expanded;

    public bool InvokeAction(string id)
    {
        var person = Person;
        var action = person?.Actions?.FirstOrDefault(candidate => string.Equals(candidate.Id, id, StringComparison.Ordinal));
        if (person is null || action is null || !action.IsEnabled) return false;
        ActionInvoked?.Invoke(this, new PeopleCardActionInvokedEventArgs(person, action));
        return true;
    }

    public void ToggleExpanded()
    {
        IsExpanded = !IsExpanded;
        if (IsExpanded) Expanded?.Invoke(this, EventArgs.Empty); else Closed?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnApplyTemplate()
    {
        if (_actions is not null) _actions.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnActionClick));
        base.OnApplyTemplate();
        _root = GetTemplateChild("PART_Root") as FrameworkElement;
        _avatar = GetTemplateChild("PART_Avatar") as ContentPresenter;
        _displayName = GetTemplateChild("PART_DisplayName") as TextBlock;
        _presence = GetTemplateChild("PART_Presence") as TextBlock;
        _fields = GetTemplateChild("PART_Fields") as ItemsControl;
        _actions = GetTemplateChild("PART_Actions") as ItemsControl;
        if (_actions is not null) _actions.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnActionClick), true);
        UpdateTemplate();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    private static void OnPersonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PeopleCard)d).UpdateTemplate();
    private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((PeopleCard)d).UpdateTemplate();

    private void OnActionClick(object sender, RoutedEventArgs e)
    {
        if (e.OriginalSource is Button { Tag: string id }) { InvokeAction(id); e.Handled = true; }
    }

    private void UpdateTemplate()
    {
        var person = Person?.Normalize();
        if (_root is not null) AutomationProperties.SetName(_root, person is null ? "Person card" : $"Person {person.DisplayName}");
        if (_avatar is not null) _avatar.Content = person?.Avatar;
        if (_displayName is not null) _displayName.Text = person?.DisplayName ?? string.Empty;
        if (_presence is not null) { _presence.Text = person?.Presence ?? string.Empty; _presence.Visibility = string.IsNullOrWhiteSpace(person?.Presence) ? Visibility.Collapsed : Visibility.Visible; }
        _fieldItems.Clear();
        foreach (var field in person?.Fields ?? new Dictionary<string, string>()) _fieldItems.Add(field);
        if (_fields is not null) { _fields.ItemsSource = _fieldItems; _fields.Visibility = IsExpanded ? Visibility.Visible : Visibility.Collapsed; }
        _actionItems.Clear();
        foreach (var action in person?.Actions ?? Array.Empty<PeopleCardAction>()) _actionItems.Add(action);
        if (_actions is not null) { _actions.ItemsSource = _actionItems; _actions.Visibility = IsActionsVisible && _actionItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed; }
    }
}
