# People Card 设计

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 保留

在不离开上下文时补充身份信息、渐进披露、统一身份与状态、动作紧邻对象、权限不足时逐节降级。

## 现代化

重制为数据源无关 PeopleCard：宿主提供最小人物模型和按需 section Provider；默认只展示本地数据，不自行访问 Microsoft Graph。 使用当前 Fluent ThemeResource 和系统排版，不像素复刻历史 UI。

## 响应式与输入

Hover 仅预览，键盘焦点或点击可固定；Esc 关闭并恢复触发元素；触摸没有 hover 依赖，状态和头像均有文本替代。 窄屏采用折叠/分层，宽屏限制正文宽度；200% 文本缩放不裁切关键内容。

## 动效与无障碍

动效只说明上下文、层级或加载变化，Reduced Motion 即时切换。High Contrast 不依赖阴影/透明度，Automation 暴露角色、状态、来源和下一动作。
