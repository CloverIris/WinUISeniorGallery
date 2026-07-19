using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using WinUI3.Senior.Core;

namespace WinUI3_Senior_Gallery.Pages;

/// <summary>Composition-level media lab; it owns only a deterministic local fake session.</summary>
public sealed partial class MediaWorkbenchPage : Page
{
    private MediaDemoSession? _session;

    public MediaWorkbenchPage()
    {
        InitializeComponent();
        ModeComboBox.SelectedIndex = 0;
        CreateSession(MediaPlaybackMode.VideoOnDemand);
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        if (_session is not null)
        {
            _session.Dispose();
            _session = null;
        }

        base.OnNavigatedFrom(e);
    }

    private void OnModeChanged(object sender, SelectionChangedEventArgs e)
    {
        var mode = ModeComboBox.SelectedIndex switch
        {
            1 => MediaPlaybackMode.Live,
            2 => MediaPlaybackMode.LiveDvr,
            _ => MediaPlaybackMode.VideoOnDemand,
        };
        ModeDescription.Text = mode switch
        {
            MediaPlaybackMode.Live => "Live：播放头跟随虚拟直播边缘，Seek 被会话拒绝。",
            MediaPlaybackMode.LiveDvr => "Live DVR：时间窗口向前滚动，允许在窗口内回看。",
            _ => "VOD：固定 4 分钟区间，支持 Seek、章节和倍速。",
        };
        CreateSession(mode);
    }

    private void CreateSession(MediaPlaybackMode mode)
    {
        _session?.Dispose();
        _session = new MediaDemoSession(mode);
        StatusText.Text = $"Session: {_session.CurrentSnapshot.SessionId.Value} · mode: {mode} · revision: {_session.CurrentSnapshot.Revision}";
    }

    private void OnChromeClick(object sender, RoutedEventArgs e) => Frame?.Navigate(typeof(MediaPlayerChromePage));
    private void OnTimelineClick(object sender, RoutedEventArgs e) => Frame?.Navigate(typeof(MediaTimelinePage));
    private void OnTimedTextClick(object sender, RoutedEventArgs e) => Frame?.Navigate(typeof(TimedTextViewPage));
}
