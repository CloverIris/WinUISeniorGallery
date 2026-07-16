# EdgeCommandPanel 集成

## 依赖与全局 Contract

依赖 contracts.windowing，遵循 Windowing、Theme、Motion、Input、Accessibility、Localization、Resources Contract；不重定义全局资源或窗口身份。

## 平台 API 与降级

纯 XAML/Composition 与 Pointer capture，无窗口权限；触摸/笔捕获丢失时按最近稳定态结束。

## 生命周期、线程与跨窗口

仅在宿主 XAML 根内工作，不注册全局边缘手势、不覆盖其他应用、不调用系统 Charms。宿主提供命令和打开策略。 所有 Window/XAML 操作在所属 DispatcherQueue；跨线程只传不可变描述。Window.Closed 后取消任务、注销事件并忽略迟到回调。

## 错误、权限与隐私

平台调用失败必须可观察并回到稳定态；默认不申请额外权限，不记录窗口标题、内容、坐标或用户输入。

## 资源

候选资源前缀 EdgeCommandPanel，具体键 ready 前冻结；样式不硬编码 DPI、Caption inset、主题或系统版本。

