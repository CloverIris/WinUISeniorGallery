using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinUI3_Senior_Gallery.Models;

namespace WinUI3_Senior_Gallery.Pages;

public sealed partial class SettingsPage : Page
{
    private GalleryPreference _draft = GalleryData.Preferences.Clone();
    private bool _suppressChanges;

    public SettingsPage()
    {
        InitializeComponent();
        LoadDraft(_draft);
    }

    private void LoadDraft(GalleryPreference draft)
    {
        _suppressChanges = true;
        ReducedMotionSwitch.IsOn = draft.ReducedMotion;
        DiagnosticsSwitch.IsOn = draft.ShowDiagnostics;
        RememberRouteSwitch.IsOn = draft.RememberLastRoute;
        DensityComboBox.SelectedIndex = draft.Density switch
        {
            "Compact" => 1,
            "Comfortable" => 2,
            _ => 0,
        };
        LanguageComboBox.SelectedIndex = draft.Language.StartsWith("English", StringComparison.Ordinal) ? 1 : 0;
        DirtyText.Text = "未修改";
        _suppressChanges = false;
    }

    private void OnPreferenceChanged(object sender, RoutedEventArgs e)
    {
        if (_suppressChanges)
        {
            return;
        }

        _draft = ReadDraftFromControls();
        DirtyText.Text = "有未应用的修改";
        SettingsInfoBar.Severity = InfoBarSeverity.Warning;
        SettingsInfoBar.Title = "设置尚未应用";
        SettingsInfoBar.Message = "这些值仅保存在页面状态中；点击应用后才会同步到当前 Gallery 会话。";
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) =>
        OnPreferenceChanged(sender, e);

    private void OnApplyClick(object sender, RoutedEventArgs e)
    {
        _draft = ReadDraftFromControls();
        Copy(_draft, GalleryData.Preferences);
        DirtyText.Text = "已应用到当前会话";
        SettingsInfoBar.Severity = InfoBarSeverity.Success;
        SettingsInfoBar.Title = "设置已应用";
        SettingsInfoBar.Message = "没有写入磁盘或系统配置。";
    }

    private void OnResetClick(object sender, RoutedEventArgs e)
    {
        _draft = new GalleryPreference();
        LoadDraft(_draft);
        SettingsInfoBar.Severity = InfoBarSeverity.Informational;
        SettingsInfoBar.Title = "已恢复默认";
        SettingsInfoBar.Message = "点击应用后，默认值会覆盖当前 Gallery 会话设置。";
    }

    private GalleryPreference ReadDraftFromControls() => new()
    {
        ReducedMotion = ReducedMotionSwitch.IsOn,
        ShowDiagnostics = DiagnosticsSwitch.IsOn,
        RememberLastRoute = RememberRouteSwitch.IsOn,
        Density = (DensityComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Default",
        Language = (LanguageComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "中文（规范源）",
    };

    private static void Copy(GalleryPreference source, GalleryPreference target)
    {
        target.ReducedMotion = source.ReducedMotion;
        target.ShowDiagnostics = source.ShowDiagnostics;
        target.RememberLastRoute = source.RememberLastRoute;
        target.Density = source.Density;
        target.Language = source.Language;
    }
}
