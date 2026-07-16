# Canvas.WinUI Integration

## Dependencies

future.canvas-abstractions, future.canvas-native

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 宿主、隐私与生命周期
宿主持Window/DPI/Dispatcher、文档与保存；控件协调输入和表面，不持久化/上传笔迹。卸载停止渲染线程并等待释放。
