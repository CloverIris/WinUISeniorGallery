# AchievementToast

这是中文规范源。当前为 in-progress lab，已具备本地队列、定时关闭和模板绑定实现。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Overlays/AchievementToast

## 实现准备
每个窗口独立 FIFO 队列；`ShowAsync` 接收不可变请求，当前项展示图标、标题、描述、进度和稀有度，宿主销毁时未完成请求返回 `HostDestroyed`。不复用 Snackbar 队列，不创建窗口。
