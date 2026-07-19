# FontPicker 规格

## 目标

提供宿主注入字体清单的本地选择器：搜索、收藏、预览和选择通知由控件负责；字体枚举、安装、注册表访问和持久化由宿主负责。

## 非目标

不安装字体、不读取系统注册表、不加载网络字体、不保存用户收藏，不声明字体文件一定可用。

## 公共 API

- `SetOptions(IEnumerable<FontPickerOption>)`：按 `FamilyName`（不区分大小写）去重；替换清单后移除不存在的收藏和选择。
- `Options`、`VisibleOptions`、`FavoriteFamilies`、`SelectedOption`：只读视图。
- `SelectedFontFamily`：可绑定字符串；外部设置到当前选项时发布非用户选择事件，未知名称保留值但不发布选择事件。
- `SearchText`、`IsFavoritesOnly`、`IsPreviewEnabled`、`PreviewText`：筛选和预览状态；查询为空显示全部。
- `Select(string, bool isUserInitiated = true)`：未知名称返回 `false`；成功发布 `SelectionChanged`。
- `ToggleFavorite(string)`、`IsFavorite(string)`：只作用于当前选项；收藏变更发布 `FavoritesChanged` 和选项变更通知。

`FontPickerOption.FamilyName` 和 `DisplayName` 不为空；`Category` 可为空。所有 API 在 UI 线程调用，事件按状态提交顺序发布。

## 状态和筛选

筛选顺序是收藏过滤（若启用）再进行 FamilyName/DisplayName 的当前区域不区分大小写匹配。`SetOptions` 后立即重建 `VisibleOptions`。选中项被移除时 `SelectedFontFamily` 清空。

## 模板与降级

模板可提供搜索框、选项列表、收藏切换和预览区；缺少可选部件时仍可通过 API 使用。控件不要求字体实际存在，宿主可在 `DisplayName` 或 `Category` 标示不可用状态。

## 失败模式

空清单显示空状态；重复字体折叠为首次出现项；未知选择返回 `false`；搜索不会抛异常。控件卸载不取消宿主字体枚举，因为本控件不启动枚举任务。

## 当前边界

当前仍为 `in-progress/lab/P2`。系统字体枚举、可变轴、最近使用和字体安装能力需另行评审，不得在本控件内私自扩展。
