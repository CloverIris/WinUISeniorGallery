# ASR Provider Integration

## Dependencies

future.captions-abstractions

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 宿主、隐私与生命周期
宿主持麦克风权限、密钥、区域、费用、同意UI和音频保留；Provider默认不记录音频/转写，取消后不得回调。
