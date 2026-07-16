# Guide Menu 集成

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 依赖方向

依赖 experiences.guide-menu，仅从 Archaeology/Gallery 指向稳定模块，稳定模块不得反向引用本展项。本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 experiences.guide-menu（GuideMenuExperience）拥有。

## 数据与平台

演示数据来自本地、匿名、可重复的假数据；不连接 Xbox 服务、账号、成就或游戏进程。若现代实现使用窗口或输入平台 API，权限和生命周期由宿主负责。

## 全局契约

使用 Theme、Motion、Input、Accessibility、Navigation、Localization 和 Resources Contract。历史名称只用于页面标题；Automation 名称与命令采用现代通用语义。

## 降级与版权

依赖不可用时显示静态结构说明。所有演示图标、声音、头像和背景必须是仓库自有或许可兼容资产；不得从 Xbox 产品提取素材，发布前保持 license_review=pending。
