# Canvas.Abstractions Integration

## Dependencies

foundation.input-system

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 宿主、隐私与生命周期
宿主拥有文档格式、存储、资产许可和恢复；抽象不访问文件/剪贴板/网络。Native跨ABI只传拥有权明确的不可变块。
