# ImmersiveNowPlaying

这是中文规范源。当前进入 in-progress，实现位于 Media 包的可复用 Now Playing 控制与状态协调层。

## Status

in-progress / lab / P1

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Experiences/ImmersiveNowPlaying  
src/WinUI3.Senior.Media/NowPlaying

## 场景准备度
沉浸展示当前媒体、歌词与控制。当前代码提供宿主拥有的会话绑定、队列选择、播放命令串行化、重复/随机策略和全屏请求；不创建窗口、不打开文件、不解析字幕。Ready 条件：无封面/长歌词/视频三原型锁定信息层级，并补齐模板与视觉验收。
