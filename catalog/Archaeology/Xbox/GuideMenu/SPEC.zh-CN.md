# Guide Menu 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

从 Xbox 360 Guide 按键延续至 Xbox One/Series 的侧边快速导航；覆盖当前游戏并提供主页、好友、成就、捕获和系统操作的分层入口。

## 设计基因

随时可达、覆盖而不离开上下文、一级目的地稳定、层级返回可预测、为手柄优化的纵向焦点和快捷肩键切换。

## 现代化重制

重制为应用内 Guide 样式侧栏，用于沉浸式媒体或大屏应用；只承载应用命令，不替代系统导航或拦截保留按键。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 experiences.guide-menu（GuideMenuExperience）拥有。 展项不得声明公共类型、资源键或平台服务，也不得复制稳定实现；Gallery 通过依赖 ID 组合现代组件并标注 Inspired by，而非 Original。

## 状态与失败模式

Closed、Root、Nested、ModalChild；B/Esc 从子层返回，根层再关闭；打开时保存并在关闭后恢复原焦点。 缺少依赖、数据或平台能力时显示解释性降级，不伪造成功状态；展项关闭后释放演示状态并恢复进入前焦点。
