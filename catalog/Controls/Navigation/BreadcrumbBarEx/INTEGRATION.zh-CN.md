# BreadcrumbBarEx Integration

## Dependencies

foundation.navigation-catalog

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 线程与生命周期
UI 线程解析/集合；建议异步且可取消，卸载取消查询与拖放。无默认文件权限，宿主提供解析器。
