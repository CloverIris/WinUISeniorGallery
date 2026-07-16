using Microsoft.UI.Xaml;

namespace WinUI3.Senior.Samples.ControlsMinimal;

public partial class App : Application
{
    private Window? _window;

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = new MainWindow();
        _window.Activate();
    }
}
