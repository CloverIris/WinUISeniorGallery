# Zune Album Art Wall 验收

## 研究与边界

Given 页面加载，When 查看 Owner/来源，Then 显示 controls.adaptive-grid、AdaptiveGrid、proposed/pending，且明确“现代化重制/非微软官方”；展项没有公共 API 或稳定资源键。

## 原型与现代差异

Given 原型结构图，When 切到 Modern Demo，Then 保留 Design DNA，同时明确列出舍弃行为；不得用现代行为冒充历史事实。

## Given / When / Then

Given 5000 专辑，When 快速滚动，Then 只加载视口缓冲且取消迟到封面。Given 封面缺失，Then 稳定占位保持网格/焦点。

## 输入、无障碍与响应式

方向键/手柄按几何邻项，鼠标/触摸调用；放大不改变焦点坐标，屏幕阅读器读专辑/艺术家而不是“图片”。 窄屏减少列数，宽屏限制单封面最大尺寸；背景模式降低细节/更新频率，前景浏览模式保留焦点和文字。 验证 Light/Dark/High Contrast、Reduced Motion、Narrator、键鼠触笔/手柄、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

页面首帧静态说明目标小于 100 ms；Demo 60 Hz 且 UI 每帧小于 4 ms，卸载后无动画/Provider/事件。性能不达标则保留静态 Demo。

## 版权门禁

自动检查仓库不存在历史截图/声音/字体/提取资源；SOURCES 可达并含 2026-07-16。license_review 通过前状态不得升为 preview/stable。

