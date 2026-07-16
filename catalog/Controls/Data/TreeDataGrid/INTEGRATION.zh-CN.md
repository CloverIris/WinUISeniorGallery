# TreeDataGrid Integration

## Dependencies

foundation.accessibility-system

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 线程与生命周期
子项加载可取消并按节点版本提交；卸载取消/释放容器。UI线程集合，Provider可后台；不持久化编辑。
