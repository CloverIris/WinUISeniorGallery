# Icon Picker 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Windows Shell 的“更改图标”对话框从文件/资源中枚举图标；现代 Windows 又通过 Segoe MDL2/Fluent Icons 提供可搜索的统一符号集合。

## 设计基因

大集合网格浏览、当前选择明确、来源路径可切换、图标预览与索引分离、键盘快速定位。

## 现代化重制

重制为安全 IconPicker：支持应用注册的 Symbol/IconSource 目录、搜索、分类、收藏和 RTL 元数据；默认不扫描任意 DLL/EXE 或提取系统资源。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 controls.icon-picker（IconPicker）拥有。 展项不声明公共 API、类型或资源键，不复制稳定控件；历史标签仅用于说明来源。

## 状态与失败模式

LoadingCatalog、Ready、Filtering、Empty、SourceUnavailable、Selected；来源撤销时清除预览并保留可解释的选择描述。 数据规模、格式或 Provider 异常必须局部降级，不损坏已提交值、选择或宿主数据。
