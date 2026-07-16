# AdaptiveGrid Integration

## Dependencies

foundation.accessibility-system

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 线程与生命周期
依赖属性和集合通知须在 UI 线程；卸载时清除实现元素缓存。无权限/API；轻量模板下 10,000 项只能实现视口及前后缓冲行。
