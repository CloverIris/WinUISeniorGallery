# MixviewExperience Acceptance

## Current gate

当前为 `in-progress`；实现可供本地 Gallery 调试，但仍需视觉、Automation 和构建验证后才可进入 `review`。

## Given / When / Then

- Given 五个确定性节点，When 打开根节点，Then 根节点居中且关联节点按顺序环绕。
- Given 当前图已打开，When 用户点击关联节点，Then `NodeSelected.IsUserInitiated=true`、状态更新且 Live Region 公告标题。
- Given 当前图已打开，When 按 Escape，Then 图关闭并触发 `Closed`，不发生页面导航。
- Given 空集合或未知 ID，When 调用 `Open`/`SelectNode`，Then 返回 false 且当前状态保持。

## Matrix and budget

Light/Dark/High Contrast、Reduced Motion、RTL、100%/150%/200% DPI、键盘/鼠标/触摸和 Narrator；默认最多绘制 13 个节点按钮，布局重绘目标 P95 ≤ 16ms（待运行时记录）。
