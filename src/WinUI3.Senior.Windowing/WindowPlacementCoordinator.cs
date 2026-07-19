using System;
using Microsoft.UI.Xaml;
using Windows.Foundation;

namespace WinUI3.Senior.Windowing;

/// <summary>Window-local placement mode. The coordinator never calls AppWindow or moves a real HWND.</summary>
public enum WindowPlacementMode
{
    Normal,
    Maximized,
    FullScreen
}

public enum WindowSnapTarget
{
    LeftHalf,
    RightHalf,
    TopHalf,
    BottomHalf,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center
}

public enum WindowResizeEdge
{
    Left,
    Right,
    Top,
    Bottom,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight
}

public sealed record WindowPlacementSnapshot(
    Rect Bounds,
    Rect RestoreBounds,
    WindowPlacementMode Mode,
    long Revision);

/// <summary>
/// Pure, host-neutral placement state machine for snap previews and custom chrome. A host can
/// consume <see cref="Snapshot"/> and apply it to AppWindow, while this type remains deterministic
/// and safe to use in a Gallery or an embedded panel.
/// </summary>
public sealed class WindowPlacementCoordinator
{
    private Rect _workArea;
    private Rect _bounds;
    private Rect _restoreBounds;
    private Size _minimumSize = new(160, 90);
    private Size _maximumSize = new(double.PositiveInfinity, double.PositiveInfinity);
    private long _revision;
    private WindowPlacementMode _publishedMode = WindowPlacementMode.Normal;

    public WindowPlacementCoordinator(Rect initialBounds, Rect workArea)
    {
        _workArea = NormalizeRect(workArea);
        _bounds = ClampRect(NormalizeRect(initialBounds));
        _restoreBounds = _bounds;
        Snapshot = CreateSnapshot();
    }

    public Rect WorkArea => _workArea;
    public Rect Bounds => _bounds;
    public Rect RestoreBounds => _restoreBounds;
    public WindowPlacementMode Mode { get; private set; } = WindowPlacementMode.Normal;
    public WindowPlacementSnapshot Snapshot { get; private set; }
    public Size MinimumSize
    {
        get => _minimumSize;
        set
        {
            _minimumSize = NormalizeSize(value, new Size(1, 1));
            _minimumSize = new Size(Math.Min(_minimumSize.Width, _maximumSize.Width), Math.Min(_minimumSize.Height, _maximumSize.Height));
            _bounds = ClampRect(_bounds);
            Publish();
        }
    }

    public Size MaximumSize
    {
        get => _maximumSize;
        set
        {
            _maximumSize = NormalizeSize(value, new Size(double.PositiveInfinity, double.PositiveInfinity));
            _maximumSize = new Size(Math.Max(_maximumSize.Width, _minimumSize.Width), Math.Max(_maximumSize.Height, _minimumSize.Height));
            _bounds = ClampRect(_bounds);
            Publish();
        }
    }

    public event EventHandler? BoundsChanged;
    public event EventHandler? ModeChanged;

    public void SetWorkArea(Rect workArea)
    {
        _workArea = NormalizeRect(workArea);
        _bounds = ClampRect(_bounds);
        if (Mode == WindowPlacementMode.Maximized)
        {
            _bounds = _workArea;
        }
        Publish();
    }

    public void SetBounds(Rect bounds, bool updateRestoreBounds = true)
    {
        var normalized = ClampRect(NormalizeRect(bounds));
        if (Mode != WindowPlacementMode.Normal)
        {
            if (updateRestoreBounds)
            {
                _restoreBounds = normalized;
                Publish();
            }
            return;
        }
        _bounds = normalized;
        if (updateRestoreBounds)
        {
            _restoreBounds = normalized;
        }
        Publish();
    }

    public void Move(Point delta)
    {
        if (!double.IsFinite(delta.X) || !double.IsFinite(delta.Y) || Mode != WindowPlacementMode.Normal)
        {
            return;
        }
        SetBounds(new Rect(_bounds.X + delta.X, _bounds.Y + delta.Y, _bounds.Width, _bounds.Height));
    }

    public void Resize(WindowResizeEdge edge, Point delta)
    {
        if (Mode != WindowPlacementMode.Normal || !double.IsFinite(delta.X) || !double.IsFinite(delta.Y))
        {
            return;
        }
        var left = _bounds.X;
        var top = _bounds.Y;
        var right = _bounds.X + _bounds.Width;
        var bottom = _bounds.Y + _bounds.Height;
        if (edge is WindowResizeEdge.Left or WindowResizeEdge.TopLeft or WindowResizeEdge.BottomLeft)
        {
            left += delta.X;
        }
        if (edge is WindowResizeEdge.Right or WindowResizeEdge.TopRight or WindowResizeEdge.BottomRight)
        {
            right += delta.X;
        }
        if (edge is WindowResizeEdge.Top or WindowResizeEdge.TopLeft or WindowResizeEdge.TopRight)
        {
            top += delta.Y;
        }
        if (edge is WindowResizeEdge.Bottom or WindowResizeEdge.BottomLeft or WindowResizeEdge.BottomRight)
        {
            bottom += delta.Y;
        }
        SetBounds(new Rect(left, top, right - left, bottom - top));
    }

    public void Snap(WindowSnapTarget target)
    {
        var halfWidth = _workArea.Width / 2;
        var halfHeight = _workArea.Height / 2;
        var quarterWidth = _workArea.Width / 2;
        var quarterHeight = _workArea.Height / 2;
        var rect = target switch
        {
            WindowSnapTarget.LeftHalf => new Rect(_workArea.X, _workArea.Y, halfWidth, _workArea.Height),
            WindowSnapTarget.RightHalf => new Rect(_workArea.X + halfWidth, _workArea.Y, halfWidth, _workArea.Height),
            WindowSnapTarget.TopHalf => new Rect(_workArea.X, _workArea.Y, _workArea.Width, halfHeight),
            WindowSnapTarget.BottomHalf => new Rect(_workArea.X, _workArea.Y + halfHeight, _workArea.Width, halfHeight),
            WindowSnapTarget.TopLeft => new Rect(_workArea.X, _workArea.Y, quarterWidth, quarterHeight),
            WindowSnapTarget.TopRight => new Rect(_workArea.X + quarterWidth, _workArea.Y, quarterWidth, quarterHeight),
            WindowSnapTarget.BottomLeft => new Rect(_workArea.X, _workArea.Y + quarterHeight, quarterWidth, quarterHeight),
            WindowSnapTarget.BottomRight => new Rect(_workArea.X + quarterWidth, _workArea.Y + quarterHeight, quarterWidth, quarterHeight),
            _ => new Rect(_workArea.X + _workArea.Width * .125, _workArea.Y + _workArea.Height * .125, _workArea.Width * .75, _workArea.Height * .75)
        };
        _restoreBounds = _bounds;
        _bounds = ClampRect(rect);
        SetMode(WindowPlacementMode.Normal);
        Publish();
    }

    public void Maximize()
    {
        if (Mode == WindowPlacementMode.Maximized)
        {
            return;
        }
        _restoreBounds = _bounds;
        _bounds = _workArea;
        SetMode(WindowPlacementMode.Maximized);
        Publish();
    }

    public void EnterFullScreen()
    {
        if (Mode == WindowPlacementMode.FullScreen)
        {
            return;
        }
        _restoreBounds = _bounds;
        _bounds = _workArea;
        SetMode(WindowPlacementMode.FullScreen);
        Publish();
    }

    public void Restore()
    {
        if (Mode == WindowPlacementMode.Normal)
        {
            return;
        }
        _bounds = ClampRect(_restoreBounds);
        SetMode(WindowPlacementMode.Normal);
        Publish();
    }

    private void SetMode(WindowPlacementMode mode)
    {
        if (Mode == mode)
        {
            return;
        }
        Mode = mode;
    }

    private void Publish()
    {
        var modeChanged = _publishedMode != Mode;
        _revision++;
        Snapshot = CreateSnapshot();
        BoundsChanged?.Invoke(this, EventArgs.Empty);
        if (modeChanged)
        {
            _publishedMode = Mode;
            ModeChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private WindowPlacementSnapshot CreateSnapshot() => new(_bounds, _restoreBounds, Mode, _revision);

    private Rect ClampRect(Rect rect)
    {
        var maxWidth = Math.Max(_minimumSize.Width, Math.Min(_maximumSize.Width, _workArea.Width));
        var maxHeight = Math.Max(_minimumSize.Height, Math.Min(_maximumSize.Height, _workArea.Height));
        var width = Math.Clamp(rect.Width, _minimumSize.Width, maxWidth);
        var height = Math.Clamp(rect.Height, _minimumSize.Height, maxHeight);
        var x = Math.Clamp(rect.X, _workArea.X, Math.Max(_workArea.X, _workArea.X + _workArea.Width - width));
        var y = Math.Clamp(rect.Y, _workArea.Y, Math.Max(_workArea.Y, _workArea.Y + _workArea.Height - height));
        return new Rect(x, y, width, height);
    }

    private static Rect NormalizeRect(Rect value)
        => new(double.IsFinite(value.X) ? value.X : 0, double.IsFinite(value.Y) ? value.Y : 0, Math.Max(1, double.IsFinite(value.Width) ? value.Width : 1), Math.Max(1, double.IsFinite(value.Height) ? value.Height : 1));

    private static Size NormalizeSize(Size value, Size fallback)
        => new(double.IsFinite(value.Width) && value.Width > 0 ? value.Width : fallback.Width, double.IsFinite(value.Height) && value.Height > 0 ? value.Height : fallback.Height);
}
