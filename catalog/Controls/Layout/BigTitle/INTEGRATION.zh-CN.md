# BigTitle Integration

## Dependencies

foundation.theme-system

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 线程与生命周期
仅 UI 线程绑定 `ScrollViewer.ViewChanged`，卸载必须解绑；每帧最多更新一次 Composition/布局，无外部 API 或权限。
