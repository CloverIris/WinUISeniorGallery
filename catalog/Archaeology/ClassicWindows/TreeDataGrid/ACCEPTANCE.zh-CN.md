# Tree Data Grid 验收

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 研究完整性

页面明确产品/模式、版本或年代、原始体验、设计基因、现代改动和限制；事实链接到 SOURCES，综合推断明确标为项目分析。

## 体验场景

100,000 个逻辑节点只实现视口行；展开异步分支后排序请求保留节点 ID 选择，加载失败分支可单独重试且其他展开状态不变。

## 输入、无障碍与性能

左右键折叠/展开或进入层级，上下键逐可见行，Home/End/Page 移动；屏幕阅读器获得行列索引、层级、展开和选择状态。 Light、Dark、High Contrast、Reduced Motion、RTL、100%–300% DPI 和 200% 文本缩放通过。关闭页面后无 Provider、事件、计时器或窗口泄漏。

## 来源与许可

至少两个来源记录访问日期 2026-07-16 与证据用途；不得提交历史资产，license_review 通过前不得升为 preview/stable。
