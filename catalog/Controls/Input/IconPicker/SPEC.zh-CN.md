# IconPicker 规格

## 目标

提供宿主注册图标源的本地选择器，支持分类、收藏、最近使用、去抖搜索、代码点预览和提交；不扫描系统资源。

## 公共 API

`IconPickerSource` 按不区分大小写的 `Id` 去重其 `IconPickerItem`。控件提供 `SetSources`、`RegisterSource`、`SelectIcon`、`CommitSelection`、`ToggleFavorite`、`IsFavorite`、`ResolveRecent`、`RecomputeVisibleIcons`，以及 `Sources`、`Categories`、`Favorites`、`Recent`、`VisibleIcons`、`SelectedIcon`、`SearchText`、`SelectedCategory`、`IsFavoritesOnly`、`SearchDebounce=200ms`、`PickerState`。

只有 `IsAvailable && HasGlyph` 的图标可见和可提交；未知 ID 返回 `false`。选择成功会把 ID 放入最多 24 项的内存 Recent，宿主可自行持久化。

## 筛选与生命周期

筛选依次应用收藏、分类和 Id/Name/Glyph 匹配；分类从可见项发现并加入列表，但不得在过滤循环中递归重入。搜索通过 Dispatcher 去抖。`PART_SearchBox` 可选；未提供时仍可使用 API。卸载时应停止搜索计时器，迟到的搜索结果不得覆盖新查询。

## 输入与无障碍

Enter 提交当前图标，Escape 清空搜索；选中、提交和状态变化提供稳定 Automation 名称。高对比度、RTL、中文/英文和 Reduced Motion 不改变图标 ID 与提交语义。

## 当前边界

工作项为 `in-progress/lab/P2`；系统 Fluent 图标枚举、字体安装和持久化收藏另行评审。
