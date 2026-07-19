# MediaPlayback 集成

## Contract 与依赖

代码位于 `WinUI3.Senior.Core`，Media、Gallery 和测试仅引用 `IMediaPlaybackSession`。`MediaPlayerElementPlaybackSessionAdapter` 属于 MediaPlayerChrome 所有权，向此 Contract 投影而不是反向把 XAML 类型带入 Core。

## 线程与生命周期

Provider 在所属 UI Dispatcher 读取和修改 UI/引擎对象；后台原生回调先检查 generation，再有序发布快照。会话替换、卸载或窗口关闭后取消命令、解除订阅并忽略迟到事件。

## 隐私与诊断

Contract 不接收媒体源、路径或用户文本。稳定错误码可进入本地诊断，关联 ID 不得包含用户内容；网络、遥测和持久化均不属于 P0。
