# Zune Big Typography 验收

## 研究与边界

Given 页面加载，When 查看 Owner/来源，Then 显示 controls.big-title、BigTitle、proposed/pending，且明确“现代化重制/非微软官方”；展项没有公共 API 或稳定资源键。

## 原型与现代差异

Given 原型结构图，When 切到 Modern Demo，Then 保留 Design DNA，同时明确列出舍弃行为；不得用现代行为冒充历史事实。

## Given / When / Then

Given 中文长标题且 200% 文本，When 宽度 320 epx，Then 不裁切/横滚且正文仍可达。Given Reduced Motion，Then Expanded/Compact 即时切换。

## 输入、无障碍与响应式

标题默认非交互，不进入 Tab；若绑定返回/导航则必须有按钮语义和标准焦点，不让整块装饰文字成为隐形命中区。 CJK 使用适合字重和较小比例，德语长词换行，RTL 对齐镜像；200% 文本缩放允许多行/紧凑态而非裁切。 验证 Light/Dark/High Contrast、Reduced Motion、Narrator、键鼠触笔/手柄、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

页面首帧静态说明目标小于 100 ms；Demo 60 Hz 且 UI 每帧小于 4 ms，卸载后无动画/Provider/事件。性能不达标则保留静态 Demo。

## 版权门禁

自动检查仓库不存在历史截图/声音/字体/提取资源；SOURCES 可达并含 2026-07-16。license_review 通过前状态不得升为 preview/stable。

