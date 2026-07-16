# MediaCenterExperience Integration

## Dependencies

media.media-center-grid

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 所有权、边界与生命周期
组合 `media.media-center-grid`；宿主提供目录/播放路由，体验不扫描媒体库或管理遥控驱动。卸载取消分页。
