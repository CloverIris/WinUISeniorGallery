# EditorCanvas

这是中文规范源。当前为 in-progress lab；Core 文档状态和 WinUI 交互壳已进入本地实现，文件/原生画布渲染仍由后续阶段负责。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Experiences/EditorCanvas；实现目录由 feature.json 的 owned_paths 明确列出。

## 场景准备度
演示 Core 文档快照、对象插入/移动/删除、选区、Undo/Redo、视口缩放和平移，以及宿主提供的压感笔画会话。当前不读取文件、不创建原生窗口、不实现协作或序列化。进入 ready 前仍需锁定 100k 对象渲染预算、页面/图层模型和持久化边界。
