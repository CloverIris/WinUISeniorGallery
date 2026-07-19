# MixviewExperience Design

## Information hierarchy

中心节点是当前上下文，关联节点按 `RelatedIds` 顺序环绕；节点标题必须可读，焦点边框必须可见。Gallery 使用合成文本，不使用历史截图或受版权保护素材。

## Responsive layout

控件按可用宽高计算中心点；窄窗口保持中心节点并允许关联节点超出可见区域，宿主可通过 `MaxVisibleNodes` 降低密度。节点按钮最小触控尺寸 48×48。

## Motion and input

当前实现使用确定性重绘；`IsReducedMotion` 保留给后续转场策略。鼠标、触摸和键盘均通过 Button/Focus 语义进入选择，Escape 关闭。

## Accessibility and themes

每个节点设置 Automation Name，`PART_LiveRegion` 以 Polite 公告用户选择。Light/Dark/High Contrast 使用主题资源；RTL 不反转关系顺序或数据 ID。

## Modernization tradeoffs

保留“关联内容围绕当前内容展开”的设计 DNA，但用二维 Canvas 和宿主节点模型替代历史产品的专有推荐服务。
