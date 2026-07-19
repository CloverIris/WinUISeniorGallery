# FileCard Specification

## Goal

Provide a host-owned file preview metadata card. The control does not open files, cache thumbnails, or execute delete/share.

## Non-goals

No file-system access, provider, permission request, thumbnail decoding, or persisted path.

## Public API

`FileCardDescriptor` (Id/Name/Kind/Size/Modified/Thumbnail/Sharing/Actions), `FileCardAction`, `File`, `IsPreviewEnabled`, and `IsActionsVisible`; methods `RequestPreview` and `InvokeAction`; events `PreviewRequested` and `ActionInvoked`.

## State model

Null File shows an empty card; Size is normalized non-negative with SizeText; duplicate action IDs keep the first; the host decides whether an unhandled Preview is rejected.

## Template parts and visual tree

`PART_Root`, `PART_Thumbnail`, `PART_Name`, `PART_Metadata`, `PART_Sharing`, and `PART_Actions` are optional; a missing thumbnail does not affect metadata or actions.

## Behavior and failure modes

Action events carry only the Descriptor and never expose FileInfo/StorageFile; hosts stop callbacks on unload; Automation Name uses `File {Name}`.

## Open Decisions

Open Decisions: provider permission boundary, thumbnail cache protocol, destructive confirmation, and sharing privacy rules.

## Scenario, data, and visual tree
FileDescriptor(Id,Name,Kind,Size,Modified,Thumbnail,Sharing,Actions); tree `WidgetCard→Preview/Metadata/Actions`; Loading/Ready/Unavailable/Error.
