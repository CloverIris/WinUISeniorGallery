using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using WinUI3.Senior.Media;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class TimedTextViewPage : Page
{
    private readonly DispatcherQueueTimer _clock;
    private bool _clockRunning;

    public TimedTextViewPage()
    {
        InitializeComponent();
        _clock = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _clock.Interval = TimeSpan.FromMilliseconds(100);
        _clock.Tick += OnClockTick;
        TimedText.Document = CreateDemoDocument();
        DisplayModeBox.SelectedIndex = 0;
        PositionSlider.Value = 0;
    }

    protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        _clock.Stop();
        _clockRunning = false;
        base.OnNavigatedFrom(e);
    }

    private void OnClockClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        _clockRunning = !_clockRunning;
        if (_clockRunning) _clock.Start(); else _clock.Stop();
        ClockButton.Content = _clockRunning ? "Pause clock" : "Start clock";
    }

    private void OnClockTick(DispatcherQueueTimer sender, object args)
    {
        var next = PositionSlider.Value + .1;
        PositionSlider.Value = next > PositionSlider.Maximum ? 0 : next;
    }

    private void OnDisplayModeChanged(object sender, SelectionChangedEventArgs e)
    {
        TimedText.DisplayMode = DisplayModeBox.SelectedIndex switch
        {
            1 => TimedTextDisplayMode.ScrollingLyrics,
            2 => TimedTextDisplayMode.Karaoke,
            3 => TimedTextDisplayMode.Bilingual,
            _ => TimedTextDisplayMode.SingleLine,
        };
    }

    private void OnPositionChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e) =>
        TimedText.Position = TimeSpan.FromSeconds(e.NewValue);

    private void OnActiveSegmentChanged(object? sender, TimedTextActiveSegmentChangedEventArgs e) =>
        StatusText.Text = e.Segment is null ? "No active segment." : $"Active: {e.Segment.Id} ({e.Segment.RevisionState})";

    private void OnSegmentInvoked(object? sender, TimedTextSegmentInvokedEventArgs e) =>
        StatusText.Text = $"Segment invocation requested: {e.Segment.Id}. The control did not seek.";

    private static TimedTextDocument CreateDemoDocument()
    {
        var segments = Enumerable.Range(0, 6).Select(index =>
        {
            var start = TimeSpan.FromSeconds(index * 5);
            var end = start + TimeSpan.FromSeconds(5);
            var words = new[]
            {
                new TimedTextWord($"word-{index}-1", start, start + TimeSpan.FromSeconds(2.5), "Synthetic", null, TimedTextRevisionState.Final),
                new TimedTextWord($"word-{index}-2", start + TimeSpan.FromSeconds(2.5), end, "timed text", null, TimedTextRevisionState.Final),
            };
            return new TimedTextSegment($"segment-{index}", start, end, $"Synthetic timed text line {index + 1}", null, TimedTextRevisionState.Final, words);
        }).ToArray();
        var translations = segments.Select((segment, index) => segment with
        {
            Text = $"合成时间文本第 {index + 1} 行",
            Words = Array.Empty<TimedTextWord>()
        }).ToArray();
        return new TimedTextDocument("gallery-synthetic-timed-text", 1,
        [
            new TimedTextTrack("en-US", "en-US", TimedTextTrackRole.Lyrics, segments),
            new TimedTextTrack("zh-CN", "zh-CN", TimedTextTrackRole.Translation, translations)
        ]);
    }
}
