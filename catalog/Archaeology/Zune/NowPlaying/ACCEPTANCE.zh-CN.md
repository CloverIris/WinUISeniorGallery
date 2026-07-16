# Zune Now Playing 验收

## 研究与边界

Given 页面加载，When 查看 Owner/来源，Then 显示 experiences.immersive-now-playing、ImmersiveNowPlaying、proposed/pending，且明确“现代化重制/非微软官方”；展项没有公共 API 或稳定资源键。

## 原型与现代差异

Given 原型结构图，When 切到 Modern Demo，Then 保留 Design DNA，同时明确列出舍弃行为；不得用现代行为冒充历史事实。

## Given / When / Then

Given 无封面/艺术家图，When 播放，Then 主题实色、元数据和控制完整。Given 焦点在音量菜单，When 自动隐藏到期，Then 控件保持可见。

## 输入、无障碍与响应式

任意输入显示控制；键盘焦点/菜单/拖动时不隐藏。Space/A 播放，方向键时间轴，B/Esc 退出沉浸；背景不命中。 小窗转为普通播放器，宽屏保留环境留白；CompactOverlay 不复刻全屏背景。高对比使用实色，不从封面取色。 验证 Light/Dark/High Contrast、Reduced Motion、Narrator、键鼠触笔/手柄、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

页面首帧静态说明目标小于 100 ms；Demo 60 Hz 且 UI 每帧小于 4 ms，卸载后无动画/Provider/事件。性能不达标则保留静态 Demo。

## 版权门禁

自动检查仓库不存在历史截图/声音/字体/提取资源；SOURCES 可达并含 2026-07-16。license_review 通过前状态不得升为 preview/stable。

