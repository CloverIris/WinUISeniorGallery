# CompactOverlayHost 集成

## 依赖与全局 Contract

依赖 contracts.windowing，遵循 Windowing、Theme、Motion、Input、Accessibility、Localization、Resources Contract；不重定义全局资源或窗口身份。

## 平台 API 与降级

使用 AppWindow.SetPresenter(CompactOverlay) 与 OverlappedPresenter 回退；不支持时拒绝请求并保持 Inline。

## 生命周期、线程与跨窗口

不创建第二窗口、不迁移 XAML 内容、不控制播放；宿主提供既有 AppWindow、处理关闭并决定请求是否允许。 所有 Window/XAML 操作在所属 DispatcherQueue；跨线程只传不可变描述。Window.Closed 后取消任务、注销事件并忽略迟到回调。

## 错误、权限与隐私

平台调用失败必须可观察并回到稳定态；默认不申请额外权限，不记录窗口标题、内容、坐标或用户输入。

## 资源

候选资源前缀 CompactOverlayHost，具体键 ready 前冻结；样式不硬编码 DPI、Caption inset、主题或系统版本。

