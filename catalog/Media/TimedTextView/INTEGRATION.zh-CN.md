# TimedTextView 集成

## 全局契约

模型类型遵循 `contracts/TimedText`，时间位置遵循 `contracts/MediaPlayback`。控件使用 Theme、Motion、Input、Accessibility、Localization 和 Resources Contract。公共模型只有一个所有者，Media 项目不得复制 Foundation 类型。

## 数据流

控件不创建 Window/AppWindow，也不把 `ItemsRepeater`、Presenter 或已实现容器跨 XamlRoot 迁移。多窗口字幕由宿主为每个窗口创建独立 View，并共享不可变 `TimedTextDocument` 快照；每个窗口独立拥有位置、滚动、焦点和 LiveRegion 生命周期。

文件解析器或 ASR Provider 产生新的不可变 `TimedTextDocument` 快照；View 校验 Revision、归一化当前 Track 并建立 ID/时间索引；播放器位置写入 `Position`；View 只更新活动项和可见虚拟项。用户调用 Segment 时宿主决定 Seek。

## ASR 与翻译 Provider

P0 只依赖未来 `Captions.Abstractions` 的概念接口：Provider 输出标准文档快照、取消和错误状态，不直接操纵 View。Windows、Azure、本地模型或其他 Provider 放在独立包中。机器翻译与控件自身 i18n 严格分离；API Key、区域和内容策略属于宿主。

## 线程与生命周期

Provider 可在后台生成快照，但设置 `Document` 必须调度至 UI DispatcherQueue。每个快照在后台预建可选索引，在 UI 线程原子替换。卸载后取消自动滚动与待处理布局，不保留 Provider 引用；重新加载时从最新 Document/Position 恢复。

## 权限、隐私与降级

控件不请求麦克风或网络权限，也不发送遥测。Provider 必须由宿主取得同意并披露数据去向。无翻译时回退原文，无词级时间时整段高亮，无语言标签时使用 UI 方向但 Automation 报告语言未知。

## 资源与格式

可见标签、空状态、Automation 状态和时间标签全部本地化。文本内容原样显示并按 XAML 安全转义，不解释 Markdown/HTML。字体必须允许应用覆盖，禁止捆绑来源不明的字体或字幕资产。
