# Immersive Reader 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

OneNote Learning Tools 发展出的沉浸式阅读体验，后来进入 Word、OneNote、Edge 与 Azure：隔离正文、朗读高亮、行聚焦、词性/音节和翻译辅助理解。

## 设计基因

去除干扰、逐步披露阅读辅助、用户可控排版、视觉与语音同步、对阅读障碍和语言学习者友好。

## 现代化重制

重制为本地优先的专注阅读壳：接受结构化文本并提供列宽、字号、间距、行聚焦和朗读位置；语音、翻译、词典和图片服务经独立 Provider 接入。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 experiences.immersive-reader（ImmersiveReaderExperience）拥有。 展项不声明公共 API，不复制现代组件；历史名称只用于研究标题，演示标注“现代化重制”。

## 状态与失败模式

Reading、Paused、LineFocus、PreferencesOpen、ServiceUnavailable；云能力失败不移除本地正文或用户排版设置。 数据、服务或依赖缺失时必须显示局部且可解释的降级，不能用假数据冒充真实账号结果。
