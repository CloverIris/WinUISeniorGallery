using System.Windows.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

/// <summary>Local host-owned lab for the command, icon and drag-feedback controls.</summary>
public sealed partial class ControlsWorkbenchPage : Page
{
    public ControlsWorkbenchPage()
    {
        InitializeComponent();
        BuildCommandRibbon();
        BuildIconCatalog();
        IconPickerHost.SelectionChanged += OnIconSelectionChanged;
        DragOverlayHost.StateChanged += OnDragStateChanged;
        DragOverlayHost.DropRequested += OnDropRequested;
    }

    private void BuildCommandRibbon()
    {
        var file = new CommandRibbonTab("file", "File");
        file.Groups.Add(new CommandRibbonGroup("file-actions", "Actions")
        {
            Commands =
            {
                new CommandRibbonCommand("new", "New", new DelegateCommand(() => RibbonStatus.Text = "New invoked"))
                {
                    IconGlyph = "\uE710", KeyTip = "N", Priority = 10
                },
                new CommandRibbonCommand("save", "Save", new DelegateCommand(() => RibbonStatus.Text = "Save invoked"))
                {
                    IconGlyph = "\uE74E", KeyTip = "S", Priority = 8
                }
            }
        });

        var view = new CommandRibbonTab("view", "View");
        view.Groups.Add(new CommandRibbonGroup("view-actions", "View")
        {
            Commands =
            {
                new CommandRibbonCommand("compact", "Compact", new DelegateCommand(() => RibbonStatus.Text = "Compact toggled"))
                {
                    IconGlyph = "\uE8A0", IsToggle = true, KeyTip = "C", Priority = 5
                },
                new CommandRibbonCommand("refresh", "Refresh", new DelegateCommand(() => RibbonStatus.Text = "Refresh invoked"))
                {
                    IconGlyph = "\uE72C", KeyTip = "R", Priority = 3
                }
            }
        });

        CommandRibbonHost.Tabs.Add(file);
        CommandRibbonHost.Tabs.Add(view);
        CommandRibbonHost.SelectedTab = file;
        CommandRibbonHost.RecomputeLayout(ActualWidth > 0 ? ActualWidth : 720);
        CommandRibbonHost.CommandInvoked += (_, args) =>
            RibbonStatus.Text = $"{args.Command.Label} · {(args.FromOverflow ? "overflow" : "ribbon")}";
    }

    private void BuildIconCatalog()
    {
        var source = new IconPickerSource("fluent-core", "Fluent Core", new[]
        {
            new IconPickerItem("home", "Home", "\uE80F", "Navigation"),
            new IconPickerItem("search", "Search", "\uE721", "Navigation"),
            new IconPickerItem("play", "Play", "\uE768", "Media"),
            new IconPickerItem("pause", "Pause", "\uE769", "Media"),
            new IconPickerItem("settings", "Settings", "\uE713", "System"),
            new IconPickerItem("unknown", "Unavailable", string.Empty, "System", IsAvailable: false)
        });
        IconPickerHost.SetSources(new[] { source });
    }

    private void OnIconSelectionChanged(object? sender, IconPickerSelectionChangedEventArgs args)
    {
        IconStatus.Text = $"{args.Item.Name} · {args.Item.CodePoint} · {(args.IsUserInitiated ? "user" : "programmatic")}";
    }

    private void OnShowAllowedClick(object sender, RoutedEventArgs args)
    {
        DragOverlayHost.Show("local-demo-item", "Preview", "Move item", DragOverlayOperations.Move | DragOverlayOperations.Copy, new Point(24, 220));
        DragStatus.Text = "允许 Copy/Move；点击任意位置不会接管命中测试";
    }

    private void OnShowForbiddenClick(object sender, RoutedEventArgs args)
    {
        DragOverlayHost.Show("local-demo-item", "Preview", "Drop not allowed", DragOverlayOperations.None, new Point(24, 220));
        DragStatus.Text = "当前目标拒绝拖放";
    }

    private void OnHideDragClick(object sender, RoutedEventArgs args)
    {
        DragOverlayHost.Hide();
        DragStatus.Text = "拖拽反馈已隐藏";
    }

    private void OnDragStateChanged(object? sender, DragOverlayStateChangedEventArgs args) =>
        DragStatus.Text = $"DragOverlay: {args.CurrentState}";

    private void OnDropRequested(object? sender, DragOverlayDropRequestedEventArgs args)
    {
        args.Accepted = args.Operations.HasFlag(DragOverlayOperations.Move);
        DragStatus.Text = args.Accepted ? "宿主已接受 Move" : "宿主拒绝该操作";
    }

    private sealed class DelegateCommand(Action execute) : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => execute();
    }
}
