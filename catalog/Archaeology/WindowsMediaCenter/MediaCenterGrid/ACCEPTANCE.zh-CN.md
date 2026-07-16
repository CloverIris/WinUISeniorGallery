# Media Center Grid 验收

## 研究与边界

Given 页面加载，When 查看 Owner/来源，Then 显示 media.media-center-grid、MediaCenterGrid、proposed/pending 与“现代化重制/非微软官方”；没有公共 API/稳定资源键。

## 原型与现代差异

Given 原型结构，When 切 Modern Demo，Then 保留 DNA 并明确舍弃项，不用现代行为冒充历史事实。

## Given / When / Then

Given 10,000 项焦点 ID 42，When Window 从 6 列变 3 列，Then 焦点仍 ID 42、仅视口缓冲实现且详情不闪错项。

## 输入、无障碍与响应式

D-pad/摇杆按几何邻项，A/Enter 调用，B/Esc 返回；鼠标、触摸和键盘同等。Automation 读行列、标题、选中与展开详情。 按最小卡片宽度重排 1–8 列，保留稳定 ID 焦点；文本缩放减少列数，不缩小字体；High Contrast 用系统焦点环。 验证主题、Reduced Motion、Narrator、多输入、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

静态说明首帧目标小于 100 ms；Demo 60 Hz/UI 每帧小于 4 ms，卸载后无动画/Provider/事件；不达标则保留静态 Demo。

## 版权门禁

自动检查无历史截图/声音/字体/提取资源；SOURCES 可达并含 2026-07-16。license_review 通过前不得升 preview/stable。

