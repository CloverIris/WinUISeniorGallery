# Semantic Zoom 研究展示验收

- Given 打开展项，When 选择概览，Then 显示分组、历史状态和 `controls.semantic-zoom-view` 链接，不调用控件 API。
- Given 位于某组，When 返回详情，Then 说明锚定目标；无映射时显示 `NoGroupMapping`，不静默跳转。
- Given 键盘、窄窗、RTL、200% DPI、High Contrast 或 Reduced Motion，When 切换层级，Then 可完成、焦点可见、文本未截断且动画非必需。

自动化断言层级名称、组标签、Esc/Back 返回、owner 链接、无网络/遥测和本地首帧；以中英/RTL、100/150/200% DPI 覆盖。仅审查通过的自制资产可发布。
