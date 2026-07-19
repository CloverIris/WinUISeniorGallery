# FontPicker 验收

## 当前门禁

工作项为 `in-progress / lab / P2`；需完成模板、输入和无障碍验证后才可进入 `review`。

## Given/When/Then

- Given 注入 1000 个含重复 FamilyName 的选项，When 调用 `SetOptions`，Then `Options` 去重且 `VisibleOptions` 与搜索结果顺序稳定。
- Given `IsFavoritesOnly=true`，When 收藏或取消收藏一个选项，Then `FavoritesChanged`、`OptionsChanged` 和 `VisibleOptions` 同步更新。
- Given 选中项被新清单移除，When `SetOptions` 完成，Then `SelectedFontFamily` 清空且不抛异常。
- Given 外部设置未知 FamilyName，When 属性变更，Then 保留字符串供宿主显示但不发布选择事件。

## 体验矩阵

Light、Dark、High Contrast、100%/150%/200% DPI；键盘、鼠标、触摸、Narrator、Reduced Motion；中文、英文和 RTL。搜索框、列表和收藏按钮具有可见焦点与稳定 Automation 名称。

## 性能与失败

1000 个选项重建筛选视图的 UI 工作应保持在一次 Dispatcher 批次内；空清单显示 Empty；未知选择返回 `false`；字体是否安装由宿主标记，不读取注册表。
