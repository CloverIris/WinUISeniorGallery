# MediaPlayerChrome 验收

## 功能场景

- Given 空会话，When 控件加载，Then 显示禁用且不抛异常。
- Given Playing 会话，When 用户按 Space，Then 恰好调用一次 Pause；快速重复输入不会并发执行冲突命令。
- Given 宿主未处理浮窗请求，When 用户点击浮窗，Then 只发出一次 `PresentationRequested`，不创建窗口且状态不变。
- Given 宿主处理并确认全屏，When 回写 `IsFullScreen=true`，Then 图标、Automation 名称和退出命令同步变化。
- Given 会话被替换，When 旧会话继续发事件，Then 控件忽略旧事件。
- Given 不支持 Stop 或倍速，When 呈现，Then 相应入口隐藏或禁用并说明原因。

## 时间轴与状态

- VOD、Live 和 Live DVR 会话映射到正确的时间轴模式。
- 拖动期间不连续 Seek；释放只提交一次最终位置。
- Buffering 不覆盖用户当前展开的菜单；Failed 显示本地化非阻塞错误并允许重试。

## 性能

- 播放位置以每秒不超过 10 次刷新 UI。
- 空闲自动隐藏计时不产生持续布局；切换显示模式不泄漏事件或计时器。
- 反复替换会话 100 次后，旧会话订阅数归零。

## 输入与无障碍

- 验证鼠标、触摸、键盘、触控板和手柄的核心播放路径。
- Light、Dark、High Contrast、Reduced Motion、100%–300% DPI 均可操作。
- 所有按钮目标至少 44×44 epx，焦点顺序稳定，屏幕阅读器读出动作而非图标名。
- RTL 下布局镜像，媒体时间增长方向仍遵循产品本地化 Contract。

## 自动化

单元测试状态映射、命令串行化、会话注销和请求事件；UI 测试模板缺失降级、响应式模式、键盘路径及 AutomationPeer；使用假会话，不依赖网络或真实媒体。

## 实现证据

- 2026-07-16：Media Release x64、Gallery Debug/Release x64 构建成功；Media 自动化测试通过 9/9。
- Gallery 提供未打包 FileOpenPicker；取消保留会话，加载失败显示页内错误，不保存路径。
- 真实媒体、键盘/手柄和视觉矩阵待 review 手工验收，保持 `in-progress`。
