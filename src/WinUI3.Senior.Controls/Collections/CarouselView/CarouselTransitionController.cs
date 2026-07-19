using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;

namespace WinUI3.Senior.Controls;

/// <summary>Composition-only presentation helper. It never owns selection or item realization.</summary>
internal sealed class CarouselTransitionController : IDisposable
{
    private readonly CarouselView _owner;
    private readonly HashSet<Visual> _animatedVisuals = new();
    private Visual? _activeVisual;
    private UIElement? _activeElement;
    private bool _retryScheduled;
    private bool _disposed;

    public CarouselTransitionController(CarouselView owner) => _owner = owner;

    public void TransitionTo(int previousIndex, int currentIndex, CarouselTransition transition)
    {
        if (_disposed) return;
        var current = _owner.GetRealizedElement(currentIndex);
        if (current is null)
        {
            // Selection changes invalidate the virtualizing layout asynchronously.
            // Retry once on the UI queue so a newly realized item can still animate,
            // while avoiding an unbounded loop when a template cannot realize it.
            if (!_retryScheduled && _owner.DispatcherQueue is { } queue)
            {
                _retryScheduled = true;
                queue.TryEnqueue(() =>
                {
                    _retryScheduled = false;
                    TransitionTo(previousIndex, currentIndex, transition);
                });
            }
            return;
        }

        var previous = _owner.GetRealizedElement(previousIndex);
        StopActiveAnimations();
        if (_owner.IsReducedMotion)
        {
            Fade(current, TimeSpan.FromMilliseconds(100));
            return;
        }

        switch (transition)
        {
            case CarouselTransition.Fade:
                Fade(current, TimeSpan.FromMilliseconds(250));
                break;
            case CarouselTransition.CoverFlow:
                CoverFlow(current, previous);
                break;
            default:
                Slide(current, previous);
                break;
        }
    }

    public void SetDragOffset(double offset)
    {
        if (_disposed) return;
        var element = _owner.GetRealizedElement(_owner.SelectedIndex);
        if (element is null)
        {
            return;
        }

        _activeElement = element;
        _activeVisual = ElementCompositionPreview.GetElementVisual(element);
        _activeVisual.StopAnimation(nameof(Visual.Offset));
        _activeVisual.Offset = new Vector3((float)offset, 0, 0);
    }

    public void SettleDrag()
    {
        if (_disposed || _activeVisual is null)
        {
            return;
        }

        var animation = _activeVisual.Compositor.CreateVector3KeyFrameAnimation();
        animation.Duration = TimeSpan.FromMilliseconds(_owner.IsReducedMotion ? 0 : 100);
        animation.InsertKeyFrame(1f, Vector3.Zero);
        _activeVisual.StartAnimation(nameof(Visual.Offset), animation);
        _animatedVisuals.Add(_activeVisual);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        StopActiveAnimations();
    }

    private void Slide(UIElement current, UIElement? previous)
    {
        var currentVisual = ElementCompositionPreview.GetElementVisual(current);
        var width = Math.Max(1, (float)_owner.ActualWidth);
        var direction = _owner.FlowDirection == FlowDirection.RightToLeft ? -1f : 1f;
        currentVisual.Offset = new Vector3(width * direction, 0, 0);
        var animation = currentVisual.Compositor.CreateVector3KeyFrameAnimation();
        animation.Duration = TimeSpan.FromMilliseconds(250);
        animation.InsertKeyFrame(1f, Vector3.Zero);
        currentVisual.StartAnimation(nameof(Visual.Offset), animation);
        _animatedVisuals.Add(currentVisual);
        if (previous is not null)
        {
            Fade(previous, TimeSpan.FromMilliseconds(120), 1f, 0f);
        }
    }

    private void Fade(UIElement element, TimeSpan duration, float from = 0f, float to = 1f)
    {
        var visual = ElementCompositionPreview.GetElementVisual(element);
        visual.Opacity = from;
        var animation = visual.Compositor.CreateScalarKeyFrameAnimation();
        animation.Duration = duration;
        animation.InsertKeyFrame(1f, to);
        visual.StartAnimation(nameof(Visual.Opacity), animation);
        _animatedVisuals.Add(visual);
    }

    private void CoverFlow(UIElement current, UIElement? previous)
    {
        var visual = ElementCompositionPreview.GetElementVisual(current);
        visual.CenterPoint = new Vector3(Math.Max(1, current.ActualSize.X) / 2f, Math.Max(1, current.ActualSize.Y) / 2f, 0);
        visual.RotationAxis = new Vector3(0, 1, 0);
        visual.Scale = new Vector3(0.82f, 0.82f, 1f);
        var scale = visual.Compositor.CreateVector3KeyFrameAnimation();
        scale.Duration = TimeSpan.FromMilliseconds(250);
        scale.InsertKeyFrame(1f, Vector3.One);
        visual.StartAnimation(nameof(Visual.Scale), scale);
        var rotation = visual.Compositor.CreateScalarKeyFrameAnimation();
        rotation.Duration = TimeSpan.FromMilliseconds(250);
        rotation.InsertKeyFrame(1f, 0);
        visual.RotationAngleInDegrees = _owner.FlowDirection == FlowDirection.RightToLeft ? -28 : 28;
        visual.StartAnimation(nameof(Visual.RotationAngleInDegrees), rotation);
        _animatedVisuals.Add(visual);
        if (previous is not null)
        {
            Fade(previous, TimeSpan.FromMilliseconds(120), 1f, 0f);
        }
    }

    private void StopActiveAnimations()
    {
        if (_activeVisual is not null)
        {
            _activeVisual.StopAnimation(nameof(Visual.Offset));
            _activeVisual.StopAnimation(nameof(Visual.Opacity));
            _activeVisual.StopAnimation(nameof(Visual.Scale));
            _activeVisual.StopAnimation(nameof(Visual.RotationAngleInDegrees));
        }
        foreach (var visual in _animatedVisuals)
        {
            visual.StopAnimation(nameof(Visual.Offset));
            visual.StopAnimation(nameof(Visual.Opacity));
            visual.StopAnimation(nameof(Visual.Scale));
            visual.StopAnimation(nameof(Visual.RotationAngleInDegrees));
        }
        _animatedVisuals.Clear();
        _activeVisual = null;
        _activeElement = null;
    }
}
