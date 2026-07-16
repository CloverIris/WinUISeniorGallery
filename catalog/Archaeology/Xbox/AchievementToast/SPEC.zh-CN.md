# Achievement Toast 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Xbox 360 建立的成就解锁通知模式：短促音画提示、图标、名称和分值在游戏之上出现，随后自动退出，不夺取操作焦点。

## 设计基因

稀有事件的高识别度反馈、短时非模态呈现、严格队列化、一次只突出一个成就，以及可在详情页追踪累计进度。

## 现代化重制

重制为通用成就 Toast：宿主提交本地化标题、图标、进度和稀有度；支持队列、合并、超时与无动画模式，不复制 Xbox 声音或图形。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 controls.achievement-toast（AchievementToast）拥有。 展项不得声明公共类型、资源键或平台服务，也不得复制稳定实现；Gallery 通过依赖 ID 组合现代组件并标注 Inspired by，而非 Original。

## 状态与失败模式

Queued、Entering、Visible、Exiting、Dismissed；重复 ID 只更新尚未显示项，已显示项不重新播放。 缺少依赖、数据或平台能力时显示解释性降级，不伪造成功状态；展项关闭后释放演示状态并恢复进入前焦点。
