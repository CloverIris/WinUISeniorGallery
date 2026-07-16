# Voice Access Overlay 研究规格

## 历史原型结构

Windows 11 Voice Access 提供屏幕顶部状态栏、实时命令反馈、可说内容提示，以及“显示数字/显示网格”覆盖层，让用户用语音定位并操作屏幕。 ListeningStatusBar、RecognizedCommandFeedback、NumberLabels、GridOverlay、Correction/Help 面板和麦克风状态构成系统级辅助层。

## 历史状态与焦点

```text
Off --> Starting --> Listening <--> Sleeping\nListening --> Recognizing --> CommandAccepted/CommandRejected --> Listening\nListening --> NumbersVisible/GridVisible --> Listening\nAny --> MicrophoneBlocked/Error/Off
```

模拟可由按钮/键盘触发 Listening、Numbers 和 Grid；不注册全局热键。焦点不移动到数字标签，Esc 关闭最内层覆盖并恢复触发按钮。

## 保留的 Design DNA

语音状态始终可见、命令即时反馈、目标编号把任意 UI 转成可说地址、失败可纠正、无需触摸精确定位。

## 现代化与废弃边界

Gallery 明确抛弃系统级覆盖、全局输入注入、麦克风采集、语音识别、模仿 Voice Access 品牌/安全提示和遮盖其他应用；只展示本地静态交互模型。

## 现代 feature owner

Owner 登记：未分配；本考古项不提供稳定 API。

当前 feature manifest 未分配现代 owner/public_api_name，因此展项没有 Live stable API Demo；只显示可交互研究模拟。进入 specified 前必须另立并评审通用 VoiceCommandFeedback/TargetOverlay owner。 考古依赖只能从 Gallery/Archaeology 指向 owner，稳定模块不得反向引用展项。

## Gallery 研究树

```text
ExhibitPage
|- Header (Windows 11 era, proposed/pending)
|- HistoricalStructureAndStateDiagram
|- DesignDnaAndDiscardedBoundary
|- ModernDemo (ResearchSimulation)
|- InputAccessibilityResponsiveMatrix
`- SourcesCopyrightAndDisclaimer
```

## 失败与晋级闭锁

owner/能力/数据缺失时保留静态研究，不伪造系统行为。进入 specified 前锁定来源、owner、资产许可、差异、自动化语义和平台禁区。
