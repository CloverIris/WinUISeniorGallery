# BottomSheet

`BottomSheet` 是支持模态/非模态、多个吸附点和拖拽关闭的自适应覆盖层。它负责呈现、焦点、遮罩和手势，不负责业务导航或数据持久化。

## 状态

- 工作项：`controls.bottom-sheet`
- 成熟度：Lab；优先级：P0；实现状态：Ready
- 包：`WinUI3.Senior.Controls`；Gallery：`/controls/bottom-sheet`

## 依赖与所有权

依赖主题、动效、输入和无障碍 Contract。实现 agent 仅可修改 `feature.json.owned_paths`；共享 Contract 缺失时标记 `blocked`。

## 文档

[规格](SPEC.zh-CN.md) · [设计](DESIGN.zh-CN.md) · [集成](INTEGRATION.zh-CN.md) · [验收](ACCEPTANCE.zh-CN.md)。中文为规范源，英文为同步翻译。
