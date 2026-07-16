# Translation Provider Integration

## Dependencies

future.captions-abstractions

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 宿主、隐私与生命周期
宿主持凭据、区域、费用、术语表、同意与数据保留；Provider不得缓存/遥测文本，日志仅ID/耗时/错误码。
