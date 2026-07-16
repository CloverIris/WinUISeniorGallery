# MediaCenterGrid 集成

## 依赖与全局 Contract

依赖 controls.content-rail，遵循 Theme、Motion、Input、Accessibility、Localization、Resources 和适用 MediaPlayback/Windowing Contract；不重定义共享键。

## 宿主与平台 API

ItemsRepeater、ScrollViewer、Composition；无窗口/网络/媒体权限。图片 Provider 后台解码并在 UI DispatcherQueue 提交版本化结果。

## 生命周期、线程与跨窗口

保留在当前页面视觉树，不创建窗口、不全屏、不播放。宿主提供 ItemsSource、模板、导航和详情；窗口、播放与后台加载归外部服务。 视觉操作在所属 DispatcherQueue；后台结果带版本/取消。销毁、Window.Closed 或卸载后忽略迟到回调。

## 降级与错误

能力/部件/数据缺失时保留可访问最小路径和本地化原因；不伪报成功、不记录媒体内容/窗口标题/输入。

## 资源

候选资源前缀 MediaCenterGrid，具体键 ready 前冻结；不硬编码颜色、DPI 或窗口尺寸。

