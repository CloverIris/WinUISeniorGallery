# Timeline Specification

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## Design DNA

时间顺序、活动缩略图、搜索、继续任务和跨来源统一模型。

## 现代化边界

只做应用自身数据的 ActivityTimeline 展示概念；不得读取系统活动历史、冒充 Task View、声称跨设备 Graph 恢复或收集用户文档。

## 非目标

不补写未发布功能历史，不把 Gallery 模拟描述为 Windows Shell、系统服务或稳定 API。
