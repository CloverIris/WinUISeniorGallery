# DynamicTile Integration

## Dependencies

foundation.motion-system

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 线程与生命周期
数据由宿主 UI 线程提供；控件不申请通知/后台权限。卸载停止计时器并释放模板引用，重载从当前面等待完整间隔。
