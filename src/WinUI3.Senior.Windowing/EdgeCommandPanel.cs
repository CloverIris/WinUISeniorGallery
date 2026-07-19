using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;
using Windows.System;

namespace WinUI3.Senior.Windowing;

/// <summary>
/// Edge-revealed command surface with deterministic snap points, keyboard handling and
/// host-neutral dismissal semantics. The panel does not create a Popup or a Window.
/// </summary>
public sealed class EdgeCommandPanel : ContentControl
{
    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
        nameof(IsOpen), typeof(bool), typeof(EdgeCommandPanel), new PropertyMetadata(false, OnIsOpenChanged));

    public static readonly DependencyProperty EdgeProperty = DependencyProperty.Register(
        nameof(Edge), typeof(EdgeCommandPanelEdge), typeof(EdgeCommandPanel), new PropertyMetadata(EdgeCommandPanelEdge.Right));

    public static readonly DependencyProperty IsModalProperty = DependencyProperty.Register(
        nameof(IsModal), typeof(bool), typeof(EdgeCommandPanel), new PropertyMetadata(true));

    public static readonly DependencyProperty DismissOnEscapeProperty = DependencyProperty.Register(
        nameof(DismissOnEscape), typeof(bool), typeof(EdgeCommandPanel), new PropertyMetadata(true));

    public static readonly DependencyProperty IsLightDismissEnabledProperty = DependencyProperty.Register(
        nameof(IsLightDismissEnabled), typeof(bool), typeof(EdgeCommandPanel), new PropertyMetadata(true));

    public static readonly DependencyProperty SnapPointProperty = DependencyProperty.Register(
        nameof(SnapPoint), typeof(double), typeof(EdgeCommandPanel), new PropertyMetadata(0d, OnSnapPointChanged));

    public static readonly DependencyProperty MaxSnapPointProperty = DependencyProperty.Register(
        nameof(MaxSnapPoint), typeof(double), typeof(EdgeCommandPanel), new PropertyMetadata(0d, OnSnapPointChanged));

    public static readonly DependencyProperty CloseThresholdProperty = DependencyProperty.Register(
        nameof(CloseThreshold), typeof(double), typeof(EdgeCommandPanel), new PropertyMetadata(0d));

    private readonly ObservableCollection<double> _snapPoints = new();
    private bool _normalizingSnapPoints;

    public EdgeCommandPanel()
    {
        _snapPoints.CollectionChanged += (_, _) => NormalizeSnapPoints();
    }

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public EdgeCommandPanelEdge Edge
    {
        get => (EdgeCommandPanelEdge)GetValue(EdgeProperty);
        set => SetValue(EdgeProperty, value);
    }

    public bool IsModal
    {
        get => (bool)GetValue(IsModalProperty);
        set => SetValue(IsModalProperty, value);
    }

    public bool DismissOnEscape
    {
        get => (bool)GetValue(DismissOnEscapeProperty);
        set => SetValue(DismissOnEscapeProperty, value);
    }

    public bool IsLightDismissEnabled
    {
        get => (bool)GetValue(IsLightDismissEnabledProperty);
        set => SetValue(IsLightDismissEnabledProperty, value);
    }

    /// <summary>Current normalized snap distance in DIPs. Zero means closed.</summary>
    public double SnapPoint
    {
        get => (double)GetValue(SnapPointProperty);
        set => SetValue(SnapPointProperty, value);
    }

    public double MaxSnapPoint
    {
        get => (double)GetValue(MaxSnapPointProperty);
        set => SetValue(MaxSnapPointProperty, Math.Max(0, value));
    }

    /// <summary>Distance below which a drag is treated as a dismissal. Defaults to 8% of max.</summary>
    public double CloseThreshold
    {
        get => (double)GetValue(CloseThresholdProperty);
        set => SetValue(CloseThresholdProperty, Math.Max(0, value));
    }

    /// <summary>Ordered snap distances in DIPs. Values are clamped to MaxSnapPoint.</summary>
    public ObservableCollection<double> SnapPoints => _snapPoints;

    public EdgeCommandPanelState State { get; private set; } = EdgeCommandPanelState.Closed;
    public int CurrentSnapIndex { get; private set; } = -1;
    public bool IsDragging => State == EdgeCommandPanelState.Dragging;

    public event EventHandler? Opened;
    public event EventHandler? Closed;
    public event EventHandler? SnapPointChanged;
    public event EventHandler? StateChanged;
    public event EventHandler<EdgeCommandPanelDismissedEventArgs>? Dismissed;

    public void Open() => IsOpen = true;

    public void Close() => Dismiss(EdgeCommandPanelDismissReason.Programmatic);

    public bool Dismiss(EdgeCommandPanelDismissReason reason)
    {
        if (!IsOpen)
        {
            return false;
        }
        if (reason is EdgeCommandPanelDismissReason.Escape && !DismissOnEscape ||
            reason is EdgeCommandPanelDismissReason.LightDismiss && !IsLightDismissEnabled)
        {
            return false;
        }
        Dismissed?.Invoke(this, new EdgeCommandPanelDismissedEventArgs(reason));
        IsOpen = false;
        return true;
    }

    public bool HandleEscape() => Dismiss(EdgeCommandPanelDismissReason.Escape);

    /// <summary>Processes keyboard snap navigation without assuming a specific visual template.</summary>
    public bool HandleKey(VirtualKey key)
    {
        if (!IsOpen && key is not VirtualKey.Enter and not VirtualKey.Space)
        {
            return false;
        }
        var previous = FlowDirection == FlowDirection.RightToLeft ? 1 : -1;
        var next = -previous;
        return key switch
        {
            VirtualKey.Escape => HandleEscape(),
            VirtualKey.Home => SnapToIndex(-1),
            VirtualKey.End => SnapToIndex(GetSnapValues().Count - 1),
            VirtualKey.Left => MoveSnap(previous),
            VirtualKey.Right => MoveSnap(next),
            VirtualKey.Up => MoveSnap(-1),
            VirtualKey.Down => MoveSnap(1),
            VirtualKey.Enter or VirtualKey.Space when !IsOpen => SetOpenFromKeyboard(),
            _ => false
        };
    }

    /// <summary>Applies an outward-positive drag delta to the current edge distance.</summary>
    public void ApplyDrag(double delta)
    {
        if (MaxSnapPoint <= 0)
        {
            return;
        }
        SetState(EdgeCommandPanelState.Dragging);
        SnapPoint = Math.Clamp(SnapPoint + delta, 0, MaxSnapPoint);
        var threshold = CloseThreshold > 0 ? CloseThreshold : Math.Max(8, MaxSnapPoint * 0.08);
        if (SnapPoint <= threshold)
        {
            Dismiss(EdgeCommandPanelDismissReason.DraggedPastThreshold);
        }
    }

    /// <summary>Converts a physical pointer delta into an outward-positive edge distance.</summary>
    public void ApplyPointerDelta(Point delta)
    {
        var outward = Edge switch
        {
            EdgeCommandPanelEdge.Left => -delta.X,
            EdgeCommandPanelEdge.Right => -delta.X,
            EdgeCommandPanelEdge.Top => -delta.Y,
            EdgeCommandPanelEdge.Bottom => -delta.Y,
            _ => 0
        };
        ApplyDrag(outward);
    }

    /// <summary>Ends an interactive drag and resolves to the nearest configured snap point.</summary>
    public void EndDrag(bool commit = true)
    {
        if (!IsDragging)
        {
            return;
        }
        if (commit)
        {
            SnapToNearest();
        }
        else
        {
            SetState(IsOpen ? EdgeCommandPanelState.Open : EdgeCommandPanelState.Closed);
        }
    }

    public void SnapToNearest()
    {
        var values = GetSnapValues();
        if (values.Count == 0)
        {
            SnapPoint = IsOpen ? MaxSnapPoint : 0;
            SetState(IsOpen ? EdgeCommandPanelState.Open : EdgeCommandPanelState.Closed);
            return;
        }
        var nearest = values.OrderBy(value => Math.Abs(value - SnapPoint)).First();
        SnapTo(nearest);
    }

    public bool SnapToIndex(int index)
    {
        var values = GetSnapValues();
        if (index < 0)
        {
            Dismiss(EdgeCommandPanelDismissReason.Programmatic);
            return true;
        }
        if (index >= values.Count)
        {
            return false;
        }
        SnapTo(values[index]);
        return true;
    }

    public void SnapTo(double distance)
    {
        SnapPoint = Math.Clamp(distance, 0, MaxSnapPoint);
        IsOpen = SnapPoint > 0;
        SetState(IsOpen ? EdgeCommandPanelState.Open : EdgeCommandPanelState.Closed);
    }

    public void SetSnapPoints(System.Collections.Generic.IEnumerable<double>? values)
    {
        _snapPoints.Clear();
        if (values is not null)
        {
            foreach (var value in values)
            {
                if (double.IsFinite(value) && value > 0)
                {
                    _snapPoints.Add(value);
                }
            }
        }
        NormalizeSnapPoints();
    }

    private bool MoveSnap(int delta)
    {
        var values = GetSnapValues();
        if (values.Count == 0)
        {
            return false;
        }
        var index = CurrentSnapIndex < 0 ? 0 : CurrentSnapIndex;
        return SnapToIndex(Math.Clamp(index + delta, 0, values.Count - 1));
    }

    private bool SetOpenFromKeyboard()
    {
        Open();
        return true;
    }

    private System.Collections.Generic.IReadOnlyList<double> GetSnapValues()
    {
        if (_snapPoints.Count > 0)
        {
            return _snapPoints;
        }
        return MaxSnapPoint > 0 ? new[] { MaxSnapPoint } : Array.Empty<double>();
    }

    private void NormalizeSnapPoints()
    {
        if (_normalizingSnapPoints)
        {
            return;
        }
        _normalizingSnapPoints = true;
        try
        {
        if (_snapPoints.Count > 1)
        {
            var max = MaxSnapPoint > 0 ? MaxSnapPoint : double.PositiveInfinity;
            var values = _snapPoints.Where(double.IsFinite).Select(value => Math.Clamp(value, 0, max)).Where(value => value > 0).Distinct().OrderBy(value => value).ToArray();
            _snapPoints.Clear();
            foreach (var value in values)
            {
                _snapPoints.Add(value);
            }
        }
        CurrentSnapIndex = GetSnapValues().ToList().FindIndex(value => Math.Abs(value - SnapPoint) < 0.5);
        }
        finally
        {
            _normalizingSnapPoints = false;
        }
    }

    private static void OnIsOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not EdgeCommandPanel panel)
        {
            return;
        }
        if ((bool)args.NewValue)
        {
            panel.SetState(EdgeCommandPanelState.Opening);
            if (panel.MaxSnapPoint > 0 && panel.SnapPoint <= 0)
            {
                panel.SnapPoint = panel.GetSnapValues().LastOrDefault(panel.MaxSnapPoint);
            }
            panel.SetState(EdgeCommandPanelState.Open);
            panel.Opened?.Invoke(panel, EventArgs.Empty);
        }
        else
        {
            panel.SnapPoint = 0;
            panel.CurrentSnapIndex = -1;
            panel.SetState(EdgeCommandPanelState.Closing);
            panel.SetState(EdgeCommandPanelState.Closed);
            panel.Closed?.Invoke(panel, EventArgs.Empty);
        }
    }

    private static void OnSnapPointChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not EdgeCommandPanel panel)
        {
            return;
        }
        var clamped = Math.Clamp(panel.SnapPoint, 0, panel.MaxSnapPoint);
        if (Math.Abs(panel.SnapPoint - clamped) > double.Epsilon)
        {
            panel.SetValue(SnapPointProperty, clamped);
            return;
        }
        panel.CurrentSnapIndex = panel.GetSnapValues().ToList().FindIndex(value => Math.Abs(value - panel.SnapPoint) < 0.5);
        panel.SnapPointChanged?.Invoke(panel, EventArgs.Empty);
    }

    private void SetState(EdgeCommandPanelState state)
    {
        if (State == state)
        {
            return;
        }
        State = state;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
