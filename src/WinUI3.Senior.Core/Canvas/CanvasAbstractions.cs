using System.Collections.Immutable;

namespace WinUI3.Senior.Core;

public readonly record struct CanvasPoint(double X, double Y)
{
    public static CanvasPoint operator +(CanvasPoint left, CanvasPoint right) => new(left.X + right.X, left.Y + right.Y);
    public static CanvasPoint operator -(CanvasPoint left, CanvasPoint right) => new(left.X - right.X, left.Y - right.Y);
}

public readonly record struct CanvasSize(double Width, double Height)
{
    public bool IsValid => double.IsFinite(Width) && double.IsFinite(Height) && Width >= 0 && Height >= 0;
}

public readonly record struct CanvasRect(double X, double Y, double Width, double Height)
{
    public double Right => X + Width;
    public double Bottom => Y + Height;
    public bool IsValid => double.IsFinite(X) && double.IsFinite(Y) && double.IsFinite(Width) && double.IsFinite(Height) && Width >= 0 && Height >= 0;
    public bool Contains(CanvasPoint point) => point.X >= X && point.X <= Right && point.Y >= Y && point.Y <= Bottom;
    public bool Intersects(CanvasRect other) => X < other.Right && Right > other.X && Y < other.Bottom && Bottom > other.Y;
    public CanvasRect Inflate(double amount) => new(X - amount, Y - amount, Math.Max(0, Width + amount * 2), Math.Max(0, Height + amount * 2));
    public CanvasRect Translate(CanvasPoint delta) => new(X + delta.X, Y + delta.Y, Width, Height);
    public static CanvasRect FromPoints(CanvasPoint a, CanvasPoint b) => new(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
}

public readonly record struct CanvasTransform(double Zoom, CanvasPoint Offset)
{
    public static CanvasTransform Identity => new(1, new CanvasPoint(0, 0));
    public CanvasPoint WorldToScreen(CanvasPoint world) => new(world.X * Zoom + Offset.X, world.Y * Zoom + Offset.Y);
    public CanvasPoint ScreenToWorld(CanvasPoint screen) => Zoom <= 0 ? screen : new((screen.X - Offset.X) / Zoom, (screen.Y - Offset.Y) / Zoom);
}

public enum CanvasInputDeviceKind
{
    Unknown,
    Mouse,
    Touch,
    Pen,
    PrecisionTouchpad,
    GameController,
}

public readonly record struct CanvasPressureSample(
    CanvasPoint Point,
    double Pressure = 1,
    double TiltX = 0,
    double TiltY = 0,
    double ContactWidth = 0,
    double ContactHeight = 0,
    long Timestamp = 0,
    CanvasInputDeviceKind DeviceKind = CanvasInputDeviceKind.Unknown)
{
    public bool IsValid => double.IsFinite(Point.X) && double.IsFinite(Point.Y)
        && double.IsFinite(Pressure) && Pressure >= 0 && Pressure <= 1
        && double.IsFinite(TiltX) && double.IsFinite(TiltY)
        && double.IsFinite(ContactWidth) && ContactWidth >= 0
        && double.IsFinite(ContactHeight) && ContactHeight >= 0;

    public CanvasPressureSample Normalize() => IsValid
        ? this with { Pressure = Math.Clamp(Pressure, 0, 1), TiltX = Math.Clamp(TiltX, -90, 90), TiltY = Math.Clamp(TiltY, -90, 90) }
        : throw new ArgumentException("Canvas pressure sample contains a non-finite or out-of-range value.");
}

public sealed record CanvasInkStyle(
    uint Color = 0xFF1A1A1A,
    double MinimumWidth = 1,
    double MaximumWidth = 8,
    bool PressureAffectsWidth = true)
{
    public CanvasInkStyle Normalize()
    {
        var minimum = double.IsFinite(MinimumWidth) ? Math.Clamp(MinimumWidth, .1, 256) : 1;
        var maximum = double.IsFinite(MaximumWidth) ? Math.Clamp(MaximumWidth, minimum, 256) : Math.Max(8, minimum);
        return this with { MinimumWidth = minimum, MaximumWidth = maximum };
    }

    public double WidthAt(double pressure)
    {
        var value = PressureAffectsWidth ? Math.Clamp(pressure, 0, 1) : 1;
        return MinimumWidth + (MaximumWidth - MinimumWidth) * value;
    }
}

public sealed record CanvasStroke(
    string Id,
    ImmutableArray<CanvasPressureSample> Samples,
    CanvasInkStyle Style,
    bool IsCompleted = true,
    object? Tag = null)
{
    public CanvasStroke Normalize() => this with
    {
        Id = string.IsNullOrWhiteSpace(Id) ? Guid.NewGuid().ToString("N") : Id,
        Samples = Samples.IsDefault ? ImmutableArray<CanvasPressureSample>.Empty : Samples.Where(sample => sample.IsValid).Select(sample => sample.Normalize()).ToImmutableArray(),
        Style = (Style ?? new CanvasInkStyle()).Normalize()
    };

    public CanvasRect Bounds
    {
        get
        {
            if (Samples.IsDefaultOrEmpty) return new CanvasRect(0, 0, 0, 0);
            var points = Samples.Select(sample => sample.Point).ToArray();
            return CanvasRect.FromPoints(new CanvasPoint(points.Min(point => point.X), points.Min(point => point.Y)), new CanvasPoint(points.Max(point => point.X), points.Max(point => point.Y))).Inflate(Style.MaximumWidth / 2);
        }
    }
}

public enum CanvasInkSessionState
{
    Idle,
    Active,
    Completed,
    Cancelled,
}

public sealed class CanvasInkChangedEventArgs(CanvasStroke? stroke, CanvasInkSessionState state, bool isUserInitiated) : EventArgs
{
    public CanvasStroke? Stroke { get; } = stroke;
    public CanvasInkSessionState State { get; } = state;
    public bool IsUserInitiated { get; } = isUserInitiated;
}

/// <summary>
/// Host-neutral pressure stroke accumulator. It accepts already normalized samples;
/// WinUI, InkPresenter, or a future C++ bridge can feed it without leaking platform types into Core.
/// </summary>
public sealed class CanvasInkSession
{
    private readonly object _gate = new();
    private ImmutableArray<CanvasPressureSample>.Builder? _samples;
    private CanvasStroke? _stroke;

    public CanvasInkSessionState State { get; private set; } = CanvasInkSessionState.Idle;
    public CanvasStroke? CurrentStroke { get { lock (_gate) return _stroke; } }
    public event EventHandler<CanvasInkChangedEventArgs>? Changed;

    public bool Begin(CanvasPressureSample firstSample, CanvasInkStyle? style = null, string? id = null)
    {
        if (!firstSample.IsValid) return false;
        lock (_gate)
        {
            if (State == CanvasInkSessionState.Active) return false;
            _samples = ImmutableArray.CreateBuilder<CanvasPressureSample>();
            _samples.Add(firstSample.Normalize());
            _stroke = new CanvasStroke(id ?? Guid.NewGuid().ToString("N"), _samples.ToImmutable(), (style ?? new CanvasInkStyle()).Normalize(), false);
            State = CanvasInkSessionState.Active;
            RaiseChanged(true);
            return true;
        }
    }

    public bool AddSample(CanvasPressureSample sample)
    {
        if (!sample.IsValid) return false;
        lock (_gate)
        {
            if (State != CanvasInkSessionState.Active || _samples is null) return false;
            var normalized = sample.Normalize();
            if (_samples.Count > 0 && _samples[_samples.Count - 1].Point == normalized.Point && _samples[_samples.Count - 1].Pressure == normalized.Pressure) return false;
            _samples.Add(normalized);
            _stroke = _stroke! with { Samples = _samples.ToImmutable() };
            RaiseChanged(true);
            return true;
        }
    }

    public CanvasStroke? Complete(bool isUserInitiated = true)
    {
        lock (_gate)
        {
            if (State != CanvasInkSessionState.Active || _stroke is null) return null;
            _stroke = _stroke with { Samples = _samples?.ToImmutable() ?? _stroke.Samples, IsCompleted = true };
            State = CanvasInkSessionState.Completed;
            RaiseChanged(isUserInitiated);
            return _stroke;
        }
    }

    public bool Cancel(bool isUserInitiated = true)
    {
        lock (_gate)
        {
            if (State != CanvasInkSessionState.Active) return false;
            State = CanvasInkSessionState.Cancelled;
            _samples = null;
            _stroke = null;
            RaiseChanged(isUserInitiated);
            return true;
        }
    }

    public void Reset()
    {
        lock (_gate)
        {
            _samples = null;
            _stroke = null;
            State = CanvasInkSessionState.Idle;
        }
    }

    private void RaiseChanged(bool isUserInitiated) => Changed?.Invoke(this, new CanvasInkChangedEventArgs(_stroke, State, isUserInitiated));
}

public sealed record CanvasObject(
    string Id,
    CanvasRect Bounds,
    object? Payload = null,
    bool IsLocked = false,
    int ZIndex = 0)
{
    public CanvasObject Translate(CanvasPoint delta) => this with { Bounds = Bounds.Translate(delta) };
}

public sealed record CanvasDocumentSnapshot(
    long Revision,
    ImmutableArray<CanvasObject> Objects,
    ImmutableHashSet<string> SelectedIds)
{
    public static CanvasDocumentSnapshot Empty { get; } = new(0, ImmutableArray<CanvasObject>.Empty, ImmutableHashSet.Create<string>(StringComparer.Ordinal));
    public CanvasObject? Find(string id) => Objects.FirstOrDefault(item => StringComparer.Ordinal.Equals(item.Id, id));
    public IReadOnlyList<CanvasObject> HitTest(CanvasPoint point) => Objects
        .Where(item => item.Bounds.Contains(point))
        .OrderByDescending(item => item.ZIndex)
        .ThenByDescending(item => Array.IndexOf(Objects.ToArray(), item))
        .ToArray();
}

public sealed class CanvasDocumentChangedEventArgs(CanvasDocumentSnapshot previous, CanvasDocumentSnapshot current, string reason, bool isUserInitiated) : EventArgs
{
    public CanvasDocumentSnapshot Previous { get; } = previous ?? throw new ArgumentNullException(nameof(previous));
    public CanvasDocumentSnapshot Current { get; } = current ?? throw new ArgumentNullException(nameof(current));
    public string Reason { get; } = reason ?? string.Empty;
    public bool IsUserInitiated { get; } = isUserInitiated;
}

/// <summary>
/// Thread-safe document state and undo/redo boundary for WinUI and future native canvas hosts.
/// Payloads are opaque to Core; rendering and serialization stay outside this contract.
/// </summary>
public sealed class CanvasDocumentController
{
    private readonly object _gate = new();
    private readonly Stack<CanvasDocumentSnapshot> _undo = new();
    private readonly Stack<CanvasDocumentSnapshot> _redo = new();
    private CanvasDocumentSnapshot _snapshot;

    public CanvasDocumentController(CanvasDocumentSnapshot? initial = null) => _snapshot = Normalize(initial ?? CanvasDocumentSnapshot.Empty);
    public CanvasDocumentSnapshot Snapshot { get { lock (_gate) return _snapshot; } }
    public bool CanUndo { get { lock (_gate) return _undo.Count > 0; } }
    public bool CanRedo { get { lock (_gate) return _redo.Count > 0; } }
    public event EventHandler<CanvasDocumentChangedEventArgs>? Changed;

    public bool Replace(CanvasDocumentSnapshot snapshot, string reason = "replace", bool isUserInitiated = false)
    {
        ArgumentNullException.ThrowIfNull(snapshot);
        return Commit(Normalize(snapshot), reason, isUserInitiated, clearHistory: false);
    }

    public bool Insert(CanvasObject item, bool select = true, bool isUserInitiated = true)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (string.IsNullOrWhiteSpace(item.Id) || !item.Bounds.IsValid) return false;
        lock (_gate)
        {
            if (_snapshot.Objects.Any(existing => StringComparer.Ordinal.Equals(existing.Id, item.Id))) return false;
            var selected = select ? ImmutableHashSet.Create(StringComparer.Ordinal, item.Id) : _snapshot.SelectedIds;
            return CommitLocked(_snapshot with { Objects = _snapshot.Objects.Add(item), SelectedIds = selected }, "insert", isUserInitiated);
        }
    }

    public bool ReplaceObject(CanvasObject item, bool isUserInitiated = true)
    {
        ArgumentNullException.ThrowIfNull(item);
        if (string.IsNullOrWhiteSpace(item.Id) || !item.Bounds.IsValid) return false;
        lock (_gate)
        {
            var index = -1;
            for (var i = 0; i < _snapshot.Objects.Length; i++)
            {
                if (StringComparer.Ordinal.Equals(_snapshot.Objects[i].Id, item.Id)) { index = i; break; }
            }
            if (index < 0) return false;
            var objects = _snapshot.Objects.SetItem(index, item);
            return CommitLocked(_snapshot with { Objects = objects }, "replace-object", isUserInitiated);
        }
    }

    public bool MoveSelection(CanvasPoint delta, bool isUserInitiated = true)
    {
        if (!double.IsFinite(delta.X) || !double.IsFinite(delta.Y)) return false;
        lock (_gate)
        {
            var objects = _snapshot.Objects.Select(item => _snapshot.SelectedIds.Contains(item.Id) && !item.IsLocked ? item.Translate(delta) : item).ToImmutableArray();
            if (objects.SequenceEqual(_snapshot.Objects)) return false;
            return CommitLocked(_snapshot with { Objects = objects }, "move-selection", isUserInitiated);
        }
    }

    public bool DeleteSelection(bool isUserInitiated = true)
    {
        lock (_gate)
        {
            var objects = _snapshot.Objects.Where(item => !_snapshot.SelectedIds.Contains(item.Id) || item.IsLocked).ToImmutableArray();
            if (objects.Length == _snapshot.Objects.Length) return false;
            return CommitLocked(_snapshot with { Objects = objects, SelectedIds = ImmutableHashSet<string>.Empty }, "delete-selection", isUserInitiated);
        }
    }

    public bool Select(IEnumerable<string> ids, bool additive = false, bool isUserInitiated = true)
    {
        ArgumentNullException.ThrowIfNull(ids);
        lock (_gate)
        {
            var valid = ids.Where(id => !string.IsNullOrWhiteSpace(id) && _snapshot.Objects.Any(item => StringComparer.Ordinal.Equals(item.Id, id)))
                .ToImmutableHashSet(StringComparer.Ordinal);
            var selected = additive ? _snapshot.SelectedIds.Union(valid).ToImmutableHashSet(StringComparer.Ordinal) : valid;
            if (selected.SetEquals(_snapshot.SelectedIds)) return false;
            return CommitLocked(_snapshot with { SelectedIds = selected }, "selection", isUserInitiated, addToUndo: false);
        }
    }

    public bool Undo()
    {
        lock (_gate)
        {
            if (_undo.Count == 0) return false;
            var previous = _snapshot;
            var target = _undo.Pop();
            _redo.Push(previous);
            _snapshot = target with { Revision = previous.Revision + 1 };
            RaiseChanged(previous, _snapshot, "undo", true);
            return true;
        }
    }

    public bool Redo()
    {
        lock (_gate)
        {
            if (_redo.Count == 0) return false;
            var previous = _snapshot;
            var target = _redo.Pop();
            _undo.Push(previous);
            _snapshot = target with { Revision = previous.Revision + 1 };
            RaiseChanged(previous, _snapshot, "redo", true);
            return true;
        }
    }

    public IReadOnlyList<CanvasObject> Query(CanvasRect viewport) => Snapshot.Objects.Where(item => item.Bounds.Intersects(viewport)).OrderBy(item => item.ZIndex).ToArray();
    public CanvasObject? HitTest(CanvasPoint worldPoint) => Snapshot.HitTest(worldPoint).FirstOrDefault(item => !item.IsLocked);

    private bool Commit(CanvasDocumentSnapshot target, string reason, bool user, bool clearHistory)
    {
        lock (_gate)
        {
            if (clearHistory) { _undo.Clear(); _redo.Clear(); }
            return CommitLocked(target, reason, user);
        }
    }

    private bool CommitLocked(CanvasDocumentSnapshot target, string reason, bool user, bool addToUndo = true)
    {
        var normalized = Normalize(target) with { Revision = _snapshot.Revision + 1 };
        if (normalized.Objects.SequenceEqual(_snapshot.Objects) && normalized.SelectedIds.SetEquals(_snapshot.SelectedIds)) return false;
        var previous = _snapshot;
        if (addToUndo) _undo.Push(previous);
        _redo.Clear();
        _snapshot = normalized;
        RaiseChanged(previous, normalized, reason, user);
        return true;
    }

    private void RaiseChanged(CanvasDocumentSnapshot previous, CanvasDocumentSnapshot current, string reason, bool user) => Changed?.Invoke(this, new CanvasDocumentChangedEventArgs(previous, current, reason, user));

    private static CanvasDocumentSnapshot Normalize(CanvasDocumentSnapshot snapshot)
    {
        var objects = snapshot.Objects.Where(item => !string.IsNullOrWhiteSpace(item.Id) && item.Bounds.IsValid)
            .GroupBy(item => item.Id, StringComparer.Ordinal).Select(group => group.First()).ToImmutableArray();
        var validIds = objects.Select(item => item.Id).ToImmutableHashSet(StringComparer.Ordinal);
        return snapshot with { Objects = objects, SelectedIds = snapshot.SelectedIds.Intersect(validIds).ToImmutableHashSet(StringComparer.Ordinal) };
    }
}

public sealed class CanvasViewportController
{
    private double _minimumZoom = .1;
    private double _maximumZoom = 8;
    public CanvasViewportController(CanvasTransform? initial = null)
    {
        var value = initial ?? CanvasTransform.Identity;
        Transform = value with { Zoom = double.IsFinite(value.Zoom) && value.Zoom > 0 ? Math.Clamp(value.Zoom, _minimumZoom, _maximumZoom) : 1 };
    }
    public CanvasTransform Transform { get; private set; }
    public double MinimumZoom { get => _minimumZoom; set { _minimumZoom = Math.Clamp(value, .01, 100); if (_maximumZoom < _minimumZoom) _maximumZoom = _minimumZoom; SetZoom(Transform.Zoom); } }
    public double MaximumZoom { get => _maximumZoom; set { _maximumZoom = Math.Max(_minimumZoom, Math.Min(100, value)); SetZoom(Transform.Zoom); } }
    public event EventHandler? Changed;

    public CanvasPoint PanBy(CanvasPoint delta)
    {
        if (!double.IsFinite(delta.X) || !double.IsFinite(delta.Y)) return Transform.Offset;
        Transform = Transform with { Offset = Transform.Offset + delta };
        Changed?.Invoke(this, EventArgs.Empty);
        return Transform.Offset;
    }

    public double ZoomAt(double factor, CanvasPoint screenAnchor)
    {
        if (!double.IsFinite(factor) || factor <= 0) return Transform.Zoom;
        var world = Transform.ScreenToWorld(screenAnchor);
        var zoom = Math.Clamp(Transform.Zoom * factor, MinimumZoom, MaximumZoom);
        Transform = new CanvasTransform(zoom, new CanvasPoint(screenAnchor.X - world.X * zoom, screenAnchor.Y - world.Y * zoom));
        Changed?.Invoke(this, EventArgs.Empty);
        return zoom;
    }

    public void Fit(CanvasRect worldBounds, CanvasSize viewport, double padding = 24)
    {
        if (!worldBounds.IsValid || !viewport.IsValid || worldBounds.Width <= 0 || worldBounds.Height <= 0) return;
        var width = Math.Max(1, viewport.Width - padding * 2);
        var height = Math.Max(1, viewport.Height - padding * 2);
        var zoom = Math.Clamp(Math.Min(width / worldBounds.Width, height / worldBounds.Height), MinimumZoom, MaximumZoom);
        var offset = new CanvasPoint((viewport.Width - worldBounds.Width * zoom) / 2 - worldBounds.X * zoom, (viewport.Height - worldBounds.Height * zoom) / 2 - worldBounds.Y * zoom);
        Transform = new CanvasTransform(zoom, offset);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public CanvasPoint ScreenToWorld(CanvasPoint point) => Transform.ScreenToWorld(point);
    public CanvasPoint WorldToScreen(CanvasPoint point) => Transform.WorldToScreen(point);
    private void SetZoom(double value) { var zoom = Math.Clamp(value, MinimumZoom, MaximumZoom); if (Math.Abs(zoom - Transform.Zoom) < double.Epsilon) return; Transform = Transform with { Zoom = zoom }; Changed?.Invoke(this, EventArgs.Empty); }
}
