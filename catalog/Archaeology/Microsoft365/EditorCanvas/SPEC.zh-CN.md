# Editor Canvas 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Word 的分页文档表面与 PowerPoint 的幻灯片编辑表面长期组合页面/画布、标尺、缩放、参考线、对象选择和缩略图导航。

## 设计基因

内容表面与工具框架分离、稳定坐标系、连续缩放、对齐参考、页面/幻灯片导航和选择驱动的上下文工具。

## 现代化重制

重制为通用编辑画布体验壳，组合缩放/平移、页面缩略图、标尺与参考线；文档模型、图形渲染、撤销和协作由宿主或未来 Canvas 模块提供。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 experiences.editor-canvas（EditorCanvasExperience）拥有。 展项不声明公共 API，不复制现代组件；历史名称只用于研究标题，演示标注“现代化重制”。

## 状态与失败模式

Browse、Select、Pan、Edit、PresentationPreview；模式切换保留缩放中心和选择，失效对象选择需原子清除。 数据、服务或依赖缺失时必须显示局部且可解释的降级，不能用假数据冒充真实账号结果。
