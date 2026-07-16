# Snap Layouts 验收

## 研究与所有权

Given 页面加载，When 查看元数据，Then 显示 proposed/pending、owner 为 windowing.dock-layout-preview（DockLayoutPreview）、现代化重制/非微软官方，且展项不提供稳定 API。

## 历史与现代差异

Given 历史结构，When 切换 Demo，Then 保留 Design DNA 并明确废弃边界，不冒充 Windows Shell/系统能力。

## Given / When / Then

Given 指针拖动内部面板，When 进入二区布局右区，Then 只显示应用内预览且不移动 AppWindow。Given 按 Esc，Then 原布局/焦点完整恢复。

## 输入、High Contrast、RTL 与自动化

研究页演示指针悬停、点击、方向键和 Enter，但不注册 Win+Z；Esc 取消预览并恢复拖拽源焦点。 Demo 按宿主内容区生成 2–6 个区域，窄窗口减少方案；RTL 镜像区域顺序，DPI/显示器变化重算应用内坐标。 验证 Light/Dark/High Contrast、Reduced Motion、Narrator、键鼠触笔/手柄、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

静态说明首帧小于 100 ms；Demo 目标 60 Hz/UI 每帧小于 4 ms，卸载后无 Provider/计时/事件；不达标保留静态研究。

## 版权门禁

自动检查无 Windows 11 截图/系统图标/声音/字体/新闻/提取资产；SOURCES 保持 13 文件形状。license_review 通过前不得升 preview/stable。

