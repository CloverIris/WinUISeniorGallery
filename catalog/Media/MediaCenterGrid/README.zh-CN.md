# MediaCenterGrid

定义面向电视、遥控器和客厅距离的虚拟化海报网格，二维焦点移动时放大当前项并呈现详情。

## 状态与范围

- 状态：proposed / lab / P1
- 依赖：controls.content-rail
- 不允许实现；候选 API/模板名只用于评审。

## 宿主边界

保留在当前页面视觉树，不创建窗口、不全屏、不播放。宿主提供 ItemsSource、模板、导航和详情；窗口、播放与后台加载归外部服务。

## 文档与 Agent 所有权

SPEC 定义晋级门禁，DESIGN 覆盖视觉/输入，INTEGRATION 覆盖生命周期，ACCEPTANCE 给出 Given/When/Then。Agent 仅拥有 catalog/Media/MediaCenterGrid。

