# Search Suggestions 验收

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 研究完整性

页面明确产品/模式、版本或年代、原始体验、设计基因、现代改动和限制；事实链接到 SOURCES，综合推断明确标为项目分析。

## 体验场景

快速输入 abc 时只呈现最终查询的结果，取消的 a/ab 不闪回；空结果显示单行说明，IME 候选确认前不调用 Provider。

## 输入、无障碍与性能

上下键浏览、Enter 提交、Esc 关闭建议/清空分层处理；触摸和屏幕阅读器显示类别、来源与结果类型。 Light、Dark、High Contrast、Reduced Motion、RTL、100%–300% DPI 和 200% 文本缩放通过。关闭页面后无 Provider、事件、计时器或窗口泄漏。

## 来源与许可

至少两个来源记录访问日期 2026-07-16 与证据用途；不得提交历史资产，license_review 通过前不得升为 preview/stable。
