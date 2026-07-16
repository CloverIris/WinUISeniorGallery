# Semantic Zoom 研究规格

目标是记录“缩小改变信息语义”的导航模型，而非缩放控件外观。历史树为 `Collection → DetailedView / OverviewGroups → GroupAnchor → DetailedView`；讲解状态为 `Detail`、`TransitioningToOverview`、`Overview`、`Returning`、`NoGroupMapping`。Overview 以组/首字母/时间等索引定位，返回时锚定相关细节。

历史输入包括捏合、Ctrl+滚轮、按钮和键盘；Gallery 只能模拟。没有分组映射、集合为空、缩放被禁用、数据替换或动效关闭时，应显示稳定概览/说明且不丢失可访问焦点。非目标：定义缩放范围、虚拟化实现、`PART_*`、AutomationPeer 或实际手势 API；均由 `SemanticZoomView` owner 决定。
