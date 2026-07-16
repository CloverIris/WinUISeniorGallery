# Charms Bar（Windows 8 设计考古）

## 定位与边界

本展项研究 Windows 8（2012）将 Search、Share、Start、Devices、Settings 集中在右侧边缘的 Charms Bar。它不是系统级复刻，也不拥有稳定 API；应用内的现代实现由 `windowing.edge-command-panel`（`EdgeCommandPanel`）拥有。本目录只沉淀历史信息架构、可保留的设计 DNA 与被淘汰的系统假设。

## 工作单元

- 状态：`proposed`；成熟度：`lab`；优先级：`P2`；许可审查：`pending`。
- 研究展示路由：`/archaeology/windows-8/charms-bar`；Gallery 必须清晰标示“研究展项”，不得暗示可接管系统边缘手势。
- 依赖：`windowing.edge-command-panel`。该依赖仅用于解释现代功能归属，不授权本展项改变其 API。

## 文档导航与交付门禁

`SPEC` 记录历史结构和研究状态；`DESIGN` 记录可保留与拒绝的交互；`INTEGRATION` 说明 Gallery 展示的宿主边界；`ACCEPTANCE` 定义研究展示验收；`SOURCES` 记录证据与资产限制。除非现代 owner 完成独立规格，本展项不得进入实现。
