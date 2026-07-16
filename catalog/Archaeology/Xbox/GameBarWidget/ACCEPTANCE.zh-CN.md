# Game Bar Widget 验收

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 研究完整性

页面明确产品、版本/年代、原始交互、设计基因、现代改动和已知限制；事实性陈述可追溯到 SOURCES，推断明确标记为分析。

## 体验场景

两个小组件重叠时，激活只提升目标层级；切换紧凑模式后核心操作保持可达且数据不重载。

## 输入、无障碍与性能

验证 D-pad/摇杆+A/B、鼠标、键盘和触摸；焦点不得落到被遮挡的控件，固定状态仍提供退出路径。 Light、Dark、High Contrast、Reduced Motion、RTL、100%–300% DPI 均可完成核心路径。演示不得持续轮询或后台联网，关闭后无计时器、事件或窗口泄漏。

## 来源与许可

SOURCES 至少包含两个可访问来源、访问日期 2026-07-16 和证据说明；Gallery 不显示未获许可的历史素材，license_review 通过前不得升为 preview/stable。
