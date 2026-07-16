# Command Ribbon 验收

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 研究完整性

页面列明产品、版本/年代、原始体验、设计基因、现代改动和限制；事实可追溯到 SOURCES，项目推断明确标为分析。

## 体验场景

选中图片时出现上下文选项卡，取消选择时安全移除；宽度从 1200 降到 480 epx 时按声明优先级折叠且所有命令仍在溢出菜单可达。

## 输入、无障碍与性能

支持鼠标、触摸、键盘 KeyTip、方向键和屏幕阅读器分组导航；常用命令在 200% 文本缩放下仍可达。 Light、Dark、High Contrast、Reduced Motion、RTL、100%–300% DPI 和 200% 文本缩放通过。演示关闭后无事件、请求、计时器或窗口泄漏。

## 来源与许可

至少两个来源均记录访问日期 2026-07-16；不得把 Microsoft Graph Toolkit 或 Azure 示例代码/资产复制进展项，license_review 通过前不升为 preview/stable。
