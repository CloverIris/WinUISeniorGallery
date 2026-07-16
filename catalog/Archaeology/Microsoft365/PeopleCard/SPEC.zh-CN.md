# People Card 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Outlook 与 Microsoft 365 的人物卡片把头像、姓名、职位、状态、联系方式、最近协作和快捷动作汇聚到悬停/点击浮层。

## 设计基因

在不离开上下文时补充身份信息、渐进披露、统一身份与状态、动作紧邻对象、权限不足时逐节降级。

## 现代化重制

重制为数据源无关 PeopleCard：宿主提供最小人物模型和按需 section Provider；默认只展示本地数据，不自行访问 Microsoft Graph。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 experiences.people-card（PeopleCard）拥有。 展项不声明公共 API，不复制现代组件；历史名称只用于研究标题，演示标注“现代化重制”。

## 状态与失败模式

Compact、Expanded、LoadingSection、Partial、Unavailable；不同 section 独立加载、失败和重试，不阻塞姓名与主操作。 数据、服务或依赖缺失时必须显示局部且可解释的降级，不能用假数据冒充真实账号结果。
