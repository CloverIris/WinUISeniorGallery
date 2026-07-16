# File Card 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

OneDrive、SharePoint 和 Microsoft 365 使用文件卡片/详情浮层呈现缩略图、文件名、类型、作者、修改时间、共享状态与打开/分享等快捷动作。

## 设计基因

把文件身份、预览、元数据和权限状态聚合在紧凑表面；加载渐进、动作权限感知、未知类型有一致回退。

## 现代化重制

重制为 Provider 无关 FileCard：宿主提供元数据、预览流与命令；控件不自行读取磁盘、OneDrive 或 SharePoint，也不执行危险操作。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 experiences.file-card（FileCard）拥有。 展项不声明公共 API，不复制现代组件；历史名称只用于研究标题，演示标注“现代化重制”。

## 状态与失败模式

MetadataOnly、PreviewLoading、Ready、Partial、AccessDenied、Missing；预览失败不隐藏文件名，权限变化立即撤销不可用动作。 数据、服务或依赖缺失时必须显示局部且可解释的降级，不能用假数据冒充真实账号结果。
