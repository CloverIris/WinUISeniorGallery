# Focus Sessions 研究规格

## 历史原型结构

Windows 11 Clock 的 Focus Sessions 把专注/休息计时、每日进度、Microsoft To Do 任务与可选 Spotify 音乐整合为一个可启动的专注流程。 DurationPicker、Focus/Break cycles、TimerRing、TaskPicker、DailyProgress、AudioProvider 和 Start/Pause/End 控制构成会话面板。

## 历史状态与焦点

```text
Idle --> Preparing --> Focusing <--> Paused\nFocusing --> Break --> Focusing\nFocusing/Break --> Completing --> Completed\nAnyActive --> Ending --> Idle\nAny --> ProviderPartial/Error
```

Space/A 播放暂停，结束需确认，键盘/触摸/手柄均可；计时每分钟或阶段公告，不每秒轰炸 Narrator。

## 保留的 Design DNA

时间与任务绑定、专注/休息节奏、进度可视、外部音乐可选且不阻塞核心计时、结束提供总结。

## 现代化与废弃边界

抛弃强制 Microsoft 账号、Spotify/To Do 硬依赖、自动控制系统勿扰和云端跟踪；核心计时本地可用，Provider 显式同意。

## 现代 feature owner

Owner 登记：`experiences.focus-session`（`FocusSession`）。

FocusSession 拥有会话状态和 Provider 抽象；考古页使用虚拟时钟/假任务，不连接 Microsoft 服务。 考古依赖只能从 Gallery/Archaeology 指向 owner，稳定模块不得反向引用展项。

## Gallery 研究树

```text
ExhibitPage
|- Header (Windows 11 era, proposed/pending)
|- HistoricalStructureAndStateDiagram
|- DesignDnaAndDiscardedBoundary
|- ModernDemo (FocusSession)
|- InputAccessibilityResponsiveMatrix
`- SourcesCopyrightAndDisclaimer
```

## 失败与晋级闭锁

owner/能力/数据缺失时保留静态研究，不伪造系统行为。进入 specified 前锁定来源、owner、资产许可、差异、自动化语义和平台禁区。
