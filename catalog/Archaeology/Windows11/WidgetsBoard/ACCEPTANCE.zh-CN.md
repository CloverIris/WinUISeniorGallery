# Widgets Board 验收

## 研究与所有权

Given 页面加载，When 查看元数据，Then 显示 proposed/pending、owner 为 experiences.widgets-board（WidgetsBoard）、现代化重制/非微软官方，且展项不提供稳定 API。

## 历史与现代差异

Given 历史结构，When 切换 Demo，Then 保留 Design DNA 并明确废弃边界，不冒充 Windows Shell/系统能力。

## Given / When / Then

Given 三个 Widget 且一个刷新失败，When Board 打开，Then 其他卡片可用、失败卡片局部重试。Given 键盘重排，Then 持久化请求一次且焦点随 ID。

## 输入、High Contrast、RTL 与自动化

键盘/手柄在卡片间二维导航，菜单键打开操作，拖拽有键盘替代；关闭 Board 恢复入口焦点，卡片刷新不抢焦点。 窄屏单列，常规 2 列，宽屏 3–4 列；200% 文本增加卡片高度。RTL 镜像网格但时间/图表语义按区域设置。 验证 Light/Dark/High Contrast、Reduced Motion、Narrator、键鼠触笔/手柄、RTL、100%–300% DPI、200% 文本。

## 生命周期与性能

静态说明首帧小于 100 ms；Demo 目标 60 Hz/UI 每帧小于 4 ms，卸载后无 Provider/计时/事件；不达标保留静态研究。

## 版权门禁

自动检查无 Windows 11 截图/系统图标/声音/字体/新闻/提取资产；SOURCES 保持 13 文件形状。license_review 通过前不得升 preview/stable。

