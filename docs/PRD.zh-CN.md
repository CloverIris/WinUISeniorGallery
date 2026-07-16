# WinUI 3 Senior Gallery 产品需求文档

**版本**：0.1（规格基线）  
**规范语言**：简体中文；英文版是同步翻译，不得反向改变 API 或产品决策。  
**产品状态**：Pre-implementation / specification-first  
**关联目录**：[Catalog](roadmap/FEATURE-CATALOG.md) · [架构说明](architecture/README.md) · [Agent 约束](../AGENTS.md)

## 1. 产品定义

WinUI 3 Senior Gallery 是两个互相支撑的产品：

1. 一组按模块发布的 WinUI 3 高级复合组件和体验组合；它们可被普通 WinUI 应用直接引用。
2. 一个 MSIX 打包的 Gallery；它既是发现与演示应用，也是组件的交互、无障碍、主题、输入和性能契约的可视化验证面。

项目把“高级”定义为：一个能力至少同时处理内容结构、状态、输入、视觉/动效、无障碍和失败降级中的多项，而不是单纯给基础控件换皮。组件必须保留宿主的业务自主权：不私自拥有网络、账户、内容版权、播放引擎、窗口创建或云服务策略。

## 2. 背景与机会

WinUI 3 Gallery 以基础控件和平台能力为主；商业套件重点通常是企业数据输入、表格和图表；Community Toolkit 则刻意保持较轻的通用边界。这使消费级媒体、内容、桌面工作台和沉浸式应用仍要重复实现：虚拟化轮播、主页内容轨道、播放器 Chrome、小窗命令、Live DVR 时间轴、歌词/字幕、拖拽面板、通知队列、可拆标签和焦点化大屏布局。

本项目不假设这些能力没有任何既有实现，而是假设其缺少一个同时满足以下条件的、面向 WinUI 3 的开源基线：

- Fluent 2 与系统主题自然融合；
- API、模板、输入、辅助功能和性能有可测试的完整契约；
- 小型应用按模块引用，不被不相关业务依赖拖累；
- 历史 Windows 体验被研究、注明来源并现代化，而不是无来源地复制样式；
- Gallery 可作为维护者、贡献者与应用开发者共同讨论的可运行产品语言。

## 3. 目标用户与待完成任务

| 用户 | 主要任务 | 成功标准 |
| --- | --- | --- |
| WinUI 应用开发者 | 在数小时内接入一个高质量媒体/内容/窗口 UI，而非数周重写基础交互。 | 只引用需要的包；可替换模板；不被绑定到某播放或数据服务。 |
| 设计师与产品负责人 | 评估高级交互的真实状态、失败与无障碍行为，而不是只看静态截图。 | Gallery 中可切换主题、输入、尺寸、RTL 和 Reduced Motion。 |
| 组件贡献者 | 领取边界清楚的功能单元并交付不漂移的实现。 | 中文规格、API、视觉树、验收与 `owned_paths` 足以独立工作。 |
| 设计史研究者 | 把经典 Windows 体验的设计价值转化为可讨论的现代模式。 | 资料可追溯；不误认原创归属；不携带受版权保护资产。 |

## 4. 愿景、原则与非目标

### 4.1 愿景

成为“现代 Windows 消费级高级交互”的可信开源基线：可直接使用、可深入定制、可无障碍访问、可长期演进。

### 4.2 产品原则

- **规格比截图更重要**：实现必须能解释状态、边界、失败、性能和可访问性。
- **原子平台能力之上建立复合能力**：复用 WinUI/Windows App SDK，而不重建平台。
- **宿主保有控制权**：媒体 UI 使用抽象播放会话；窗口 UI 发出请求或接收宿主；Provider 不在 P0 偷渡云服务。
- **每窗口隔离**：消息、焦点、生命周期和浮层不得默认跨窗口泄漏。
- **渐进稳定性**：`lab → preview → stable` 是质量承诺，不是营销标签。
- **历史可考证、现代可维护**：提取交互 DNA，不复刻专有产品外观。

### 4.3 非目标

- 不构建通用后台管理、ERP、DataGrid/报表/甘特图套件的替代品。
- 不提供账号、同步、CDN、ASR、翻译或媒体解码后端。
- 不把 Gallery 变成业务应用、内容平台或遥测收集器。
- 不承诺逐像素重制 Windows、Zune、Xbox、Office 或任何微软产品。
- 不在首轮实现 Canvas 的既有 C++ 内核；只保留可审查的抽象与集成边界。

## 5. 产品范围与模块边界

| 模块 | 责任 | 禁止责任 |
| --- | --- | --- |
| `Core` | 主题、动效、输入、无障碍、导航、本地化、资源和跨模块 Contract。 | 反向依赖 Gallery、考古、具体播放器或业务服务。 |
| `Controls` | 可复用高级控件，如 Carousel、BottomSheet、Pickers、Rails、Grid。 | 直接拥有应用导航、网络或业务数据源。 |
| `Media` | 播放器 Chrome、时间轴、Timed Text、小型播放器和媒体布局。 | 绑定唯一播放器内核；P0 直接创建/迁移窗口。 |
| `Windowing` | 标题栏、浮动宿主、边缘/设置面板、Dock 预览与窗口行为。 | 让控件隐式接管进程级窗口策略。 |
| `Archaeology` | 现代体验组合、设计考古说明与来源管理。 | 定义与 Controls/Media/Windowing 冲突的稳定 API。 |
| `Gallery` | 页面、导航、示例、配置面板和验证场景。 | 成为其他模块的依赖项或承载业务后端。 |

固定依赖图：`Controls → Core`；`Media → Core + Controls`；`Windowing → Core`；`Archaeology → Core + Controls + Media + Windowing`；`Gallery → 全部模块`。

## 6. 信息架构与 Gallery 页面要求

Gallery 是“组件目录 + 交互实验室 + 设计说明书”，不是简单的示例列表。其稳定页面路由为：

| 页面 | 路由 | 用户价值 |
| --- | --- | --- |
| HomePage | `/` | 了解项目、发现 P0、进入分类和最近状态。 |
| ControlsIndexPage | `/controls` | 按内容、导航、布局、输入、命令、覆盖层、数据分类发现控件。 |
| MediaIndexPage | `/media` | 按播放、时间轴、Timed Text、浮层和大屏媒体布局发现能力。 |
| WindowingIndexPage | `/windowing` | 了解标题栏、窗口边界、浮动宿主和 Docking。 |
| ExperiencesIndexPage | `/experiences` | 浏览由多个稳定能力组合而成的产品体验。 |
| ArchaeologyIndexPage | `/archaeology` | 查看历史来源、设计 DNA、现代化选择与版权状态。 |
| PlaygroundPage | `/playground` | 在受控安全参数范围内试验主题、尺寸、输入和组件参数。 |
| SettingsPage | `/settings` | 设置本地展示偏好，绝不作为系统设置页。 |
| AboutPage | `/about` | 版本、许可、贡献、安全与非官方声明。 |

每个页面规格必须明确：页面视觉树、区块排序、数据模型、加载/空/错误状态、深链接、返回行为、窄/中/宽布局、键盘焦点顺序、Narrator 命名、自动化测试标识、主题/RTL/文字缩放，以及页面卸载后的取消策略。

## 7. 功能需求

### FR-01 Catalog 与工作单元

- 每项能力用 `feature.json` 机器化声明 ID、状态、成熟度、优先级、包、路由、依赖、提供能力、考古引用、所有权与许可审查。
- 每项必须有中英 README、SPEC、DESIGN、INTEGRATION、ACCEPTANCE；考古项另有 SOURCES。
- `ready` 的规格不得保留 Open Decisions；`proposed/specified` 必须明确进入 `ready` 前的关闭条件。
- Catalog 校验必须检查 JSON 形状、文档存在、ID/路由/provides/owned paths 唯一、依赖可达且无循环。

### FR-02 主题、动效、输入和可访问性

- 所有正式组件支持 Light、Dark、High Contrast，使用 ThemeResource/Contract 而非硬编码颜色。
- 所有时间性动效都必须有 Reduced Motion 降级，且不能依赖动画完成才使功能可用。
- 鼠标、触摸、精确式触控板、键盘和适用时手柄拥有等价的核心操作路径。
- 可访问性基线包括 AutomationPeer、可见焦点、语义名称/值、Live Region、100/150/200% DPI、文本缩放、中文/英文和 RTL。

### FR-03 高级控件

首批目录覆盖 Carousel/Pivot/Semantic Zoom、Breadcrumb/Search/Wizard、Adaptive Grid/Content Rail/Widget/Dynamic Tile、Bottom Sheet/Snackbar/Overlay、命令带、选色/字体/图标、Property Grid 和 Tree Data Grid。每一个控件在进入 `ready` 前必须定义公共 API、状态、模板部件、数据/虚拟化策略、输入矩阵、边界异常和验收预算。

### FR-04 媒体能力

- `IMediaPlaybackSession` 是播放器 Chrome 的稳定抽象；适配 `MediaPlayerElement` 但不绑定它。
- 时间轴支持点播与 Live DVR、缓冲/禁用/章节/标记区间、预览节流、最终 Seek、Live Edge 和辅助功能 Value 模式。
- Timed Text 使用 Document → Track → Segment → Word 模型，覆盖单行字幕、滚动歌词、Karaoke、双语和 ASR 修订。
- P0 只表达全屏/浮窗请求和状态，不在 Media 中创建、移动或迁移窗口。

### FR-05 窗口与浮层能力

Windowing 负责无原生边框标题栏宿主、Compact Overlay、浮动 Widget、边缘命令面板、设置面板、Dock 预览、标签拖出、Reveal Border 和 Connected Transition。每项必须声明宿主提供的窗口句柄/生命周期、失败降级、焦点归属、跨窗口禁止规则和平台版本差异。

### FR-06 体验组合与未来占位

Home Screen、Hub Panorama、Immersive Now Playing、Widgets Board、Tabbed Shell、Focus Session、Game Bar、Immersive Reader、Editor Canvas、Captions Translation 等是体验组合，不越权定义底层稳定 API。`Captions.Abstractions`、ASR/Translation Providers 和 Canvas 抽象是明确的未来占位，默认不接入网络、云端或专有实现。

### FR-07 设计考古

每个考古展项必须记录：原始产品、版本/年代、来源 URL、访问日期、版权/资产状态、保留的设计 DNA、现代化修改、现代 feature 引用和“不做什么”。历史素材默认不入库；若未来有资产需求，必须先通过独立许可审查。

## 8. P0 交付定义

| P0 | 交付边界 | 关键质量门槛 |
| --- | --- | --- |
| CarouselView | 虚拟化、多输入、自动播放、有限/循环、三种过渡。 | 1,000 个逻辑项不全部实例化；Reduced Motion、RTL、AutomationPeer 完整。 |
| BottomSheet | 模态/非模态、SnapPoint、拖动关闭、遮罩、Esc、焦点与尺寸回退。 | 所有关闭路径一致；窄窗口底部进入，宽窗口按规格降级。 |
| SnackbarHost | 每窗口 Host/队列、优先级、去重、动作、取消和公告。 | 窗口销毁必须取消未显示项，绝不投递到另一窗口。 |
| MediaPlayerChrome | 播放/暂停/停止/跳转/音量/速度/全屏/浮窗请求。 | 只依赖播放会话抽象；窗口创建不属于 P0。 |
| MediaTimeline | VOD/Live DVR、区间和节流预览。 | 拖动仅发布节流预览，释放只提交一次 Seek；键盘立即同步 Automation 值。 |
| TimedTextView | 字幕、歌词、Karaoke、双语、ASR 增量。 | 文档模型可增量修订；不在 P0 实现 ASR/翻译 Provider。 |

具体 API、模板与验收以各目录的中文 SPEC/DESIGN/INTEGRATION/ACCEPTANCE 为准；本 PRD 不复制并覆盖它们。

## 9. 非功能需求

### 9.1 性能

- 任何可重复内容集合必须说明虚拟化/回收策略；不得以“演示数据少”为理由跳过。
- 输入响应不得被网络、解码或长动画阻塞；异步路径必须可取消。
- 视觉效果优先使用 Composition 或平台能力；不以高频 XAML 布局动画驱动大集合。
- 每个 `ready` 单元必须给出可执行的实例化、滚动、输入或更新时间预算。

### 9.2 可靠性与生命周期

- 资源、订阅和计时器必须随控件卸载/窗口销毁释放。
- 所有跨线程 UI 回调必须明确切回 UI 调度器的责任方。
- 空数据、数据替换、宿主拒绝请求、平台能力缺失和模板部件缺失必须有确定行为。

### 9.3 隐私与安全

- 默认不收集遥测、媒体内容、语音、文本或使用行为。
- ASR/翻译等外部 Provider 必须由宿主显式配置，并在将来声明网络、凭据、数据驻留和取消策略。
- 安全报告流程见 `SECURITY.md`；Gallery 不包含真实用户内容或密钥。

### 9.4 兼容性与可维护性

- 最低系统为 Windows 10 1809；对高版本 API 必须提供检查或降级。
- 公共 API 遵守语义化版本和兼容性评审；稳定后不能通过静默行为改变破坏宿主。
- 所有可见字符串资源化；稳定术语和 API 不随翻译变化。

## 10. 质量门禁与验收

一个工作单元从 `proposed` 进入 `specified`，需要完成目标、范围、依赖和初步交互；从 `specified` 进入 `ready`，需要：

1. 中文规格锁定 API、默认值、状态模型、模板/视觉树、错误/降级和性能预算。
2. 英文文档与中文标题层级、稳定 ID、API 名称和验收场景同步。
3. Theme、Motion、Input、Accessibility、Localization 和窗口/媒体 Contract 的依赖明确。
4. Given/When/Then 覆盖正常、空、错误、取消、主题、高对比度、RTL、DPI、键盘、触摸、Narrator 与 Reduced Motion。
5. `feature.json`、本地链接、所有权、依赖图和架构测试全部通过。

实现完成后，Gallery 演示、单元/架构测试、必要 UI/视觉测试和性能记录是进入 `review`/`done` 的门槛。

## 11. 成功度量

首个公开预览的成功不是功能数量，而是以下结果：

- 六个 P0 可被新建 WinUI 应用独立引用，并有最小样例和 Gallery 场景。
- P0 在基线输入/主题/无障碍矩阵下无已知阻断缺陷。
- 应用只引用所需子包，不被 Gallery 或不相关媒体/窗口模块反向拖入。
- 每个已实现公开能力能从 Gallery 跳转到规范、验收和来源（适用时）。
- 外部贡献者可以在不改变中央配置的前提下完成一个目录级工作单元。

## 12. 风险与开放的产品级议题

| 风险/议题 | 控制方式 |
| --- | --- |
| 高级组件范围膨胀 | 以 Catalog 状态和 `ready` 门禁控制；不因名称好听而提前承诺 API。 |
| Fluent/系统版本演进 | Contract 隔离平台差异；在 Gallery 保留版本与降级验证。 |
| 媒体与 ASR 依赖复杂 | 先抽象会话/Provider，P0 不内置云服务或解码器。 |
| 多窗口错误泄漏 | 每窗口 Host、显式目标、销毁取消和架构测试。 |
| 设计考古的版权与归属 | SOURCES、`license_review`、不提交原始资产、独立审查例外。 |
| 文档随实现漂移 | 中文规范源、双语结构测试、目录所有权和 Gallery 作为契约测试面。 |

## 13. 发布与治理

包策略是“元包 + 子包”：`Core`、`Controls`、`Media`、`Windowing`、`Archaeology` 和将来的 `All` 元包。`lab` 可以快速演进；`preview` 需要迁移说明；`stable` 必须按语义化版本维护兼容性。公开发布前须完成许可证、第三方 Notice、包元数据、样例、变更记录和安全检查。

项目采用 MIT License，但不授予第三方产品名称、商标或历史资产的任何额外权利。项目与 Microsoft 无关联、未获 Microsoft 背书或赞助。
