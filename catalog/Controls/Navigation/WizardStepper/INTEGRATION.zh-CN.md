# WizardStepper Integration

## Dependencies

foundation.navigation-catalog

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 线程与生命周期
验证可取消并在 UI Dispatcher 提交结果；卸载取消任务，不自行持久化，宿主提供状态仓库。
