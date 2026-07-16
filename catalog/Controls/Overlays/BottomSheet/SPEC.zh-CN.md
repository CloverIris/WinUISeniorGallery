# BottomSheet 规格

## 目标与非目标

提供可放入任意窗口内容根部的单 Sheet 宿主，支持模态/非模态、像素/比例/内容高度吸附点、拖拽、遮罩关闭、Esc、焦点约束及窄/宽窗口呈现回退。不实现应用路由、跨窗口迁移、嵌套 Sheet 或系统级触摸边缘手势。

## 公共 API

```csharp
public enum BottomSheetModality { Modal, Modeless }
public enum BottomSheetPlacement { Auto, Bottom, Side, Center }
public enum BottomSheetDismissReason { Programmatic, Drag, Scrim, Escape, Back, HostUnloaded }
public enum BottomSheetSnapPointKind { Pixels, AvailableFraction, Content }

public sealed record BottomSheetSnapPoint(string Id, BottomSheetSnapPointKind Kind, double Value);

public sealed class BottomSheet : ContentControl
{
    public bool IsOpen { get; set; }
    public BottomSheetModality Modality { get; set; } // Modal
    public BottomSheetPlacement Placement { get; set; } // Auto
    public IReadOnlyList<BottomSheetSnapPoint> SnapPoints { get; set; }
    public string? ActiveSnapPointId { get; set; }
    public bool IsDragEnabled { get; set; } // true
    public bool IsDismissOnScrimEnabled { get; set; } // true when Modal
    public bool IsDismissOnEscapeEnabled { get; set; } // true
    public bool IsFocusTrapEnabled { get; set; } // true when Modal
    public double WideModeThreshold { get; set; } // 720
    public BottomSheetPlacement WideModePlacement { get; set; } // Side
    public event EventHandler<BottomSheetOpeningEventArgs> Opening;
    public event EventHandler<EventArgs> Opened;
    public event EventHandler<BottomSheetClosingEventArgs> Closing;
    public event EventHandler<BottomSheetClosedEventArgs> Closed;
    public event EventHandler<BottomSheetSnapPointChangedEventArgs> SnapPointChanged;
    public void Open(string? snapPointId = null);
    public void Close(BottomSheetDismissReason reason = Programmatic);
    public void SnapTo(string snapPointId);
}
```

`Opening` 和 `Closing` 可取消；取消后恢复原状态。`Closing` 提供原因。默认 SnapPoints 为 50% 与内容高度中较小者的 `Content` 点。ID 必须非空且唯一；像素值大于 0、比例值在 `(0,1]`，否则赋值时抛 `ArgumentException`。`SnapTo` 未知 ID 抛 `ArgumentException`。

## 状态与布局

生命周期：`Closed → Opening → Open → Snapping/Dragging → Closing → Closed`。动画可中断：Opening 中 Close 直接从当前视觉值 Closing；Closing 中 Open 同理反向。一个时刻只触发一个终态事件。

`Auto` 在可用宽度小于 `WideModeThreshold` 时为 Bottom；宽窗口使用 `WideModePlacement`。Side 遵循流向（LTR 右侧、RTL 左侧）；Center 为对话框式居中。窗口变窄时保持逻辑 SnapPoint ID，并在新布局中钳制到安全区域。

吸附高度按可用区域计算并钳制在最小 64px 与安全区域最大值之间。拖动释放时，速度超过 800px/s 选择速度方向的相邻点；否则选择距离最近点。低于最小点再拖 25% 或速度向关闭方向超过 1000px/s 时关闭。

## 模板部件

- `PART_Root`：覆盖层根。
- `PART_Scrim`：遮罩。
- `PART_Surface`：Sheet 表面和变换目标。
- `PART_DragHandle`：可选拖动把手。
- `PART_ContentPresenter`：内容宿主。
- `PART_FocusSentinelStart`、`PART_FocusSentinelEnd`：模态焦点循环。

VisualStateGroups：`OpenStates`、`PlacementStates`、`ModalityStates`、`InteractionStates`。缺少 Surface 或 ContentPresenter 抛模板异常；其余缺失安全降级。

## 焦点与关闭行为

模态打开后保存当前窗口焦点，焦点进入首个可聚焦子项，否则落在 Surface；Tab 在 Sheet 内循环，背景从 UIA 树的交互视图中暂时排除。关闭后若原元素仍可用则恢复，否则回到宿主。非模态不锁焦点、不隐藏背景，点击外部默认不关闭。

Esc 仅关闭顶层已打开 BottomSheet，并先让内部控件处理；取消 Closing 时事件视为已处理。宿主卸载强制以 `HostUnloaded` 关闭且不可取消，释放指针捕获和焦点哨兵。
