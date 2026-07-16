# Canvas.Native Integration

## Dependencies

future.canvas-abstractions

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 宿主、隐私与生命周期
宿主持GPU选择、线程、崩溃隔离和C++二进制许可；Native不读文件/网络/剪贴板，不记录笔迹。ABI失败安全回退CPU占位。
