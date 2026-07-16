# Settings Flyout 研究规格

历史树：`AppContent → SettingsEntry → Flyout → Header/Back/SettingsList → Detail`。讲解状态：`Closed`、`Root`、`Detail`、`Dirty`、`Saving`、`SaveFailed`、`Unavailable`。它记录边缘设置层级和返回路径，不定义设置存储、验证、提交、窗口大小、`PART_*` 或公开事件。Gallery 只模拟状态：关闭/返回恢复触发焦点；保存失败显示说明；宿主、权限或数据源缺失不产生虚假成功。
