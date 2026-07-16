# DockLayoutPreview 集成

## 依赖与全局 Contract

依赖 contracts.windowing，遵循 Windowing、Theme、Motion、Input、Accessibility、Localization、Resources Contract；不重定义全局资源或窗口身份。

## 平台 API 与降级

XAML Drag/Pointer 与坐标转换，无系统窗口管理权限；若拖拽离开客户区则取消预览或交给专门 Windowing Host。

## 生命周期、线程与跨窗口

只布局应用内部面板，不移动 OS 窗口、不调用系统 Snap、不覆盖标题栏悬停菜单。宿主拥有拖拽对象和最终布局事务。 所有 Window/XAML 操作在所属 DispatcherQueue；跨线程只传不可变描述。Window.Closed 后取消任务、注销事件并忽略迟到回调。

## 错误、权限与隐私

平台调用失败必须可观察并回到稳定态；默认不申请额外权限，不记录窗口标题、内容、坐标或用户输入。

## 资源

候选资源前缀 DockLayoutPreview，具体键 ready 前冻结；样式不硬编码 DPI、Caption inset、主题或系统版本。

