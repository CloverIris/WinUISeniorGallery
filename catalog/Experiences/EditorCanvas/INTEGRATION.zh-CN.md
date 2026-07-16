# EditorCanvas Integration

## Dependencies

future.canvas-winui

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 所有权、边界与生命周期
只依赖 Future CanvasWinUI；宿主拥有文档格式、保存、字体/图片许可。未稳定ABI前禁止实现体验，失败不得损坏原文档。
