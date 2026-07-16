# App Contracts 研究规格

历史结构为 `UserIntent → AppContext → ContractRequest → SystemBroker → Target/Result`，讲解状态为 `Idle`、`Eligible`、`Invoked`、`AwaitingTarget`、`Completed`、`Cancelled`、`Unavailable`。目标是研究可发现的跨应用任务交接；非目标是重建 broker、枚举其他应用、自动发送内容、申请文件/联系人/网络权限或承诺任何现代 API。Gallery 仅可切换这些说明状态；取消、无目标、权限拒绝或平台缺失均显示明确结果，不模拟成功。
