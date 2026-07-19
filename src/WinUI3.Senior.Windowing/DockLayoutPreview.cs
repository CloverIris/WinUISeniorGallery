using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace WinUI3.Senior.Windowing;

public enum DockLayoutKind
{
    LeftHalf,
    RightHalf,
    TopHalf,
    BottomHalf,
    ThreeColumnLeft,
    ThreeColumnCenter,
    ThreeColumnRight,
    FourQuadrants
}

public sealed record DockLayoutZone(string Id, double X, double Y, double Width, double Height);

public sealed class DockLayoutDragEventArgs : EventArgs
{
    public DockLayoutDragEventArgs(Point pointerPosition, DockLayoutZone? zone, bool isCommitted)
        => (PointerPosition, Zone, IsCommitted) = (pointerPosition, zone, isCommitted);

    public Point PointerPosition { get; }
    public DockLayoutZone? Zone { get; }
    public bool IsCommitted { get; }
}

/// <summary>Pure layout model used by snap/docking presenters; applying a layout stays host-owned.</summary>
public sealed class DockLayoutPreview : ContentControl
{
    public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register(
        nameof(IsOpen), typeof(bool), typeof(DockLayoutPreview), new PropertyMetadata(false));

    public static readonly DependencyProperty SelectedLayoutProperty = DependencyProperty.Register(
        nameof(SelectedLayout), typeof(DockLayoutKind?), typeof(DockLayoutPreview), new PropertyMetadata(null, OnLayoutChanged));

    public static readonly DependencyProperty FocusedZoneIdProperty = DependencyProperty.Register(
        nameof(FocusedZoneId), typeof(string), typeof(DockLayoutPreview), new PropertyMetadata(null));

    public static readonly DependencyProperty LayoutIdProperty = DependencyProperty.Register(
        nameof(LayoutId), typeof(string), typeof(DockLayoutPreview), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty PreviewBoundsProperty = DependencyProperty.Register(
        nameof(PreviewBounds), typeof(Rect), typeof(DockLayoutPreview), new PropertyMetadata(default(Rect)));

    public DockLayoutPreview()
    {
        Zones = new ObservableCollection<DockLayoutZone>();
    }

    public bool IsOpen
    {
        get => (bool)GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public DockLayoutKind? SelectedLayout
    {
        get => (DockLayoutKind?)GetValue(SelectedLayoutProperty);
        set => SetValue(SelectedLayoutProperty, value);
    }

    public string? FocusedZoneId
    {
        get => (string?)GetValue(FocusedZoneIdProperty);
        set => SetValue(FocusedZoneIdProperty, value);
    }

    /// <summary>Stable host-facing layout identifier, suitable for persistence or telemetry-free diagnostics.</summary>
    public string LayoutId
    {
        get => (string)GetValue(LayoutIdProperty);
        private set => SetValue(LayoutIdProperty, value);
    }

    /// <summary>Bounds used to translate normalized zones into presenter coordinates.</summary>
    public Rect PreviewBounds
    {
        get => (Rect)GetValue(PreviewBoundsProperty);
        set => SetValue(PreviewBoundsProperty, value);
    }

    public ObservableCollection<DockLayoutZone> Zones { get; }
    public DockLayoutZone? FocusedZone => Zones.FirstOrDefault(zone => string.Equals(zone.Id, FocusedZoneId, StringComparison.Ordinal));
    public int ZoneCount => Zones.Count;
    public bool IsDragging { get; private set; }
    public event EventHandler? LayoutChanged;
    public event EventHandler? LayoutCommitted;
    public event EventHandler<DockLayoutCommitRequestedEventArgs>? CommitRequested;
    public event EventHandler<DockLayoutFocusChangedEventArgs>? FocusChanged;
    public event EventHandler<DockLayoutDragEventArgs>? DragStarted;
    public event EventHandler<DockLayoutDragEventArgs>? DragUpdated;
    public event EventHandler<DockLayoutDragEventArgs>? DragCompleted;

    public void Show(DockLayoutKind layout)
    {
        IsOpen = true;
        SelectedLayout = layout;
    }

    public void Hide()
    {
        IsOpen = false;
        IsDragging = false;
        FocusedZoneId = null;
    }

    public bool MoveFocus(string? zoneId)
    {
        if (zoneId is null || Zones.All(zone => !string.Equals(zone.Id, zoneId, StringComparison.Ordinal)))
        {
            return false;
        }
        var previous = FocusedZoneId;
        FocusedZoneId = zoneId;
        if (!string.Equals(previous, zoneId, StringComparison.Ordinal))
        {
            FocusChanged?.Invoke(this, new DockLayoutFocusChangedEventArgs(previous, zoneId));
        }
        return true;
    }

    /// <summary>Moves focus through zones in deterministic reading order.</summary>
    public bool MoveFocusBy(int delta)
    {
        if (Zones.Count == 0 || delta == 0)
        {
            return false;
        }
        var current = Zones.IndexOf(FocusedZone!);
        if (current < 0)
        {
            current = delta > 0 ? -1 : Zones.Count;
        }
        var next = Math.Clamp(current + Math.Sign(delta), 0, Zones.Count - 1);
        return MoveFocus(Zones[next].Id);
    }

    /// <summary>Chooses the first zone containing a point in <see cref="PreviewBounds"/>.</summary>
    public bool SelectZoneAt(Point point)
    {
        var bounds = PreviewBounds;
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return false;
        }
        var nx = (point.X - bounds.X) / bounds.Width;
        var ny = (point.Y - bounds.Y) / bounds.Height;
        var zone = Zones.FirstOrDefault(candidate => nx >= candidate.X && nx <= candidate.X + candidate.Width && ny >= candidate.Y && ny <= candidate.Y + candidate.Height);
        return zone is not null && MoveFocus(zone.Id);
    }

    public bool BeginDrag(Point pointerPosition)
    {
        if (!IsOpen || Zones.Count == 0)
        {
            return false;
        }
        IsDragging = true;
        SelectZoneAt(pointerPosition);
        DragStarted?.Invoke(this, new DockLayoutDragEventArgs(pointerPosition, FocusedZone, false));
        return true;
    }

    public bool UpdateDrag(Point pointerPosition)
    {
        if (!IsDragging)
        {
            return false;
        }
        SelectZoneAt(pointerPosition);
        DragUpdated?.Invoke(this, new DockLayoutDragEventArgs(pointerPosition, FocusedZone, false));
        return true;
    }

    public bool EndDrag(Point pointerPosition, bool commit = true)
    {
        if (!IsDragging)
        {
            return false;
        }
        SelectZoneAt(pointerPosition);
        var committed = commit && Commit();
        IsDragging = false;
        DragCompleted?.Invoke(this, new DockLayoutDragEventArgs(pointerPosition, FocusedZone, committed));
        return committed;
    }

    public Rect GetZoneBounds(string zoneId)
    {
        var zone = Zones.FirstOrDefault(candidate => string.Equals(candidate.Id, zoneId, StringComparison.Ordinal));
        if (zone is null || PreviewBounds.Width <= 0 || PreviewBounds.Height <= 0)
        {
            return default;
        }
        return new Rect(
            PreviewBounds.X + zone.X * PreviewBounds.Width,
            PreviewBounds.Y + zone.Y * PreviewBounds.Height,
            zone.Width * PreviewBounds.Width,
            zone.Height * PreviewBounds.Height);
    }

    /// <summary>Raises a host request after the user confirms the selected zone.</summary>
    public bool Commit()
    {
        if (!IsOpen || SelectedLayout is null || FocusedZoneId is null)
        {
            return false;
        }
        var commit = new DockLayoutCommit(LayoutId, SelectedLayout.Value, FocusedZoneId);
        CommitRequested?.Invoke(this, new DockLayoutCommitRequestedEventArgs(commit));
        LayoutCommitted?.Invoke(this, EventArgs.Empty);
        return true;
    }

    private static void OnLayoutChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not DockLayoutPreview preview)
        {
            return;
        }
        preview.Zones.Clear();
        if (preview.SelectedLayout is DockLayoutKind layout)
        {
            preview.LayoutId = GetLayoutId(layout);
            foreach (var zone in CreateZones(layout))
            {
                preview.Zones.Add(zone);
            }
            var previous = preview.FocusedZoneId;
            preview.FocusedZoneId = GetInitialZoneId(layout, preview.Zones);
            if (!string.Equals(previous, preview.FocusedZoneId, StringComparison.Ordinal))
            {
                preview.FocusChanged?.Invoke(preview, new DockLayoutFocusChangedEventArgs(previous, preview.FocusedZoneId));
            }
        }
        else
        {
            preview.LayoutId = string.Empty;
            preview.FocusedZoneId = null;
        }
        preview.LayoutChanged?.Invoke(preview, EventArgs.Empty);
    }

    private static string GetLayoutId(DockLayoutKind layout) => layout switch
    {
        DockLayoutKind.LeftHalf => "left-half",
        DockLayoutKind.RightHalf => "right-half",
        DockLayoutKind.TopHalf => "top-half",
        DockLayoutKind.BottomHalf => "bottom-half",
        DockLayoutKind.ThreeColumnLeft => "three-column-left",
        DockLayoutKind.ThreeColumnCenter => "three-column-center",
        DockLayoutKind.ThreeColumnRight => "three-column-right",
        DockLayoutKind.FourQuadrants => "four-quadrants",
        _ => "unknown"
    };

    private static string? GetInitialZoneId(DockLayoutKind layout, IReadOnlyList<DockLayoutZone> zones)
    {
        var preferred = layout switch
        {
            DockLayoutKind.RightHalf or DockLayoutKind.ThreeColumnRight => "right",
            DockLayoutKind.TopHalf => "top",
            DockLayoutKind.BottomHalf => "bottom",
            DockLayoutKind.ThreeColumnCenter => "center",
            _ => zones.FirstOrDefault()?.Id
        };
        return zones.Any(zone => string.Equals(zone.Id, preferred, StringComparison.Ordinal))
            ? preferred
            : zones.FirstOrDefault()?.Id;
    }

    private static IReadOnlyList<DockLayoutZone> CreateZones(DockLayoutKind layout) => layout switch
    {
        DockLayoutKind.LeftHalf => new[] { new DockLayoutZone("left", 0, 0, .5, 1), new DockLayoutZone("right", .5, 0, .5, 1) },
        DockLayoutKind.RightHalf => new[] { new DockLayoutZone("left", 0, 0, .5, 1), new DockLayoutZone("right", .5, 0, .5, 1) },
        DockLayoutKind.TopHalf => new[] { new DockLayoutZone("top", 0, 0, 1, .5), new DockLayoutZone("bottom", 0, .5, 1, .5) },
        DockLayoutKind.BottomHalf => new[] { new DockLayoutZone("top", 0, 0, 1, .5), new DockLayoutZone("bottom", 0, .5, 1, .5) },
        DockLayoutKind.ThreeColumnLeft => new[] { new DockLayoutZone("left", 0, 0, .5, 1), new DockLayoutZone("center", .5, 0, .25, 1), new DockLayoutZone("right", .75, 0, .25, 1) },
        DockLayoutKind.ThreeColumnCenter => new[] { new DockLayoutZone("left", 0, 0, .25, 1), new DockLayoutZone("center", .25, 0, .5, 1), new DockLayoutZone("right", .75, 0, .25, 1) },
        DockLayoutKind.ThreeColumnRight => new[] { new DockLayoutZone("left", 0, 0, .25, 1), new DockLayoutZone("center", .25, 0, .25, 1), new DockLayoutZone("right", .5, 0, .5, 1) },
        DockLayoutKind.FourQuadrants => new[] { new DockLayoutZone("top-left", 0, 0, .5, .5), new DockLayoutZone("top-right", .5, 0, .5, .5), new DockLayoutZone("bottom-left", 0, .5, .5, .5), new DockLayoutZone("bottom-right", .5, .5, .5, .5) },
        _ => Array.Empty<DockLayoutZone>()
    };
}
