# CarouselView 规格

## 目标与非目标

控件必须在 1000 个逻辑项时保持有限虚拟化，统一处理鼠标、滚轮、键盘、触摸、精确触控板和游戏手柄，并提供 `Slide`、`Fade`、`CoverFlow` 转场。它不负责数据拉取、联网、图像缓存、页面导航、媒体播放或历史 Cover Flow 资产复刻。

## 公共 API

```csharp
public enum CarouselTransition { Slide, Fade, CoverFlow }
public enum CarouselNavigationMode { Bounded, Loop }
public enum CarouselAutoplayPauseReason { None, PointerOver, KeyboardFocusWithin, WindowInactive, UserInteraction, NotVisible }
public enum CarouselInputDeviceKind { Unknown, Mouse, Touch, Keyboard, GameController, PrecisionTouchpad }

public sealed class CarouselView : ListView
{
    public CarouselNavigationMode NavigationMode { get; set; } // Bounded
    public CarouselTransition Transition { get; set; } // Slide
    public bool IsAutoplayEnabled { get; set; } // false
    public bool IsHostWindowActive { get; set; } // true; host supplied
    public TimeSpan AutoplayInterval { get; set; } // 5 s; minimum 1 s
    public bool PauseAutoplayOnPointerOver { get; set; } // true
    public bool PauseAutoplayOnKeyboardFocusWithin { get; set; } // true
    public int RealizationBuffer { get; set; } // 1; range 0..3
    public bool IsAdjacentPreviewEnabled { get; set; } // false
    public double AdjacentPreviewExtent { get; set; } // 48 effective px; non-negative
    public bool IsSwipeEnabled { get; set; } // true
    public ICommand? ItemInvokedCommand { get; set; }
    public CarouselAutoplayPauseReason AutoplayPauseReason { get; }
    public int RealizedElementCount { get; } // Lab 诊断，不是布局保证
    public new object? SelectedItem { get; }
    public event EventHandler<CarouselItemInvokedEventArgs> ItemInvoked;
    public void MoveNext();
    public void MovePrevious();
    public void MoveTo(int index);
}
```

继承的 `ItemsSource`、`ItemTemplate`、`ItemTemplateSelector`、`SelectedIndex`、`SelectionChanged` 与 `IsSwipeEnabled` 保持 `ListView`/`Selector` 语义。属性和方法只能在 UI 线程访问。`CarouselItemInvokedEventArgs` 固定公开 `Item`、`Index`、`InputDeviceKind`；不能可靠区分精确触控板时必须报告 `Unknown`。

`MoveTo` 对空集合不操作；非空集合越界时抛出 `ArgumentOutOfRangeException`。`AutoplayInterval < 1s`、`RealizationBuffer` 不在 0..3、或负 `AdjacentPreviewExtent` 均抛出 `ArgumentOutOfRangeException`。`IsHostWindowActive` 默认 `true`，仅由宿主写入，不会查询、激活或创建窗口。

## 状态模型

状态包括 `Empty`、`Idle`、`Interacting`、`Settling`、`AutoplayWaiting`、`Suspended`。自动播放暂停优先级固定为 `NotVisible > WindowInactive > KeyboardFocusWithin > PointerOver > UserInteraction > None`。任何暂停结束或用户导航都会停止计时器，并在完整 `AutoplayInterval` 后才允许下一次移动。

`ItemsSource`、`ItemTemplate` 和 `ItemTemplateSelector` 的运行时替换必须同步到内部 `ItemsRepeater`，但不得枚举或复制全部数据。集合变更优先按对象身份保留选中项；该对象已移除时选择同一位置的后继项，若删除末项则选择新末项；空集合选择 `-1`，非空且未选择时选择 `0`。

## 模板部件与视觉树

- `PART_Root`：根 `Grid`。
- `PART_Repeater`：必需的 `ItemsRepeater` 内容宿主。
- `PART_PreviousButton`、`PART_NextButton`：可选导航按钮。
- `PART_IndicatorPresenter`：可选指示器宿主。
- `PART_LiveRegion`：可选屏幕阅读器公告元素。

模板必须提供 `CommonStates`（`Normal`、`PointerOver`、`Disabled`）、`NavigationStates`（`CanPrevious`、`CanNext`、`SingleItem`）和 `InteractionStates`（`Idle`、`Dragging`、`Settling`）。缺失可选部件仅降级；缺失 `PART_Repeater` 抛出明确的 `InvalidOperationException`。

## 行为与失败模式

左右键和手柄 D-pad 按视觉方向导航；RTL 镜像方向。Home/End 到首末项；Enter/Space/手柄 A 调用选中项。水平滚轮或 Shift+垂直滚轮导航，普通垂直滚轮冒泡。拖拽超过宽度 20% 或释放速度超过 500 有效像素/秒才切换；否则回弹。拖拽、动画和 settle 期间是 `UserInteraction`。

Reduced Motion 或 Composition 不可用时，`Slide`/`CoverFlow` 降级为不超过 100ms 的淡入或无动画。新导航中断已有动画并从当前视觉值收敛。自动播放不得触发 Live Region 公告；仅用户触发的选择变化公告。`ItemsSource` 枚举失败时传播原始异常。
