using WinUI3.Senior.Media;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class CaptionsTranslationPage : Microsoft.UI.Xaml.Controls.Page
{
    public CaptionsTranslationPage()
    {
        InitializeComponent();
        ModeBox.SelectedIndex = 3;
        Apply(1);
    }

    private void OnRevisionOneClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Apply(1);
    private void OnRevisionTwoClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Apply(2);
    private void OnStaleClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e) => Apply(1);
    private void OnModeChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e) => Experience.DisplayMode = ModeBox.SelectedIndex switch { 1 => TimedTextDisplayMode.ScrollingLyrics, 2 => TimedTextDisplayMode.Karaoke, 3 => TimedTextDisplayMode.Bilingual, _ => TimedTextDisplayMode.SingleLine };
    private void OnRevisionApplied(object? sender, CaptionsTranslationRevisionAppliedEventArgs e) => StatusText.Text = e.Accepted ? $"Accepted revision {e.Revision.Revision}" : $"Ignored stale revision {e.Revision.Revision}";
    private void OnStateChanged(object? sender, EventArgs e) { if (Experience.CurrentRevision is not null) StatusText.Text = $"{Experience.State} · revision {Experience.Revision}"; }
    private void Apply(long revision)
    {
        var source = CreateDocument("en-US", TimedTextTrackRole.Captions, revision, "Synthetic caption");
        var translation = CreateDocument("zh-CN", TimedTextTrackRole.Translation, revision, "合成字幕");
        Experience.ApplyRevision(new CaptionsTranslationRevision(revision, source, translation, revision >= 2));
    }
    private static TimedTextDocument CreateDocument(string language, TimedTextTrackRole role, long revision, string text) => new($"synthetic-{language}", revision, [new TimedTextTrack(language, language, role, [new TimedTextSegment($"segment-{language}", TimeSpan.Zero, TimeSpan.FromSeconds(5), text, null, revision >= 2 ? TimedTextRevisionState.Final : TimedTextRevisionState.Interim, [])])]);
}
