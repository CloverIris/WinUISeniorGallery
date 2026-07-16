# Action Center Specification

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## Design DNA

队列、分组、优先级、可展开内容、内联动作和持久/短暂信息区分。

## 现代化边界

展项只研究模式；SnackbarHost 仅覆盖应用内短暂消息，不是 Action Center 替代品。不得读取或清除系统通知、复制 Quick Actions。

## 非目标

不补写未发布功能历史，不把 Gallery 模拟描述为 Windows Shell、系统服务或稳定 API。
