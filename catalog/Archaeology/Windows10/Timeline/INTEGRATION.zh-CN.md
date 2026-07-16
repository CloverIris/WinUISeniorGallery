# Timeline Integration

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## Feature 引用

无稳定 API；未来活动流体验候选 / no stable API; future activity-feed candidate。考古展项拥有来源与解释，现代 feature 拥有可复用实现；两者状态不得互相冒充。

## 平台和数据边界

只做应用自身数据的 ActivityTimeline 展示概念；不得读取系统活动历史、冒充 Task View、声称跨设备 Graph 恢复或收集用户文档。 默认使用假数据与本地资源，不请求账户、联系人、系统通知、活动历史或外部服务。
