# Search Suggestions 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Windows 搜索从 Vista/7 的即时索引结果演进到 Windows 8–11 的分类建议、最近查询、应用/文件/设置结果与趋势入口。

## 设计基因

输入即反馈、来源分组、键盘预览选择、查询与建议提交区分、无结果也提供明确反馈。

## 现代化重制

重制为 SearchBoxEx：宿主注入可取消的多来源 Provider，控件合并分组、最近项和趋势项；不默认上传查询或访问 Windows 搜索索引。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 controls.search-box-ex（SearchBoxEx）拥有。 展项不声明公共 API、类型或资源键，不复制稳定控件；历史标签仅用于说明来源。

## 状态与失败模式

Idle、Typing、Debouncing、Loading、Results、Empty、Error；旧查询结果按版本丢弃，IME 合成中不提交搜索。 数据规模、格式或 Provider 异常必须局部降级，不损坏已提交值、选择或宿主数据。
