# IconPicker Integration

## Dependencies

foundation.resource-catalog

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 线程与生命周期
Provider异步可取消，UI线程提交；不扫描任意文件。卸载释放缩略图缓存，收藏由宿主持久化。
