# Zune Mixview 验收

## 研究与边界

Given 页面加载，When 查看 Owner/来源，Then 显示 experiences.mixview、MixviewExperience、proposed/pending，且明确“现代化重制/非微软官方”；展项没有公共 API 或稳定资源键。

## 原型与现代差异

Given 原型结构图，When 切到 Modern Demo，Then 保留 Design DNA，同时明确列出舍弃行为；不得用现代行为冒充历史事实。

## Given / When / Then

Given 中心有 40 关系，When 加载，Then 首屏按规则选最多 12 节点并可分页。Given Narrator，Then 列表顺序与视觉关系类别一致。

## 输入、无障碍与响应式

方向键/手柄按空间邻近，Enter/A 重新居中，Esc/B 返回上一中心；提供线性关系列表给 Narrator 和无法使用二维导航者。 窄屏减少环数并提供底部详情，宽屏扩展半径但限制节点数；文本缩放时放大节点而非压缩标签。 验证 Light/Dark/High Contrast、Reduced Motion、Narrator、键鼠触笔/手柄、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

页面首帧静态说明目标小于 100 ms；Demo 60 Hz 且 UI 每帧小于 4 ms，卸载后无动画/Provider/事件。性能不达标则保留静态 Demo。

## 版权门禁

自动检查仓库不存在历史截图/声音/字体/提取资源；SOURCES 可达并含 2026-07-16。license_review 通过前状态不得升为 preview/stable。

