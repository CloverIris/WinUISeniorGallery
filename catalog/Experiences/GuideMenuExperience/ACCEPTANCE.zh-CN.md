# GuideMenuExperience Acceptance

## Current gate

`in-progress`；本地实验可用，视觉/Automation/构建验证完成后才进入 `review`。

## Given / When / Then

- Given 根节点含子节点，When 调用 `Invoke(child)`, Then `NavigationPath` 深度增加并刷新当前项。
- Given 已进入子层，When 按 Escape，Then 返回父层而不是关闭。
- Given 叶节点，When 调用 `Invoke(leaf)`, Then 触发 `NodeInvoked`；关闭行为遵循 `IsDismissOnLeafInvoke`。
- Given 空集合或未知 ID，When 调用公开方法，Then 返回 false 且不改变状态。

## Matrix

Light/Dark/High Contrast、RTL、100%/150%/200% DPI、键盘/鼠标/触摸、Narrator 和 Reduced Motion。
