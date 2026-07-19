using System;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;

namespace WinUI3.Senior.Windowing;

public sealed class TabTearOutRequestedEventArgs : EventArgs
{
    public TabTearOutRequestedEventArgs(string tabId, Point pointerPosition, Point delta = default)
        => (TabId, PointerPosition, Delta) = (tabId, pointerPosition, delta);

    public string TabId { get; }
    public Point PointerPosition { get; }
    public Point Delta { get; }
    public double Distance => Math.Sqrt(Delta.X * Delta.X + Delta.Y * Delta.Y);
}

/// <summary>
/// Detects an intentional tab drag and asks the host to create a new window. It never creates
/// or migrates a window itself. The controller keeps all pointer state local to one tab item.
/// </summary>
public sealed class TabTearOutController : IDisposable
{
    private TabViewItem? _item;
    private Point _pressPoint;
    private bool _tracking;
    private bool _requested;

    public string TabId { get; set; } = string.Empty;
    public double TearOutThreshold { get; set; } = 24;
    public bool AllowVerticalTearOut { get; set; } = true;
    public bool CanTearOut { get; set; } = true;
    public TabTearOutState State { get; private set; } = TabTearOutState.Idle;

    public event EventHandler<TabTearOutRequestedEventArgs>? TearOutRequested;
    public event EventHandler<TabTearOutDragEventArgs>? DragStarted;
    public event EventHandler<TabTearOutDragEventArgs>? DragProgress;
    public event EventHandler? DragCancelled;
    public event EventHandler? StateChanged;

    public void Attach(TabViewItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (ReferenceEquals(_item, item))
        {
            return;
        }
        Detach();
        _item = item;
        _item.PointerPressed += OnPointerPressed;
        _item.PointerMoved += OnPointerMoved;
        _item.PointerReleased += OnPointerReleased;
        _item.PointerCanceled += OnPointerCanceled;
        _item.PointerCaptureLost += OnPointerCaptureLost;
    }

    public void Detach()
    {
        if (_item is null)
        {
            return;
        }
        _item.PointerPressed -= OnPointerPressed;
        _item.PointerMoved -= OnPointerMoved;
        _item.PointerReleased -= OnPointerReleased;
        _item.PointerCanceled -= OnPointerCanceled;
        _item.PointerCaptureLost -= OnPointerCaptureLost;
        _item = null;
        Reset(false);
    }

    public void Dispose() => Detach();

    /// <summary>Allows a host to request tear-out from a keyboard command or context menu.</summary>
    public bool RequestTearOut(Point pointerPosition)
    {
        if (!CanTearOut || string.IsNullOrWhiteSpace(TabId) || _requested)
        {
            return false;
        }
        _requested = true;
        SetState(TabTearOutState.Requested);
        var delta = new Point(pointerPosition.X - _pressPoint.X, pointerPosition.Y - _pressPoint.Y);
        TearOutRequested?.Invoke(this, new TabTearOutRequestedEventArgs(TabId, pointerPosition, delta));
        return true;
    }

    public void Cancel()
    {
        if (State is TabTearOutState.Idle or TabTearOutState.Cancelled)
        {
            return;
        }
        Reset(true);
    }

    /// <summary>Marks a host-accepted tear-out transaction complete and re-arms the controller.</summary>
    public void CompleteRequest()
    {
        if (State != TabTearOutState.Requested)
        {
            return;
        }
        Reset(false);
    }

    private void OnPointerPressed(object sender, PointerRoutedEventArgs args)
    {
        if (!CanTearOut || string.IsNullOrWhiteSpace(TabId))
        {
            return;
        }
        var point = args.GetCurrentPoint((TabViewItem)sender);
        if (!point.Properties.IsLeftButtonPressed)
        {
            return;
        }
        _pressPoint = point.Position;
        _tracking = true;
        _requested = false;
        SetState(TabTearOutState.Pressed);
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs args)
    {
        if (!_tracking || !CanTearOut || string.IsNullOrWhiteSpace(TabId))
        {
            return;
        }
        var item = (TabViewItem)sender;
        var point = args.GetCurrentPoint(item);
        if (!point.Properties.IsLeftButtonPressed)
        {
            return;
        }
        var delta = new Point(point.Position.X - _pressPoint.X, point.Position.Y - _pressPoint.Y);
        var distance = Math.Sqrt(delta.X * delta.X + delta.Y * delta.Y);
        if (!AllowVerticalTearOut && Math.Abs(delta.X) < Math.Abs(delta.Y))
        {
            DragProgress?.Invoke(this, new TabTearOutDragEventArgs(TabId, point.Position, distance));
            return;
        }
        if (distance < Math.Max(4, TearOutThreshold))
        {
            return;
        }
        if (State == TabTearOutState.Pressed)
        {
            SetState(TabTearOutState.Dragging);
            DragStarted?.Invoke(this, new TabTearOutDragEventArgs(TabId, point.Position, distance));
        }
        DragProgress?.Invoke(this, new TabTearOutDragEventArgs(TabId, point.Position, distance));
        _tracking = false;
        RequestTearOut(point.Position);
    }

    private void OnPointerReleased(object sender, PointerRoutedEventArgs args)
    {
        if (!_requested)
        {
            Reset(false);
        }
        else
        {
            _tracking = false;
        }
    }

    private void OnPointerCanceled(object sender, PointerRoutedEventArgs args) => Cancel();
    private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs args) => Cancel();

    private void Reset(bool notifyCancellation)
    {
        _tracking = false;
        _requested = false;
        if (notifyCancellation)
        {
            SetState(TabTearOutState.Cancelled);
            DragCancelled?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            SetState(TabTearOutState.Idle);
        }
    }

    private void SetState(TabTearOutState state)
    {
        if (State == state)
        {
            return;
        }
        State = state;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
