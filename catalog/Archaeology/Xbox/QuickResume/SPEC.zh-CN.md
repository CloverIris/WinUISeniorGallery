# Quick Resume 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Xbox Series X|S 的多游戏挂起与快速恢复能力；系统保存多个运行状态，用户从分组或最近列表选择后返回原游戏位置。

## 设计基因

可见的可恢复状态、最近使用排序、状态缩略信息、快速切换和明确的不可恢复降级，而非普通应用启动器。

## 现代化重制

重制为应用内部的会话恢复选择器，卡片展示快照、保存时间和恢复能力；只恢复宿主提供的序列化状态，不承诺进程或游戏内存快照。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 experiences.quick-resume（QuickResumeExperience）拥有。 展项不得声明公共类型、资源键或平台服务，也不得复制稳定实现；Gallery 通过依赖 ID 组合现代组件并标注 Inspired by，而非 Original。

## 状态与失败模式

Available、Restoring、Unavailable、Expired、Failed；恢复必须可取消，失败保留卡片并显示可重试或重新开始。 缺少依赖、数据或平台能力时显示解释性降级，不伪造成功状态；展项关闭后释放演示状态并恢复进入前焦点。
