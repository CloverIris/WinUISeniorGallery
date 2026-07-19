# DynamicTile

这是中文规范源。当前为 in-progress，已进入业务逻辑实现阶段；仍需由宿主补齐模板与视觉验收。

## Status

in-progress / lab / P1

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Collections/DynamicTile

## 当前实现面

`DynamicTile` 提供稳定 `Id`、`Frames`、`Size`、`Transition`、自动轮换、人工切换、宿主可见性/窗口活跃度暂停和取消式 `RefreshProvider`。
`DynamicTileBoard` 负责多个 tile 的确定性排序、编辑模式重排、暂停广播和并行刷新；它不创建窗口、不注册系统磁贴，也不持久化路径或内容。

应用内动态卡片而非系统磁贴；进入 ready 前仍需锁定视觉更新队列上限，默认只保留最新 3 面。
