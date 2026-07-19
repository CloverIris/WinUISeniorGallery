# GameBarWidgetExperience

中文规范源。当前为 `in-progress` lab，提供宿主拥有的浮动 Widget 生命周期和可恢复的点击穿透交互模式。

## Status

in-progress / lab / P2

## Scope

控件管理 Interactive、ClickThrough、Minimized 和 Closed 状态，要求点击穿透始终带恢复热键并获得宿主确认；它不修改命中测试、不创建窗口。

## Ownership

实现：`src/WinUI3.Senior.Windowing/GameBarWidgetExperience.cs`；演示：`src/WinUI3.Senior.Gallery/Pages/GameBarWidgetPage.xaml*`。
