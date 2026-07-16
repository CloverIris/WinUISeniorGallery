# MediaTimeline 验收

## 功能场景

- Given VOD `[0, 120s]`，When 位置为 30s，Then 进度与 RangeValue 均为 25%。
- Given Live 模式，When 用户点击轨道，Then 不发 Seek 且读出“直播”。
- Given Live DVR 窗口从 `[100s, 400s]` 移至 `[110s, 410s]`，When 未拖动，Then Live Edge 和相对位置无闪跳。
- Given 拖动持续 1 秒且节流 100ms，When 指针高频移动，Then 预览不超过 11 次、最终预览存在、释放只发一次 Seek。
- Given 拖动取消，Then 不发 Seek 并恢复宿主 `Position`。
- Given目标落入禁用区间，Then 按最近边界和拖动方向确定唯一有效位置。
- Given End 键位于 Live DVR，Then 发一次 Live Edge 请求；未处理时 Seek 当前 `Maximum`。
- Given `PlaybackRate=2.0`，Then 显示与 Automation 描述包含 2.0×，媒体范围、步进和 Seek 位置不发生缩放。

## 区间与边界

- 裁剪、排序并合并重叠缓冲/禁用范围；忽略零长、反向和区间外数据。
- `Maximum < Minimum`、空集合和动态替换均不抛异常。
- 窗口在拖动中移动时，释放位置按最新范围重新钳制。
- 章节和标记同位置时保持输入顺序，密集视觉聚合不丢失 Automation 信息。

## 性能

- 1000 个标记的首次布局在目标硬件 Release x64 下不阻塞 UI 超过一帧预算的连续 3 帧。
- 高频 `Position` 更新不重建区间和标记视觉；未变化集合使用引用相等快速路径。
- 60 秒拖动测试无持续内存增长、卸载后无计时器回调。

## 输入与无障碍

- 验证鼠标点击/拖动、触摸、笔、键盘和手柄步进。
- 验证 Light、Dark、High Contrast、Reduced Motion、RTL、100%–300% DPI。
- RangeValue 的 Minimum、Maximum、Value、SmallChange、LargeChange 正确；只读 Live 不暴露 SetValue。

## 自动化

单元测试归一化、禁用范围修正、节流尾值、最终提交和 Live Edge；UI 测试模板降级、ToolTip、焦点、RTL 与 AutomationPeer；测试使用虚拟时钟保证确定性。
