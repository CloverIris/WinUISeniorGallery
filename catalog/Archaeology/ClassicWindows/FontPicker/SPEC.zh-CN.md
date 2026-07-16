# Font Picker 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Win32 ChooseFont 和 Office 字体下拉长期提供字体家族、样式、字号、脚本与实时预览，并逐渐加入最近使用、主题字体和搜索。

## 设计基因

字体名以自身外观预览、家族/字形/字号分层、系统字体枚举、缺失字体回退、键盘快速定位和最近使用。

## 现代化重制

重制为虚拟化 FontPicker：宿主提供字体目录与许可信息，控件负责搜索、预览、收藏/最近和回退说明；不捆绑或上传系统字体。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 controls.font-picker（FontPicker）拥有。 展项不声明公共 API、类型或资源键，不复制稳定控件；历史标签仅用于说明来源。

## 状态与失败模式

Enumerating、Ready、Filtering、Previewing、Missing、Restricted；字体加载失败回退系统 UI 字体并保留原名称警告。 数据规模、格式或 Provider 异常必须局部降级，不损坏已提交值、选择或宿主数据。
