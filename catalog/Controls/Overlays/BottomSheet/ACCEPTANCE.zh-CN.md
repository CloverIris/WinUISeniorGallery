# BottomSheet 验收

## 功能场景

- Given 320px 窗口和 Auto，When 打开，Then 从底部进入且不越过安全区域；Given 1000px，Then 使用 Side。
- Given 25%、50%、Content 三个点，When 慢速释放，Then 落到最近点；When 速度超过 800px/s，Then 落到方向相邻点且不跳点。
- Given 位于最小点，When 向关闭方向拖过 25% 或超过 1000px/s，Then Closing 原因是 Drag。
- Given Modal，When 打开、Tab/Shift+Tab、Esc、关闭，Then 焦点在 Sheet 内循环并最终恢复，背景不可由 UIA 操作。
- Given Modeless，When 打开并操作背景，Then 背景仍可用且焦点不被强制收回。
- Given Opening 中调用 Close，Then 从当前帧反向，恰好触发一次 Closed，不触发 Opened。
- Given Host 卸载，Then 以 HostUnloaded 强制关闭，释放捕获、事件和 Composition 对象。

## 质量矩阵

- 验证鼠标拖动、触摸、触控板、键盘、Narrator；Light/Dark/High Contrast、RTL、Reduced Motion、100/200% DPI。
- 测试软键盘出现、窗口跨过 720px、极小窗口、内容动态增高和 SnapPoints 运行时替换。
- 指针移动期间每帧最多一次布局/视觉更新；Release x64 95 百分位帧耗时不超过 25ms。
- 单元测试覆盖参数验证、取消 Opening/Closing、事件顺序、布局钳制和速度阈值；UI 测试覆盖焦点陷阱、Z 顺序与 Esc 顶层规则。
