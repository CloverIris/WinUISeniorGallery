# CarouselView 集成

## 全局 Contract

使用 Theme、Motion、Input 与 Accessibility Contract。资源键仅在 Controls 的 `Generic.xaml` 注册。Gallery、Sample 和应用宿主只能使用公开属性、事件和命令，不得访问内部实现元素或 `CarouselVirtualizingLayout`。

## 宿主与窗口边界

宿主必须在自己的 `Window.Activated`/停用路径中写入 `IsHostWindowActive`。控件不会寻找当前窗口、创建窗口、请求激活或向其他窗口投递状态。每个 `CarouselView` 拥有独立的自动播放计时器、选择状态和诊断计数；卸载时停止计时器并取消 Composition 动画。

## 数据、线程与生命周期

`ItemsSource`、集合通知、模板属性、导航和所有依赖属性均在 UI 线程调用。后台生产者必须先将变更调度到所属 `DispatcherQueue`。替换 ItemsSource 或模板后，内部 Repeater 在下一次布局中同步，数据源不被复制。`RealizedElementCount` 是 Lab 诊断快照，不是稳定布局承诺，不能作为业务逻辑依据。

## 平台、权限与降级

仅依赖 Windows App SDK XAML、ItemsRepeater、输入事件与 Composition；不需要网络、文件、媒体、位置、麦克风或任何能力声明。Composition 或动画首选项不可用时使用短淡入或无动画。窗口停用、不可见、失焦、悬停或用户交互会按规范暂停自动播放。

## 隐私与诊断

控件不收集遥测、不记录用户输入、不存储数据，也不发送网络请求。诊断仅暴露 `AutoplayPauseReason` 和 `RealizedElementCount`；宿主如需记录它们，必须自行负责用户同意与隐私策略。
