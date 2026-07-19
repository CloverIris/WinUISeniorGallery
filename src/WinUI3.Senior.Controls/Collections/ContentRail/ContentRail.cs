using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Windows.Input;

namespace WinUI3.Senior.Controls;

/// <summary>
/// A horizontally scrolling, keyboard-friendly content rail with optional adjacent peeks.
/// The rail owns selection and focus only; item data and navigation remain host-owned.
/// </summary>
public sealed class ContentRail : ListView
{
    public static readonly DependencyProperty ItemWidthProperty = DependencyProperty.Register(
        nameof(ItemWidth), typeof(double), typeof(ContentRail), new PropertyMetadata(260d, OnLayoutPropertyChanged));
    public static readonly DependencyProperty ItemSpacingProperty = DependencyProperty.Register(
        nameof(ItemSpacing), typeof(double), typeof(ContentRail), new PropertyMetadata(12d, OnLayoutPropertyChanged));
    public static readonly DependencyProperty PeekWidthProperty = DependencyProperty.Register(
        nameof(PeekWidth), typeof(double), typeof(ContentRail), new PropertyMetadata(24d, OnLayoutPropertyChanged));
    public static readonly DependencyProperty IsSnapEnabledProperty = DependencyProperty.Register(
        nameof(IsSnapEnabled), typeof(bool), typeof(ContentRail), new PropertyMetadata(true));
    public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
        nameof(Header), typeof(object), typeof(ContentRail), new PropertyMetadata(null));
    public static readonly DependencyProperty SeeAllCommandProperty = DependencyProperty.Register(
        nameof(SeeAllCommand), typeof(ICommand), typeof(ContentRail), new PropertyMetadata(null));
    public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register(
        nameof(PageSize), typeof(int), typeof(ContentRail), new PropertyMetadata(0));
    public static readonly DependencyProperty IsWrapNavigationEnabledProperty = DependencyProperty.Register(
        nameof(IsWrapNavigationEnabled), typeof(bool), typeof(ContentRail), new PropertyMetadata(false));

    public ContentRail()
    {
        DefaultStyleKey = typeof(ContentRail);
        SelectionMode = ListViewSelectionMode.Single;
        IsItemClickEnabled = true;
        ItemClick += OnItemClick;
        KeyDown += OnKeyDown;
        ContainerContentChanging += OnContainerContentChanging;
        RegisterPropertyChangedCallback(FrameworkElement.FlowDirectionProperty, (_, _) => ApplyContainerLayout());
    }

    public double ItemWidth { get => (double)GetValue(ItemWidthProperty); set => SetValue(ItemWidthProperty, double.IsFinite(value) ? Math.Max(1, value) : 260d); }
    public double ItemSpacing { get => (double)GetValue(ItemSpacingProperty); set => SetValue(ItemSpacingProperty, double.IsFinite(value) ? Math.Max(0, value) : 12d); }
    public double PeekWidth { get => (double)GetValue(PeekWidthProperty); set => SetValue(PeekWidthProperty, double.IsFinite(value) ? Math.Max(0, value) : 24d); }
    public bool IsSnapEnabled { get => (bool)GetValue(IsSnapEnabledProperty); set => SetValue(IsSnapEnabledProperty, value); }
    public object? Header { get => GetValue(HeaderProperty); set => SetValue(HeaderProperty, value); }
    public ICommand? SeeAllCommand { get => (ICommand?)GetValue(SeeAllCommandProperty); set => SetValue(SeeAllCommandProperty, value); }
    public int PageSize { get => (int)GetValue(PageSizeProperty); set => SetValue(PageSizeProperty, Math.Max(0, value)); }
    public bool IsWrapNavigationEnabled { get => (bool)GetValue(IsWrapNavigationEnabledProperty); set => SetValue(IsWrapNavigationEnabledProperty, value); }

    public event EventHandler<ContentRailItemInvokedEventArgs>? ItemInvoked;
    public event EventHandler? SelectionChangedByUser;

    protected override AutomationPeer OnCreateAutomationPeer() => new FrameworkElementAutomationPeer(this);

    public void MoveSelection(int delta)
    {
        if (Items.Count == 0) return;
        var next = SelectedIndex < 0 ? 0 : SelectedIndex + delta;
        if (IsWrapNavigationEnabled) next = (next % Items.Count + Items.Count) % Items.Count;
        else next = Math.Clamp(next, 0, Items.Count - 1);
        SelectedIndex = next;
        ScrollIntoView(SelectedItem);
        SelectionChangedByUser?.Invoke(this, EventArgs.Empty);
    }

    public void ScrollNext() => MoveSelection(PageSize > 0 ? PageSize : 1);
    public void ScrollPrevious() => MoveSelection(-(PageSize > 0 ? PageSize : 1));

    public bool ScrollToIndex(int index)
    {
        if (index < 0 || index >= Items.Count) return false;
        SelectedIndex = index;
        ScrollIntoView(SelectedItem, ScrollIntoViewAlignment.Default);
        return true;
    }

    public bool InvokeSeeAll()
    {
        if (SeeAllCommand?.CanExecute(Header) != true) return false;
        SeeAllCommand.Execute(Header);
        return true;
    }

    private void OnItemClick(object sender, ItemClickEventArgs args)
    {
        SelectionChangedByUser?.Invoke(this, EventArgs.Empty);
        ItemInvoked?.Invoke(this, new ContentRailItemInvokedEventArgs(args.ClickedItem, Items.IndexOf(args.ClickedItem)));
    }

    private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ContentRail)d).ApplyContainerLayout();
    private void OnContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
    {
        if (!args.InRecycleQueue) ApplyContainerLayout(args.ItemIndex);
    }

    private void ApplyContainerLayout(int? index = null)
    {
        if (index is { } value)
        {
            if (ContainerFromIndex(value) is ListViewItem item)
            {
                item.Width = Math.Max(1, ItemWidth);
                var peek = Math.Max(0, PeekWidth);
                var leading = value == 0 ? peek : 0;
                var trailing = value == Items.Count - 1 ? peek : 0;
                if (FlowDirection == FlowDirection.RightToLeft) (leading, trailing) = (trailing, leading);
                item.Margin = new Thickness(leading, 0, Math.Max(0, ItemSpacing) + trailing, 0);
            }
            return;
        }
        for (var i = 0; i < Items.Count; i++) ApplyContainerLayout(i);
    }

    private void OnKeyDown(object sender, KeyRoutedEventArgs args)
    {
        switch (args.Key)
        {
            case Windows.System.VirtualKey.Left:
                MoveSelection(FlowDirection == FlowDirection.RightToLeft ? 1 : -1);
                args.Handled = true;
                break;
            case Windows.System.VirtualKey.Right:
                MoveSelection(FlowDirection == FlowDirection.RightToLeft ? -1 : 1);
                args.Handled = true;
                break;
            case Windows.System.VirtualKey.Enter:
            case Windows.System.VirtualKey.Space:
                if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
                {
                    ItemInvoked?.Invoke(this, new ContentRailItemInvokedEventArgs(SelectedItem, SelectedIndex));
                    args.Handled = true;
                }
                break;
        }
    }
}

public sealed class ContentRailItemInvokedEventArgs(object? item, int index) : EventArgs
{
    public object? Item { get; } = item;
    public int Index { get; } = index;
}
