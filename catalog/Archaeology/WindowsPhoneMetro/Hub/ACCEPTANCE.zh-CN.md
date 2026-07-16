# Hub 验收

## 研究与边界

Given 页面加载，When 查看 Owner/来源，Then 显示 experiences.hub-panorama、HubPanorama、proposed/pending，且明确“现代化重制/非微软官方”；展项没有公共 API 或稳定资源键。

## 原型与现代差异

Given 原型结构图，When 切到 Modern Demo，Then 保留 Design DNA，同时明确列出舍弃行为；不得用现代行为冒充历史事实。

## Given / When / Then

Given 三个异构 Section，When 从第 1 滚到第 2，Then 只实现邻近 Section、背景视差不影响文字对比。Given 嵌套纵向列表滚动，Then Hub 不抢夺纵向手势。

## 输入、无障碍与响应式

触摸/触控板平移、Shift+滚轮、方向键/手柄按 Section 移动；嵌套列表优先其主轴，B/Esc 返回宿主。 窄屏呈现经典单 Section 视口；宽屏允许 1.5–2.5 Section，但正文最大宽度受限；高 DPI 不改变语义跨度。 验证 Light/Dark/High Contrast、Reduced Motion、Narrator、键鼠触笔/手柄、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

页面首帧静态说明目标小于 100 ms；Demo 60 Hz 且 UI 每帧小于 4 ms，卸载后无动画/Provider/事件。性能不达标则保留静态 Demo。

## 版权门禁

自动检查仓库不存在历史截图/声音/字体/提取资源；SOURCES 可达并含 2026-07-16。license_review 通过前状态不得升为 preview/stable。

