# HomeScreen Integration

## Dependencies

controls.adaptive-grid, controls.widget-card

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 所有权、边界与生命周期
宿主提供可取消 Section Provider 与导航；页面不联网/缓存。仅组合 `controls.adaptive-grid`、`controls.widget-card`，卸载取消刷新并保留可序列化滚动锚点。
