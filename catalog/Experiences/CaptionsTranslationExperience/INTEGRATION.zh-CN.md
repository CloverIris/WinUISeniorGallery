# CaptionsTranslationExperience Integration

## Dependencies

media.timed-text-view, future.captions-abstractions

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 所有权、边界与生命周期
组合 TimedTextView 与 Future Captions；宿主选择Provider/凭据/同意/保留策略。体验不录音、不上传、不记录文本，取消传播到两Provider。
