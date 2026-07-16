# MediaCenterGrid 验收

## 当前门禁

保持 proposed，不实现 .cs/.xaml；SPEC 门禁完成且双语同步后才可 ready。

## Given / When / Then

Given 10,000 项且焦点在 ID 42，When 刷新并移动该项，Then 仅实现缓冲范围、焦点仍为 42 并滚入安全区。Given 图片失败，Then 标题、调用和焦点仍可用。

## 输入、焦点与生命周期

手柄/D-pad 优先，A/Enter 调用、B/Esc 返回；鼠标/滚轮/触摸/键盘等价。焦点不进入未实现项，Automation 读行列、标题和操作。 覆盖关闭、卸载、取消、替换、DPI/显示器变化；关闭后无焦点陷阱、迟到事件或泄漏。

## 性能预算

10,000 逻辑项只实现视口及前后各一屏；60 Hz、UI 每帧小于 4 ms、焦点反馈小于 50 ms，取消图片不迟到绑定。

## 自动化矩阵

架构测 ID/依赖/headings；单元测状态/取消/宿主拒绝；UI 测主题、Reduced Motion、多输入、Narrator、RTL、100%–300% DPI、200% 文本。

## Ready 晋级验收

须有确定性假宿主/虚拟时钟，失败可观察，Automation/性能可重复；否则仍 proposed。

