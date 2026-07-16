# MiniPlayerHost 规范

## 目标

定义同一 XAML Window 内把完整播放器折叠为边缘迷你播放器的宿主，保持同一播放会话、焦点恢复点与内容状态。

## 宿主与窗口边界

只管理当前窗口内两个呈现槽位；不创建 AppWindow、不请求 CompactOverlay、不跨窗口重父级化 XAML。窗口化交给 CompactOverlayHost/DetachablePlayerHost，播放命令归 MediaPlayerChrome/IMediaPlaybackSession。

## 非目标

不解码媒体、不拥有播放会话生命周期、不自动选停靠边、不持久化窗口偏好。

## 候选表面与闭锁条件

候选概念为 Content、MiniContent、Mode、PreferredDock、ExpandRequested、CollapseRequested、TransitionFailed；不是公共 API。进入 ready 前冻结属性类型、取消语义、默认停靠边和模板 Contract。

## 状态图

```text
Expanded --> Collapsing --> Mini\nMini --> Expanding --> Expanded\nTransition --> Failed --> previous stable state\nAny --> Unloaded --> host content restored
```

## 模板部件与视觉树候选

候选视觉树：RootGrid 下含 ExpandedPresenter、MiniPresenter、TransitionLayer、FocusSentinel；同一时刻仅一个 Presenter 连接交互内容，另一侧为轻量占位。

## 行为与失败模式

宿主可拒绝请求；重复请求合并，反向请求从当前进度返回。关闭、卸载或会话替换取消过渡但不 Dispose 宿主对象。

## Ready 晋级门禁

冻结 API/默认值/线程、模板 Contract、转换表、销毁/取消、资源、性能和 AutomationPeer，并同步双语 API/ID 后才可 ready。

