using System.Collections.Immutable;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3.Senior.Controls;
using WinUI3.Senior.Core;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class EditorCanvasPage : Page
{
    private readonly CanvasDocumentController _document;

    public EditorCanvasPage()
    {
        InitializeComponent();
        _document = new CanvasDocumentController(new CanvasDocumentSnapshot(
            0,
            ImmutableArray.Create(
                new CanvasObject("hero", new CanvasRect(80, 70, 260, 150), "Hero"),
                new CanvasObject("notes", new CanvasRect(390, 120, 220, 110), "Notes", ZIndex: 1),
                new CanvasObject("timeline", new CanvasRect(180, 300, 340, 90), "Timeline", ZIndex: 2)),
            ImmutableHashSet<string>.Empty));
        CanvasEditor.Document = _document;
        CanvasEditor.InkSession = new CanvasInkSession();
        CanvasEditor.ObjectInvoked += OnObjectInvoked;
        _document.Changed += OnDocumentChanged;
        UpdateStatus();
    }

    private void OnSelectClick(object sender, RoutedEventArgs e) => CanvasEditor.Tool = EditorCanvasTool.Select;
    private void OnPanClick(object sender, RoutedEventArgs e) => CanvasEditor.Tool = EditorCanvasTool.Pan;
    private void OnRectangleClick(object sender, RoutedEventArgs e) => CanvasEditor.Tool = EditorCanvasTool.Rectangle;
    private void OnTextClick(object sender, RoutedEventArgs e) => CanvasEditor.Tool = EditorCanvasTool.Text;
    private void OnInkClick(object sender, RoutedEventArgs e) => CanvasEditor.Tool = EditorCanvasTool.Ink;
    private void OnFitClick(object sender, RoutedEventArgs e) => CanvasEditor.FitToContent();
    private void OnUndoClick(object sender, RoutedEventArgs e) => _document.Undo();
    private void OnRedoClick(object sender, RoutedEventArgs e) => _document.Redo();
    private void OnGridChanged(object sender, RoutedEventArgs e) => CanvasEditor.IsGridVisible = GridToggle.IsChecked == true;
    private void OnSnapChanged(object sender, RoutedEventArgs e) => CanvasEditor.SnapToGrid = SnapToggle.IsChecked == true;

    private void OnDocumentChanged(object? sender, CanvasDocumentChangedEventArgs e) => UpdateStatus();
    private void OnObjectInvoked(object? sender, EditorCanvasObjectInvokedEventArgs e)
    {
        StatusText.Text = $"Invoked: {e.Item.Id} · world ({e.WorldPoint.X:0}, {e.WorldPoint.Y:0})";
    }

    private void UpdateStatus()
    {
        var snapshot = _document.Snapshot;
        StatusText.Text = $"objects {snapshot.Objects.Length} · selected {snapshot.SelectedIds.Count} · rev {snapshot.Revision} · zoom {CanvasEditor.Viewport.Transform.Zoom:0.00}";
    }
}
