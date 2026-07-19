# FileCard

Specification work item for FileCard.

## Status

in-progress / lab / P2. Local file descriptor, metadata presentation, and Preview/Action requests are implemented.

## Documents

- SPEC.en-US.md
- DESIGN.en-US.md
- INTEGRATION.en-US.md
- ACCEPTANCE.en-US.md

## Agent ownership

catalog/Experiences/FileCard

## Scenario readiness
The control does not read files, cache thumbnails, or execute delete/share. The host supplies FileCardDescriptor and handles PreviewRequested/ActionInvoked. Permission, cache, and destructive-action policy remain before Ready.
