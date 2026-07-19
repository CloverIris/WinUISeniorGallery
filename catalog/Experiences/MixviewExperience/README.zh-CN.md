# MixviewExperience

这是中文规范源。当前为 `in-progress` lab，已提供宿主中立的径向关联图业务逻辑和本地 Gallery 实验页。

## Status

in-progress / lab / P2

## Scope

关系节点由宿主提供，控件负责确定性径向布局、选择、Esc 关闭和 Live Region 公告。控件不负责推荐、导航、网络、媒体加载或窗口创建。

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

实现路径：`src/WinUI3.Senior.Controls/Experiences/Mixview/MixviewExperience.cs`；Gallery：`src/WinUI3.Senior.Gallery/Pages/MixviewPage.xaml*`。

## Readiness

业务逻辑和本地演示已存在；视觉性能、Automation 和主题矩阵仍待 review，暂不推进到 `done`。
