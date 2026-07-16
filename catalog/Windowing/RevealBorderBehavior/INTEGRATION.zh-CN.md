# RevealBorderBehavior 集成

## 依赖与全局 Contract

依赖 contracts.motion，遵循 Windowing、Theme、Motion、Input、Accessibility、Localization、Resources Contract；不重定义全局资源或窗口身份。

## 平台 API 与降级

使用 Microsoft.UI.Composition/XamlCompositionBrushBase 或兼容回退；不支持效果、远程桌面或省电策略时使用静态 ThemeResource 边框。

## 生命周期、线程与跨窗口

只附加到当前 Window 的 XAML 元素，不创建灯光窗口、不全局追踪指针、不改变命中测试或命令。 所有 Window/XAML 操作在所属 DispatcherQueue；跨线程只传不可变描述。Window.Closed 后取消任务、注销事件并忽略迟到回调。

## 错误、权限与隐私

平台调用失败必须可观察并回到稳定态；默认不申请额外权限，不记录窗口标题、内容、坐标或用户输入。

## 资源

候选资源前缀 RevealBorderBehavior，具体键 ready 前冻结；样式不硬编码 DPI、Caption inset、主题或系统版本。

