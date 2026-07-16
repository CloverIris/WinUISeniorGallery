# PivotView Integration

## Dependencies

foundation.input-system, foundation.motion-system

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 线程与生命周期
集合/依赖属性 UI 线程；卸载释放手势与未实现页。相邻页策略必须保持内存上限。
