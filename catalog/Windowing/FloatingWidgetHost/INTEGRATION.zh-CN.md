# FloatingWidgetHost 集成

## 依赖与全局 Contract

依赖 contracts.windowing，遵循 Windowing、Theme、Motion、Input、Accessibility、Localization、Resources Contract；不重定义全局资源或窗口身份。

## 平台 API 与降级

使用 AppWindow/Window、OverlappedPresenter、DispatcherQueue；多线程窗口仅在平台/应用模型允许时启用，否则同 UI 线程多窗口。

## 生命周期、线程与跨窗口

Host 拥有次级 AppWindow 和窗口级服务，但内容由宿主 ContentFactory 在目标 DispatcherQueue 创建；禁止把主窗口 XAML 实例重父级化。 所有 Window/XAML 操作在所属 DispatcherQueue；跨线程只传不可变描述。Window.Closed 后取消任务、注销事件并忽略迟到回调。

## 错误、权限与隐私

平台调用失败必须可观察并回到稳定态；默认不申请额外权限，不记录窗口标题、内容、坐标或用户输入。

## 资源

候选资源前缀 FloatingWidgetHost，具体键 ready 前冻结；样式不硬编码 DPI、Caption inset、主题或系统版本。

