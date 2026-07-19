using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class AboutPage : Page
{
    public AboutPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        AboutStatus.Text = $"本地运行会话：{DateTime.Now:yyyy-MM-dd HH:mm} · 不发送诊断数据。";
    }
}
