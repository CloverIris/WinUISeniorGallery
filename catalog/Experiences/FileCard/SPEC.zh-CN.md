# FileCard Specification

## Goal

目标：提供宿主拥有来源的文件预览元数据卡。控件不打开文件、不缓存缩略图、不执行删除/分享。

## Non-goals

不实现文件系统访问、Provider、权限申请、缩略图解码或持久化路径。

## Public API

`FileCardDescriptor`（Id/Name/Kind/Size/Modified/Thumbnail/Sharing/Actions）、`FileCardAction`、`File`、`IsPreviewEnabled`、`IsActionsVisible`；方法 `RequestPreview`、`InvokeAction`；事件 `PreviewRequested`、`ActionInvoked`。

## State model

文件为空显示空卡；Size 归一化为非负并提供 SizeText；重复动作 Id 保留首项；Preview 未被 Handled 时由宿主决定是否拒绝。

## Template parts and visual tree

`PART_Root`、`PART_Thumbnail`、`PART_Name`、`PART_Metadata`、`PART_Sharing`、`PART_Actions` 可选；缺失缩略图不影响元数据和动作。

## Behavior and failure modes

所有动作事件只携带 Descriptor，不暴露 FileInfo/StorageFile；宿主卸载时不再回调；Automation Name 使用 `File {Name}`。

## Open Decisions

Open Decisions：Provider 权限边界、缩略图缓存协议、危险操作确认和分享状态隐私规则。

## 场景、数据与视觉树
模型 FileDescriptor(Id,Name,Kind,Size,Modified,Thumbnail,Sharing,Actions)；树 `WidgetCard→Preview/Metadata/Actions`；状态 Loading/Ready/Unavailable/Error。
