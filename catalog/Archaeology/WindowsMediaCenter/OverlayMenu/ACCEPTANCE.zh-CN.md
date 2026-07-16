# Media Center Overlay Menu 验收

## 研究与边界

Given 页面加载，When 查看 Owner/来源，Then 显示 controls.overlay-menu、OverlayMenu、proposed/pending 与“现代化重制/非微软官方”；没有公共 API/稳定资源键。

## 原型与现代差异

Given 原型结构，When 切 Modern Demo，Then 保留 DNA 并明确舍弃项，不用现代行为冒充历史事实。

## Given / When / Then

Given 字幕子菜单打开且焦点在语言，When 自动隐藏到期，Then Menu 保持可见。Given 连按 B 两次，Then 先回 Root 再关闭并恢复视频焦点。

## 输入、无障碍与响应式

Menu/Enter/A 打开，D-pad 导航，B/Esc 逐层返回；鼠标/触摸/键盘等价。菜单、焦点或子弹层存在时不自动隐藏。 窄屏底部 Sheet，宽屏中心/侧面 Overlay，10-foot 增大目标；CompactOverlay 只保留核心命令，不塞完整设置。 验证主题、Reduced Motion、Narrator、多输入、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

静态说明首帧目标小于 100 ms；Demo 60 Hz/UI 每帧小于 4 ms，卸载后无动画/Provider/事件；不达标则保留静态 Demo。

## 版权门禁

自动检查无历史截图/声音/字体/提取资源；SOURCES 可达并含 2026-07-16。license_review 通过前不得升 preview/stable。

