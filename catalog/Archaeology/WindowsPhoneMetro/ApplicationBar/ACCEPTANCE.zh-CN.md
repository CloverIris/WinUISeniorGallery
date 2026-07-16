# Application Bar 验收

## 研究与边界

Given 页面加载，When 查看 Owner/来源，Then 显示 controls.expandable-command-bar、ExpandableCommandBar、proposed/pending，且明确“现代化重制/非微软官方”；展项没有公共 API 或稳定资源键。

## 原型与现代差异

Given 原型结构图，When 切到 Modern Demo，Then 保留 Design DNA，同时明确列出舍弃行为；不得用现代行为冒充历史事实。

## Given / When / Then

Given 6 命令，When 折叠，Then 4 主命令可见、2 次级在展开后可达。Given Esc 从次级菜单，Then 先关菜单再收起 Bar 并恢复焦点。

## 输入、无障碍与响应式

触摸上滑或省略号展开，键盘/手柄可聚焦所有命令，Esc/B 逐层关闭；关闭恢复触发命令或内容焦点。 窄/触摸优先底部；宽屏可保留底部但限制宽度，不自动变成顶栏。安全区域和输入面板 inset 由宿主提供。 验证 Light/Dark/High Contrast、Reduced Motion、Narrator、键鼠触笔/手柄、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

页面首帧静态说明目标小于 100 ms；Demo 60 Hz 且 UI 每帧小于 4 ms，卸载后无动画/Provider/事件。性能不达标则保留静态 Demo。

## 版权门禁

自动检查仓库不存在历史截图/声音/字体/提取资源；SOURCES 可达并含 2026-07-16。license_review 通过前状态不得升为 preview/stable。

