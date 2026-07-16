# WinUI 3 Senior Gallery

> 面向现代 Windows 消费级应用的高级 WinUI 3 组件库、体验组合库与 Windows 设计考古项目。

[English](README.en-US.md) · [产品需求文档（中文规范源）](docs/PRD.zh-CN.md) · [Feature Catalog](docs/roadmap/FEATURE-CATALOG.md) · [贡献指南](CONTRIBUTING.md)

## 项目要解决什么

WinUI 3 提供可靠的原子控件与 Fluent 基础能力，但一个真正的媒体、内容或沉浸式 Windows 应用仍要反复自行拼装轮播、首页内容流、浮动播放条、时间轴、字幕、窗口标题栏、通知队列与大屏焦点体验。本项目将这些反复出现、跨产品可复用、且需要严谨交互边界的“高级复合组件”沉淀为可按需引用的 NuGet 模块，并用 Gallery 公开展示、比较和验证它们。

它不是企业 LOB 控件库的替代品，也不复刻 Microsoft 产品。设计考古的目标是识别历史 Windows 产品中仍有价值的交互 DNA，在现代 Fluent、可访问性、触摸/鼠标/键盘/手柄和多窗口约束下重新设计。

## 当前阶段

仓库处于“规格先行”的骨架阶段。`catalog/` 的每个目录都是一个可独立领取的工作单元；`feature.json` 是机器真相；中文文档是规范源；只有状态为 `ready` 的单元可以进入编码。

第一批可实现的 P0：

- `controls.carousel-view`：虚拟化轮播，含 Slide、Fade、CoverFlow、自动播放和全输入支持。
- `controls.bottom-sheet`：模态/非模态、SnapPoint、拖拽、焦点约束和窗口尺寸回退。
- `controls.snackbar-host`：严格按窗口隔离的消息队列与服务接口。
- `media.media-player-chrome`：与具体播放内核解耦的播放器控制层。
- `media.media-timeline`：点播、Live DVR、缓冲区间、章节与可访问时间轴。
- `media.timed-text-view`：歌词、字幕、Karaoke、双语与 ASR 增量修订显示。

其余单元为 `proposed` 或 `specified`：它们是受控的产品 backlog，不等于已设计完成，更不等于已实现。

## 产品形态

```text
NuGet packages                         MSIX Gallery
──────────────────────────────────     ──────────────────────────────────
WinUI3.Senior.Core                     WinUI 3 Senior Gallery
WinUI3.Senior.Controls                 ├─ 首页与功能索引
WinUI3.Senior.Media                    ├─ 单组件演示与 Playground
WinUI3.Senior.Windowing                ├─ 主题/无障碍/输入矩阵
WinUI3.Senior.Archaeology              └─ 设计考古与现代化说明
```

固定依赖方向：

```text
Core
├─ Controls → Core
├─ Media → Core + Controls
├─ Windowing → Core
└─ Archaeology → Core + Controls + Media + Windowing

Gallery → 全部模块
```

`Core` 不反向依赖 Gallery、Archaeology、具体播放引擎或业务服务；P0 的 Media 不依赖 Windowing。这样应用可只引用媒体 UI，不被多窗口能力拖入依赖图。

## 仓库导航

| 路径 | 职责 |
| --- | --- |
| `catalog/` | 功能、体验、Gallery 页面和考古展项的可领取规格单元。 |
| `contracts/` | 跨模块主题、动效、输入、媒体、窗口、本地化与资源契约。 |
| `src/` | 将来承载 NuGet 模块和最小 MSIX Gallery；当前没有正式控件实现。 |
| `tests/` | 架构门禁、核心/控件/媒体/窗口测试以及将来的 UI/视觉测试。 |
| `eng/` | JSON Schema、模板、Catalog/主题索引生成与验证脚本。 |
| `docs/` | PRD、架构、治理、路线图和考古说明。 |
| `samples/` | 未来最小消费者示例的预留目录。 |

每个功能目录必须包含：

```text
feature.json
README.zh-CN.md / README.en-US.md
SPEC.zh-CN.md / SPEC.en-US.md
DESIGN.zh-CN.md / DESIGN.en-US.md
INTEGRATION.zh-CN.md / INTEGRATION.en-US.md
ACCEPTANCE.zh-CN.md / ACCEPTANCE.en-US.md
```

考古展项额外拥有 `SOURCES.zh-CN.md` 与 `SOURCES.en-US.md`。它们只拥有历史研究和现代化说明，不能抢占现代控件的稳定 API 所有权。

## 规格与协作规则

1. 先读本地中文规格、`feature.json` 与所引用 Contract，再修改任何文件。
2. 一位 agent 一次领取一个完整功能目录；不得把同一工作单元的中英文文档拆给不同 agent。
3. `owned_paths` 是修改边界。公共 Contract、项目依赖、中央构建配置和生成索引由根维护者控制。
4. 稳定 ID、公开 API、Resource Key、路由、AutomationId 与测试 ID 始终使用英文。
5. `ready` 之前必须锁定 API、状态模型、模板部件、默认值、失败/降级、性能预算与验收；发现缺失公共契约时应改为 `blocked`，不能局部发明替代 API。
6. 每次交付必须同步更新验收条件与状态，并通过 Catalog 校验。

完整约束见 [AGENTS.md](AGENTS.md)。

## 工程基线

- C#、.NET 10 LTS（SDK 由 `global.json` 锁定）。
- Windows App SDK 2.2，采用中央包管理。
- Windows 10 1809（`10.0.17763.0`）为最低目标系统。
- 声明 x86、x64、ARM64；首轮本机构建和 Gallery 验证使用 x64。
- Gallery 是最小 MSIX Packaged WinUI 应用；它展示组件，不承载业务后端。
- 所有颜色与动效必须通过 Theme/Motion Contract 进入，不硬编码品牌色或系统主题色。
- 默认覆盖 Light、Dark、High Contrast、100/150/200% DPI、键盘、鼠标、触摸、手柄、Narrator、Reduced Motion、中文、英文与 RTL。

构建与验证：

```powershell
dotnet restore .\WinUI3SeniorGallery.slnx -r win-x64 -p:PublishReadyToRun=true
dotnet build .\WinUI3SeniorGallery.slnx -c Release -p:PublishReadyToRun=true
powershell -NoProfile -ExecutionPolicy Bypass -File .\eng\scripts\Validate-Catalog.ps1
dotnet test .\WinUI3SeniorGallery.slnx -c Release --no-build --no-restore
```

## 设计原则

- **复合但可拆解**：组件解决真实界面任务，同时把播放、窗口、数据、网络等业务决策留给宿主。
- **性能是一等 API**：虚拟化、取消、窗口隔离、无阻塞输入和 Reduced Motion 不是优化项，而是契约。
- **现代化而非像素复刻**：历史产品的内容层级、焦点模型、节奏和空间感可以借鉴；原始截图、声音、壁纸、图标和专有资产不进入仓库。
- **默认可访问**：键盘/手柄焦点、AutomationPeer、朗读语义、文字缩放和高对比度在规格阶段锁定。
- **先约束再扩展**：稳定 API 宁可少，也不把尚未验证的设想伪装成公共承诺。

## 设计考古范围

研究范围覆盖 Windows Phone/Metro、Zune、Windows Media Center、Windows 8/10/11、Xbox、Microsoft 365、Classic Windows 与媒体软件史。每个展项记录产品年代、可核查来源、设计 DNA、现代化取舍及版权状态；例如 Cover Flow 明确归入媒体软件历史，不宣称为 Zune 原创。

## 路线图与贡献

近期目标是评审和实现六个 P0，并让 Gallery 成为这些组件的交互式契约测试场。之后再按 `specified → ready` 流程推进 Home Screen、Window Chrome、Widget Board、Docking、Canvas 与 ASR/翻译抽象。

贡献前请阅读 [CONTRIBUTING.md](CONTRIBUTING.md)、[CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md) 与 [SECURITY.md](SECURITY.md)。项目使用 [MIT License](LICENSE)。

> 本项目由社区维护，不隶属于、未获背书且不受 Microsoft 赞助。Windows、WinUI、Zune、Xbox、Microsoft 365 及相关名称和资产归各自权利人所有。
