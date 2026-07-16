# MiniPlayerHost 集成

## 依赖与全局 Contract

依赖 media.media-player-chrome，遵循 Theme、Motion、Input、Accessibility、Localization、Resources 和适用 MediaPlayback/Windowing Contract；不重定义共享键。

## 宿主与平台 API

纯 XAML/Composition、单 DispatcherQueue、无窗口/媒体权限；Window 关闭取消动画并原样返回内容所有权。

## 生命周期、线程与跨窗口

只管理当前窗口内两个呈现槽位；不创建 AppWindow、不请求 CompactOverlay、不跨窗口重父级化 XAML。窗口化交给 CompactOverlayHost/DetachablePlayerHost，播放命令归 MediaPlayerChrome/IMediaPlaybackSession。 视觉操作在所属 DispatcherQueue；后台结果带版本/取消。销毁、Window.Closed 或卸载后忽略迟到回调。

## 降级与错误

能力/部件/数据缺失时保留可访问最小路径和本地化原因；不伪报成功、不记录媒体内容/窗口标题/输入。

## 资源

候选资源前缀 MiniPlayerHost，具体键 ready 前冻结；不硬编码颜色、DPI 或窗口尺寸。

