# GuideMenuExperience

中文规范源。当前为 `in-progress` lab，已实现宿主中立的分层 Guide 菜单和 Gallery 实验页。

## Status

in-progress / lab / P2

## Scope

宿主提供 `GuideNode` 树；控件负责打开、层级进入、返回、Esc 关闭和叶节点事件，不执行导航、窗口切换或命令副作用。

## Ownership

实现：`src/WinUI3.Senior.Controls/Experiences/GuideMenu/GuideMenuExperience.cs`；演示：`src/WinUI3.Senior.Gallery/Pages/GuideMenuPage.xaml*`。
