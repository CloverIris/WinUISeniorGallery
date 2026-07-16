# Pivot 验收

## 研究与边界

Given 页面加载，When 查看 Owner/来源，Then 显示 controls.pivot-view、PivotView、proposed/pending，且明确“现代化重制/非微软官方”；展项没有公共 API 或稳定资源键。

## 原型与现代差异

Given 原型结构图，When 切到 Modern Demo，Then 保留 Design DNA，同时明确列出舍弃行为；不得用现代行为冒充历史事实。

## Given / When / Then

Given 5 页且第 2 页选中，When 轻扫不足阈值，Then 回到第 2 页且焦点不变。Given 键盘切到第 3 页，Then 内容实现一次并公告选中状态。

## 输入、无障碍与响应式

左右轻扫保留；左右键/手柄肩键切换，Tab 进入页面内容，屏幕阅读器读“第 n 项，共 m 项”。RTL 下方向与视觉顺序镜像。 窄屏保留邻标题暗示；宽屏限制 HeaderStrip 内容宽度并可显示更多标题，不把 Pivot 扩展成侧栏。 验证 Light/Dark/High Contrast、Reduced Motion、Narrator、键鼠触笔/手柄、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

页面首帧静态说明目标小于 100 ms；Demo 60 Hz 且 UI 每帧小于 4 ms，卸载后无动画/Provider/事件。性能不达标则保留静态 Demo。

## 版权门禁

自动检查仓库不存在历史截图/声音/字体/提取资源；SOURCES 可达并含 2026-07-16。license_review 通过前状态不得升为 preview/stable。

