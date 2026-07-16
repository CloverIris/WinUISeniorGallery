# FocusSession Integration

## Dependencies

controls.snackbar-host

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 所有权、边界与生命周期
组合 SnackbarHost；宿主提供任务和可选勿扰Provider，体验无系统设置权限/后台保证。挂起恢复按时间策略。
