# MediaCenterExperience Acceptance

## Current gate

当前为 `in-progress`；本地合成目录可用，视觉/Automation/运行时验证待后续统一进行。

## Given / When / Then

- Given 两个分类，When 选择分类，Then Grid 只显示该分类条目并进入 Browse。
- Given 当前条目，When 选择并点击 Play，Then 触发 `SelectionRequested`，不创建播放器。
- Given Details 打开，When Close，Then 回到 Browse 并触发 `DetailsClosed`。
- Given 空集合或未知 ID，When 调用公开方法，Then 返回 false 且不抛出。
