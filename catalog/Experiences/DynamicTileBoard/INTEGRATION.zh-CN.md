# DynamicTileBoard Integration

## Dependencies

controls.dynamic-tile

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 所有权、边界与生命周期
只组合 `controls.dynamic-tile`；宿主负责数据、后台更新和持久化。页面不可注册系统磁贴或通知，卸载停止所有可见更新。
