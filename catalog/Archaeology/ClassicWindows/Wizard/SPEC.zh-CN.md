# Wizard / Stepper 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Windows 安装、配置和添加硬件长期使用多页向导，把复杂任务拆成选择、输入、确认、执行和完成步骤，并提供 Back/Next/Cancel。

## 设计基因

渐进收集信息、每步验证、提交前可回退、不可逆动作前明确确认、完成/失败提供后续路径。

## 现代化重制

重制为 WizardStepper：宿主声明步骤图、验证与提交命令；控件提供横向/纵向进度、保存恢复、分支跳转和焦点管理。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 controls.wizard-stepper（WizardStepper）拥有。 展项不声明公共 API、类型或资源键，不复制稳定控件；历史标签仅用于说明来源。

## 状态与失败模式

Editing、Validating、Blocked、Committing、Completed、Failed、Canceled；提交阶段禁止重复执行，允许宿主声明是否可取消。 数据规模、格式或 Provider 异常必须局部降级，不损坏已提交值、选择或宿主数据。
