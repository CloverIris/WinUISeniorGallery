# Charms Bar 研究展示验收

## 场景

- Given Gallery 打开展项，When 用户选择“显示历史结构”，Then 显示 rail、五项语义与现代 owner 链接，且不执行系统命令。
- Given rail 已显示，When 按 Esc 或 Back，Then 先关闭 context pane，再关闭 rail，并将焦点返回触发器。
- Given 窄窗、High Contrast、RTL、200% DPI 或 Reduced Motion，When 打开展示，Then 标签可读、焦点可见、逻辑结束边正确，且没有依赖动画完成的操作。
- Given 无现代宿主或宿主拒绝，When 用户选择示例动作，Then 显示不可执行说明，不创建窗口、不注册快捷键、不泄漏到另一窗口。

## 自动化与门槛

验证研究路由、标题、owner 链接、每层 Esc 行为、Automation 可见名称和无系统 capability。以 100/150/200% DPI、Light/Dark/High Contrast、中文/英文/RTL 进行截图或 UI 自动化。页面首次本地渲染不应等待网络；只有许可审查通过的自制资产可进入发布包。
