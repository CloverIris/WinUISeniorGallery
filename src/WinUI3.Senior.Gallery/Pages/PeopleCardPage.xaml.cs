using Microsoft.UI.Xaml;
using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class PeopleCardPage : Microsoft.UI.Xaml.Controls.Page
{
    public PeopleCardPage()
    {
        InitializeComponent();
        Card.Person = new PersonCardData("person-ava", "Ava Chen", "A", new Dictionary<string, string> { ["Role"] = "Designer", ["Team"] = "Senior Gallery" }, new[] { new PeopleCardAction("message", "Message"), new PeopleCardAction("share", "Share") }, "Available");
        Card.ActionInvoked += OnActionInvoked;
        StatusText.Text = "Ready";
    }

    private void OnToggleClick(object sender, RoutedEventArgs e) => Card.ToggleExpanded();
    private void OnMessageClick(object sender, RoutedEventArgs e) => Card.InvokeAction("message");
    private void OnActionInvoked(object? sender, PeopleCardActionInvokedEventArgs e) => StatusText.Text = $"{e.Action.Label}: {e.Person.DisplayName}";
}
