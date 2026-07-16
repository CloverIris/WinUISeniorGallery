# MiniPlayerHost 验收

## 当前门禁

保持 proposed，不实现 .cs/.xaml；SPEC 门禁完成且双语同步后才可 ready。

## Given / When / Then

Given 正在播放且焦点在暂停，When 接受 Collapse，Then 播放不中断、仅一个交互播放器且焦点到迷你暂停。Given 过渡中关闭 Window，Then 无迟到回调且内容不被 Dispose。

## 输入、焦点与生命周期

折叠后焦点到语义最接近的迷你元素，展开恢复原焦点；Esc 不隐式折叠，拖动仅在 DragRegion 生效。 覆盖关闭、卸载、取消、替换、DPI/显示器变化；关闭后无焦点陷阱、迟到事件或泄漏。

## 性能预算

Release x64 目标 60 Hz、UI 每帧小于 4 ms；不重建 MediaPlayerElement、不产生第二会话，卸载后订阅为零。

## 自动化矩阵

架构测 ID/依赖/headings；单元测状态/取消/宿主拒绝；UI 测主题、Reduced Motion、多输入、Narrator、RTL、100%–300% DPI、200% 文本。

## Ready 晋级验收

须有确定性假宿主/虚拟时钟，失败可观察，Automation/性能可重复；否则仍 proposed。

