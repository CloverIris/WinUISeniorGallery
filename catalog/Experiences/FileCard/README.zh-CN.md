# FileCard

这是中文规范源。当前为 in-progress lab，已具备本地文件描述、元数据展示和 Preview/Action 请求。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Experiences/FileCard

## 场景准备度
控件不读取文件、不缓存缩略图、不执行删除或分享；宿主提供 FileCardDescriptor 并处理 PreviewRequested/ActionInvoked。进入 ready 前仍需锁定权限、缓存和危险操作确认策略。
