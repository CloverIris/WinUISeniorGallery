# DetachablePlayerHost Integration

## Dependencies

media.media-player-chrome, windowing.compact-overlay-host

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 所有权、边界与生命周期
组合媒体 Chrome 与 CompactOverlayHost；Windowing 拥有 AppWindow，Media 拥有 Session，体验只编排。失败回滚 Inline，卸载解除事件但不默认停止播放。
