# Voice Access Overlay 验收

## 研究与所有权

Given 页面加载，When 查看元数据，Then 显示 proposed/pending、owner 为 未分配（无稳定 API）、现代化重制/非微软官方，且展项不提供稳定 API。

## 历史与现代差异

Given 历史结构，When 切换 Demo，Then 保留 Design DNA 并明确废弃边界，不冒充 Windows Shell/系统能力。

## Given / When / Then

Given 研究 Demo，When 激活“显示数字”，Then 只标记 Demo 内可调用元素、不采集音频、不改变系统焦点。Given Esc，Then 标签消失并恢复触发按钮。

## 输入、High Contrast、RTL 与自动化

模拟可由按钮/键盘触发 Listening、Numbers 和 Grid；不注册全局热键。焦点不移动到数字标签，Esc 关闭最内层覆盖并恢复触发按钮。 数字标签避让目标和边缘，网格按本地 Demo 面积重算；RTL 只影响文本，不改变数字读法。High Contrast 使用实色边界且标签不遮挡焦点。 验证 Light/Dark/High Contrast、Reduced Motion、Narrator、键鼠触笔/手柄、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

静态说明首帧小于 100 ms；Demo 目标 60 Hz/UI 每帧小于 4 ms，卸载后无 Provider/计时/事件；不达标保留静态研究。

## 版权门禁

自动检查无 Windows 11 截图/系统图标/声音/字体/新闻/提取资产；SOURCES 保持 13 文件形状。license_review 通过前不得升 preview/stable。

