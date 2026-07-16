# Game Bar Widget 规范

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 原始体验

Windows 10 时代 Xbox Game Bar 的可停靠覆盖层小组件；以 Win+G 唤出，可移动、调整尺寸、固定并在游戏画面之上保持半透明。

## 设计基因

不中断前台内容的覆盖层、独立小组件窗口状态、固定/取消固定、紧凑与桌面模式，以及手柄和鼠标可连续切换的二维焦点导航。

## 现代化重制

重制为应用内的浮动小组件体验：宿主提供工作区，小组件可拖动、缩放、最小化和置顶；不得模拟系统 Game Bar，也不得注入其他进程。

## 稳定实现边界

本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 experiences.game-bar-widget（GameBarWidgetExperience）拥有。 展项不得声明公共类型、资源键或平台服务，也不得复制稳定实现；Gallery 通过依赖 ID 组合现代组件并标注 Inspired by，而非 Original。

## 状态与失败模式

窗口化、固定、非固定、紧凑、失焦和输入设备切换必须保持内容状态；宿主关闭时安全回收。 缺少依赖、数据或平台能力时显示解释性降级，不伪造成功状态；展项关闭后释放演示状态并恢复进入前焦点。
