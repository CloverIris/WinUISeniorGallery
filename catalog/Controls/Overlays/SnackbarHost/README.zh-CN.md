# SnackbarHost

`SnackbarHost` 为单个窗口提供短时消息呈现和独立队列；`ISnackbarService` 负责显式目标绑定、优先级、取消与去重。消息绝不跨窗口漂移。

## 状态

- 工作项：`controls.snackbar-host`
- 成熟度：Lab；优先级：P0；实现状态：Ready
- 包：`WinUI3.Senior.Controls`；Gallery：`/controls/snackbar-host`

## 依赖与所有权

依赖主题、动效和无障碍 Contract。实现 agent 仅可修改 `feature.json.owned_paths`；共享 Contract 缺失时标记 `blocked`。

## 文档

[规格](SPEC.zh-CN.md) · [设计](DESIGN.zh-CN.md) · [集成](INTEGRATION.zh-CN.md) · [验收](ACCEPTANCE.zh-CN.md)。中文为规范源，英文同步。
