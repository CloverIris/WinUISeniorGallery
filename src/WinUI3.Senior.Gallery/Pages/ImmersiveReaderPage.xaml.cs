using Microsoft.UI.Xaml;
using WinUI3.Senior.Controls;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class ImmersiveReaderPage : Microsoft.UI.Xaml.Controls.Page
{
    public ImmersiveReaderPage()
    {
        InitializeComponent();
        Reader.SetBlocks(
        [
            new ReaderBlock("heading", "A small modern reading surface", true, 1),
            new ReaderBlock("intro", "This paragraph demonstrates virtualized reader blocks and deterministic focus movement."),
            new ReaderBlock("design", "Focus mode dims surrounding paragraphs without changing the host document or creating a speech service."),
            new ReaderBlock("boundary", "The Read focused block button emits a host request. A Gallery page may accept it, defer it, or reject it."),
            new ReaderBlock("end", "All text in this laboratory is synthetic and is not loaded from a file or network source."),
        ]);
    }

    protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        Reader.Blocks.Clear();
        base.OnNavigatedFrom(e);
    }

    private void OnReadClick(object sender, RoutedEventArgs e) =>
        StatusText.Text = Reader.ToggleReading() ? "Speech request toggled for the focused block." : "No focused block is available.";

    private void OnLineFocusToggled(object sender, RoutedEventArgs e) => Reader.IsLineFocusEnabled = LineFocusSwitch.IsOn;
    private void OnScaleChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e) => Reader.FontScale = e.NewValue;
    private void OnBlockInvoked(object? sender, ReaderBlockInvokedEventArgs e) => StatusText.Text = $"Focused {e.Block.Id}; host received a user invocation.";
    private void OnSpeechRequested(object? sender, ReaderSpeechRequestedEventArgs e) { e.Handled = true; StatusText.Text = $"Synthetic speech request: {(e.IsStart ? "start" : "stop")} for {e.Block.Id}."; }
}
