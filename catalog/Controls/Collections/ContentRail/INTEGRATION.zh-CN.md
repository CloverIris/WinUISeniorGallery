# ContentRail Integration

## Dependencies

foundation.input-system

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 线程与生命周期
集合仅 UI 线程更新，卸载清除容器/事件。无权限；1000 项只实现视口缓冲，滚动每帧至多一次视觉更新。
