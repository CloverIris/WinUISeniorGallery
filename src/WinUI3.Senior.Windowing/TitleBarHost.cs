using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace WinUI3.Senior.Windowing;

/// <summary>Composable title-bar visual surface that reports host hit-test regions.</summary>
public sealed class TitleBarHost : ContentControl
{
    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(TitleBarHost), new PropertyMetadata(string.Empty, OnLayoutPropertyChanged));

    public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
        nameof(Subtitle), typeof(string), typeof(TitleBarHost), new PropertyMetadata(string.Empty, OnLayoutPropertyChanged));

    public static readonly DependencyProperty PreferredHeightProperty = DependencyProperty.Register(
        nameof(PreferredHeight), typeof(double), typeof(TitleBarHost), new PropertyMetadata(32d, OnLayoutPropertyChanged));

    public static readonly DependencyProperty IsDragRegionEnabledProperty = DependencyProperty.Register(
        nameof(IsDragRegionEnabled), typeof(bool), typeof(TitleBarHost), new PropertyMetadata(true, OnLayoutPropertyChanged));

    private readonly ObservableCollection<TitleBarInteractiveRegion> _interactiveRegions = new();
    private bool _batchUpdatingRegions;

    public TitleBarHost()
    {
        _interactiveRegions.CollectionChanged += (_, _) => RaiseRegionsChanged();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public double PreferredHeight
    {
        get => (double)GetValue(PreferredHeightProperty);
        set => SetValue(PreferredHeightProperty, Math.Clamp(value, 24, 96));
    }

    public bool IsDragRegionEnabled
    {
        get => (bool)GetValue(IsDragRegionEnabledProperty);
        set => SetValue(IsDragRegionEnabledProperty, value);
    }

    public IReadOnlyList<TitleBarInteractiveRegion> InteractiveRegions => _interactiveRegions;

    public event EventHandler<TitleBarRegionsChangedEventArgs>? RegionsChanged;

    public void SetInteractiveRegions(IEnumerable<TitleBarInteractiveRegion>? regions)
    {
        _batchUpdatingRegions = true;
        try
        {
            _interactiveRegions.Clear();
            if (regions is not null)
            {
                foreach (var region in regions)
                {
                    if (region.Bounds.Width <= 0 || region.Bounds.Height <= 0 || !double.IsFinite(region.Bounds.X) || !double.IsFinite(region.Bounds.Y))
                    {
                        continue;
                    }
                    var existing = _interactiveRegions.FirstOrDefault(candidate => !string.IsNullOrWhiteSpace(region.AutomationId) && string.Equals(candidate.AutomationId, region.AutomationId, StringComparison.Ordinal));
                    if (existing is not null) _interactiveRegions.Remove(existing);
                    _interactiveRegions.Add(region);
                }
            }
        }
        finally
        {
            _batchUpdatingRegions = false;
            RaiseRegionsChanged();
        }
    }

    public IReadOnlyList<Rect> GetDragRegions(Rect bounds)
    {
        if (!IsDragRegionEnabled || bounds.Width <= 0 || bounds.Height <= 0)
        {
            return Array.Empty<Rect>();
        }

        var regions = _interactiveRegions.Select(static x => x.Bounds).ToArray();
        if (regions.Length == 0)
        {
            return new[] { bounds };
        }

        // Subtract each interactive rectangle. Hosts can translate these DIP rectangles to HWND pixels.
        var remaining = new List<Rect> { bounds };
        foreach (var region in regions)
        {
            var intersection = Intersect(bounds, region);
            if (intersection.Width <= 0 || intersection.Height <= 0)
            {
                continue;
            }
            var next = new List<Rect>();
            foreach (var candidate in remaining)
            {
                next.AddRange(Subtract(candidate, intersection));
            }
            remaining = next;
        }
        return remaining.Where(static rect => rect.Width > 0 && rect.Height > 0).ToArray();
    }

    public TitleBarInteractiveRegion? GetInteractiveRegionAt(Point point)
        => _interactiveRegions.LastOrDefault(region => region.Bounds.Contains(point));

    private static Rect Intersect(Rect a, Rect b)
    {
        var left = Math.Max(a.X, b.X);
        var top = Math.Max(a.Y, b.Y);
        var right = Math.Min(a.X + a.Width, b.X + b.Width);
        var bottom = Math.Min(a.Y + a.Height, b.Y + b.Height);
        return right > left && bottom > top ? new Rect(left, top, right - left, bottom - top) : default;
    }

    private static IEnumerable<Rect> Subtract(Rect source, Rect cut)
    {
        var intersection = Intersect(source, cut);
        if (intersection.Width <= 0 || intersection.Height <= 0)
        {
            yield return source;
            yield break;
        }
        if (intersection.Y > source.Y)
        {
            yield return new Rect(source.X, source.Y, source.Width, intersection.Y - source.Y);
        }
        if (intersection.Y + intersection.Height < source.Y + source.Height)
        {
            yield return new Rect(source.X, intersection.Y + intersection.Height, source.Width, source.Y + source.Height - intersection.Y - intersection.Height);
        }
        if (intersection.X > source.X)
        {
            yield return new Rect(source.X, intersection.Y, intersection.X - source.X, intersection.Height);
        }
        if (intersection.X + intersection.Width < source.X + source.Width)
        {
            yield return new Rect(intersection.X + intersection.Width, intersection.Y, source.X + source.Width - intersection.X - intersection.Width, intersection.Height);
        }
    }

    private static void OnLayoutPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is TitleBarHost host)
        {
            host.RaiseRegionsChanged();
        }
    }

    private void RaiseRegionsChanged()
    {
        if (_batchUpdatingRegions) return;
        RegionsChanged?.Invoke(this, new TitleBarRegionsChangedEventArgs(_interactiveRegions.ToArray()));
    }
}
