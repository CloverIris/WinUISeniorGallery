# Media Center Horizontal Strip 验收

## 研究与边界

Given 页面加载，When 查看 Owner/来源，Then 显示 controls.content-rail、ContentRail、proposed/pending 与“现代化重制/非微软官方”；没有公共 API/稳定资源键。

## 原型与现代差异

Given 原型结构，When 切 Modern Demo，Then 保留 DNA 并明确舍弃项，不用现代行为冒充历史事实。

## Given / When / Then

Given 1000 项且焦点第 5，When 右移到缓冲外，Then 仅实现邻近项、焦点在 50 ms 内反馈并滚入安全区。

## 输入、无障碍与响应式

D-pad/摇杆左右移动，A/Enter 调用，B/Esc 返回 Section；滚轮/触摸滑动等价。Narrator 读类别、位置和标题。 窄窗口退化为卡片列表，宽屏可显示 4–8 项但限制卡片最大宽度；10-foot 模式放大排版/焦点，不改数据。 验证主题、Reduced Motion、Narrator、多输入、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

静态说明首帧目标小于 100 ms；Demo 60 Hz/UI 每帧小于 4 ms，卸载后无动画/Provider/事件；不达标则保留静态 Demo。

## 版权门禁

自动检查无历史截图/声音/字体/提取资源；SOURCES 可达并含 2026-07-16。license_review 通过前不得升 preview/stable。

