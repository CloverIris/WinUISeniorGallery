# Zune Quickplay 验收

## 研究与边界

Given 页面加载，When 查看 Owner/来源，Then 显示 experiences.home-screen、HomeScreen、proposed/pending，且明确“现代化重制/非微软官方”；展项没有公共 API 或稳定资源键。

## 原型与现代差异

Given 原型结构图，When 切到 Modern Demo，Then 保留 Design DNA，同时明确列出舍弃行为；不得用现代行为冒充历史事实。

## Given / When / Then

Given History 被禁用，When 首页加载，Then 不查询历史且显示可解释隐私空状态。Given 键盘重排 Pin，Then顺序持久化请求只发一次。

## 输入、无障碍与响应式

Tab/方向键/手柄在 Section 与项间导航，菜单键管理 Pin；触摸和鼠标等价。Narrator 读 Section 标题、项数和隐私状态。 窄屏纵向 Section，宽屏可形成两列但保持阅读顺序；200% 文本缩放标题换行，历史项不因布局被截断。 验证 Light/Dark/High Contrast、Reduced Motion、Narrator、键鼠触笔/手柄、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

页面首帧静态说明目标小于 100 ms；Demo 60 Hz 且 UI 每帧小于 4 ms，卸载后无动画/Provider/事件。性能不达标则保留静态 Demo。

## 版权门禁

自动检查仓库不存在历史截图/声音/字体/提取资源；SOURCES 可达并含 2026-07-16。license_review 通过前状态不得升为 preview/stable。

