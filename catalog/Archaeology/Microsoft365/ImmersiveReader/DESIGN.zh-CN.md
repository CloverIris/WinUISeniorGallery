# Immersive Reader 设计

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 保留

去除干扰、逐步披露阅读辅助、用户可控排版、视觉与语音同步、对阅读障碍和语言学习者友好。

## 现代化

重制为本地优先的专注阅读壳：接受结构化文本并提供列宽、字号、间距、行聚焦和朗读位置；语音、翻译、词典和图片服务经独立 Provider 接入。 使用当前 Fluent ThemeResource 和系统排版，不像素复刻历史 UI。

## 响应式与输入

完整键盘/触摸/鼠标操作，朗读具有播放、暂停和速度控制；焦点顺序先正文后辅助工具，行聚焦不能遮挡当前 Automation 阅读范围。 窄屏采用折叠/分层，宽屏限制正文宽度；200% 文本缩放不裁切关键内容。

## 动效与无障碍

动效只说明上下文、层级或加载变化，Reduced Motion 即时切换。High Contrast 不依赖阴影/透明度，Automation 暴露角色、状态、来源和下一动作。
