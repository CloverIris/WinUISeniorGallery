using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Navigation;
using WinUI3_Senior_Gallery.Models;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class PlaygroundPage : Page
{
    private readonly DispatcherQueueTimer _timer;
    private int _ticks;
    private bool _isRunning;

    public IReadOnlyList<PlaygroundScenario> Scenarios => GalleryData.Scenarios;

    public PlaygroundPage()
    {
        InitializeComponent();
        DataContext = this;
        _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(250);
        _timer.Tick += OnTimerTick;
        ScenarioComboBox.SelectedIndex = 0;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        _timer.Stop();
        _isRunning = false;
        base.OnNavigatedFrom(e);
    }

    private void OnScenarioChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ScenarioComboBox.SelectedItem is not PlaygroundScenario scenario)
        {
            return;
        }

        ScenarioDescription.Text = scenario.Description;
        ScenarioContract.Text = $"Contract: {scenario.Contract} · status: {scenario.Status}";
    }

    private void OnRunClick(object sender, RoutedEventArgs e)
    {
        if (ScenarioComboBox.SelectedItem is not PlaygroundScenario scenario)
        {
            return;
        }

        _ticks = 0;
        RunProgress.Value = 0;
        _isRunning = true;
        StateText.Text = $"Running · {scenario.Name}";
        StatusBar.Severity = InfoBarSeverity.Success;
        StatusBar.Title = "场景已启动";
        StatusBar.Message = "计时器仅用于模拟状态推进；不代表正式组件的性能结论。";
        _timer.Start();
    }

    private void OnResetClick(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        _isRunning = false;
        _ticks = 0;
        RunProgress.Value = 0;
        StateText.Text = "Idle";
        TickText.Text = "ticks: 0";
        StatusBar.Severity = InfoBarSeverity.Informational;
        StatusBar.Title = "已重置";
        StatusBar.Message = "选择一个场景后，运行结果会出现在下方。";
    }

    private void OnTimerTick(DispatcherQueueTimer sender, object args)
    {
        if (!_isRunning)
        {
            return;
        }

        _ticks++;
        RunProgress.Value = Math.Min(100, _ticks * 4);
        TickText.Text = $"ticks: {_ticks}";
        if (_ticks >= 25)
        {
            _timer.Stop();
            _isRunning = false;
            StateText.Text = "Completed · deterministic run";
            StatusBar.Title = "场景完成";
            StatusBar.Message = "可以再次运行或切换到另一个本地场景。";
        }
    }

    private void OnSpeedChanged(object sender, RangeBaseValueChangedEventArgs e)
    {
        var multiplier = Math.Max(0.05, e.NewValue);
        SpeedLabel.Text = $"{multiplier:0.00}×";
        _timer.Interval = TimeSpan.FromMilliseconds(250 / multiplier);
    }

    private void OnAutoRefreshToggled(object sender, RoutedEventArgs e)
    {
        StatusBar.Message = AutoRefreshSwitch.IsOn
            ? "自动刷新诊断已开启（本地计时器）。"
            : "自动刷新诊断已关闭。";
    }

    private void OnReducedMotionToggled(object sender, RoutedEventArgs e)
    {
        StatusBar.Message = ReducedMotionSwitch.IsOn
            ? "Reduced Motion：转场应降级为即时或短淡入。"
            : "完整动效已启用（仍不影响任务完成）。";
    }
}
