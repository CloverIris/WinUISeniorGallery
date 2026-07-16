# ImmersiveNowPlaying Integration

## Dependencies

media.media-player-chrome, media.timed-text-view

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 所有权、边界与生命周期
只组合 MediaPlayerChrome/TimedTextView；宿主许可封面并提供主题提色服务。卸载停止视觉采样，不处理 DRM/解码。
