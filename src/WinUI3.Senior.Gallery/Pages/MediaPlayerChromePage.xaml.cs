using Microsoft.UI.Xaml.Controls;
using Windows.Media.Core;
using Windows.Storage.Pickers;
using WinUI3.Senior.Media;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class MediaPlayerChromePage : Page
{
    private MediaPlayerElementPlaybackSessionAdapter? _adapter;

    public MediaPlayerChromePage() => InitializeComponent();

    protected override void OnNavigatedFrom(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        ReleaseAdapter();
        base.OnNavigatedFrom(e);
    }

    private async void OnOpenLocalMediaClick(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (App.MainWindow is null)
        {
            StatusText.Text = "The host window is unavailable.";
            return;
        }

        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".mp4");
        picker.FileTypeFilter.Add(".wmv");
        picker.FileTypeFilter.Add(".mp3");
        picker.FileTypeFilter.Add(".m4a");
        picker.FileTypeFilter.Add(".wav");
        WinRT.Interop.InitializeWithWindow.Initialize(picker, App.MainWindow.WindowHandle);
        Windows.Storage.StorageFile? file;
        try
        {
            file = await picker.PickSingleFileAsync();
        }
        catch (Exception)
        {
            StatusText.Text = "The file picker could not be opened. The current session was kept.";
            return;
        }
        if (file is null)
        {
            StatusText.Text = "Selection cancelled; the current session was kept.";
            return;
        }

        try
        {
            ReleaseAdapter();
            PlayerElement.Source = MediaSource.CreateFromStorageFile(file);
            _adapter = new MediaPlayerElementPlaybackSessionAdapter(PlayerElement);
            Chrome.PlaybackSession = _adapter;
            StatusText.Text = $"Loaded {file.Name}. Playback support depends on installed Windows codecs.";
        }
        catch
        {
            ReleaseAdapter();
            StatusText.Text = "Windows could not load this media file. Choose a supported local format.";
        }
    }

    private void OnPresentationRequested(object sender, MediaChromePresentationRequestedEventArgs e) =>
        StatusText.Text = $"Requested {e.RequestedMode}. This P0 lab intentionally does not create or move windows.";

    private void OnCommandCompleted(object? sender, MediaChromeCommandCompletedEventArgs e) =>
        StatusText.Text = $"Playback command: {e.Result.Status} ({e.Result.ErrorCode}).";

    private void ReleaseAdapter()
    {
        Chrome.PlaybackSession = null;
        _adapter?.Dispose();
        _adapter = null;
        // Source is host-owned for the duration of this page only. Clearing it before
        // navigation prevents a late MediaOpened/MediaFailed event from touching a new page.
        PlayerElement.Source = null;
    }
}
