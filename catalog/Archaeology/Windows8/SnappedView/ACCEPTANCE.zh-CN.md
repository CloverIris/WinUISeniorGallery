# Snapped View 研究展示验收

- Given 打开展项，When 选择历史布局，Then 显示主区、Snap 区、状态说明与 `windowing.dock-layout-preview` 链接，且没有窗口操作。
- Given 模拟预览，When Esc、Back 或取消，Then 返回 `Single` 并把焦点交还触发器。
- Given 窄窗/200% DPI/RTL/High Contrast/Reduced Motion，When 改变布局，Then 无截断、方向正确、可见焦点和即时可完成任务。
- Given 无宿主或能力，When 选择“应用示例”，Then 明确降级且不写入布局、不调用平台 API。

自动化检查路由、所有状态文本、键盘序、accessible name、无窗口调用；以中英/RTL 和三档 DPI 截图。发布只允许经审查的自制资产。
