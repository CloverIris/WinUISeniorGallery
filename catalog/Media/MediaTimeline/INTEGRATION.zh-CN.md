# MediaTimeline 集成

## 全局契约

采用 `contracts/MediaPlayback` 的统一时间域与播放状态语义，并使用 Theme、Motion、Input、Accessibility、Localization 和 Resources Contract。`MediaPlayerChrome` 可组合此控件，但 `MediaTimeline` 不反向依赖 Chrome。

## 数据流

控件属于调用页面的当前 XamlRoot，不创建 Window/AppWindow、不执行 Presenter 切换，也不跨窗口共享指针捕获。若同一播放会话在窗口间迁移，每个窗口创建自己的 `MediaTimeline`，宿主通过共享会话快照同步属性；关闭任一窗口只取消该窗口的拖动和订阅。

宿主从播放会话把 `Minimum`、`Maximum`、`Position`、`PlaybackRate` 和集合快照写入控件。用户输入生成节流的预览事件；释放或键盘操作生成 Seek 请求；宿主执行 Seek 后再更新 `Position`。该单向回路防止控件假报成功。倍速变更仍由播放会话或 `MediaPlayerChrome` 执行，时间轴只呈现结果。

Live DVR 更新应把范围与位置作为同一 DispatcherQueue 批次提交。`LiveWindowEndTime` 表示 `Maximum` 对应的绝对时间，其他墙钟时间按差值推导。

## 线程与生命周期

公共属性只能在 UI 线程设置；来自后台的集合必须先制作不可变快照。控件不订阅可变集合内部事件。卸载时停止节流计时和指针捕获；加载后从当前属性重建轻量视觉。

## 平台 API 与权限

控件不直接调用媒体、网络、存储或窗口 API，不需要应用权限。可使用 XAML/Composition 绘制轨道；不得依赖媒体播放器具体实现。

## 降级与本地化

未提供章节、标记或缓冲区间时相应层为空。墙钟锚点缺失时显示相对时间。时间格式由区域设置决定，超 24 小时内容不得回绕；Automation 值使用本地化文本但数值范围保持媒体秒数。
