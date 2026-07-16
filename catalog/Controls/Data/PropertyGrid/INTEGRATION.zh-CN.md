# PropertyGrid Integration

## Dependencies

foundation.accessibility-system

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 线程与生命周期
Provider负责描述/读写/验证，UI线程提交；卸载结束编辑并解绑对象。默认不反射私有成员或记录值。
