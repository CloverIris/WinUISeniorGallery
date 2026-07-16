# Live Tile 验收

## 研究与边界

Given 页面加载，When 查看 Owner/来源，Then 显示 controls.dynamic-tile、DynamicTile、proposed/pending，且明确“现代化重制/非微软官方”；展项没有公共 API 或稳定资源键。

## 原型与现代差异

Given 原型结构图，When 切到 Modern Demo，Then 保留 Design DNA，同时明确列出舍弃行为；不得用现代行为冒充历史事实。

## Given / When / Then

Given 宽 Tile 有两页内容，When Reduced Motion，Then 内容即时替换且状态公告一次。Given 数据过期，Then 显示静态回退并保留调用。

## 输入、无障碍与响应式

点击/Enter/A 调用，键盘/触摸拖拽重排由外层 Board 负责；动态内容不改变焦点名称，屏幕阅读器公告聚合状态而非每次翻面。 尺寸使用现代栅格断点而非 WP 像素；窄屏至少两列，文本缩放时增加高度或隐藏次要文案，不缩小触控目标。 验证 Light/Dark/High Contrast、Reduced Motion、Narrator、键鼠触笔/手柄、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

页面首帧静态说明目标小于 100 ms；Demo 60 Hz 且 UI 每帧小于 4 ms，卸载后无动画/Provider/事件。性能不达标则保留静态 Demo。

## 版权门禁

自动检查仓库不存在历史截图/声音/字体/提取资源；SOURCES 可达并含 2026-07-16。license_review 通过前状态不得升为 preview/stable。

