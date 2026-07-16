using System;
using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Hosting;

namespace WinUI3.Senior.Controls;

/// <summary>Composition-only presentation helper. It never owns selection or item realization.</summary>
internal sealed class CarouselTransitionController : IDisposable
{
    private readonly CarouselView _owner;
    private Visual? _activeVisual;
    private UIElement? _activeElement;

    public CarouselTransitionController(CarouselView owner) => _owner = owner;

    public void TransitionTo(int previousIndex, int currentIndex, CarouselTransition transition)
    {
        var current = _owner.GetRealizedElement(currentIndex);
        if (current is null)
        {
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
        if (_activeVisual is null)
        {
            return;
        }

        var animation = _activeVisual.Compositor.CreateVector3KeyFrameAnimation();
        animation.Duration = TimeSpan.FromMilliseconds(_owner.IsReducedMotion ? 0 : 100);
        animation.InsertKeyFrame(1f, Vector3.Zero);
        _activeVisual.StartAnimation(nameof(Visual.Offset), animation);
    }

    public void Dispose() => StopActiveAnimations();

    private void Slide(UIElement current, UIElement? previous)
    {
        var currentVisual = ElementCompositionPreview.GetElementVisual(current);
        var width = Math.Max(1, (float)_owner.ActualWidth);
        currentVisual.Offset = new Vector3(width, 0, 0);
        var animation = currentVisual.Compositor.CreateVector3KeyFrameAnimation();
        animation.Duration = TimeSpan.FromMilliseconds(250);
        animation.InsertKeyFrame(1f, Vector3.Zero);
        currentVisual.StartAnimation(nameof(Visual.Offset), animation);
        if (previous is not null)
        {
            Fade(previous, TimeSpan.FromMilliseconds(120), 1f, 0f);
        }
    }

    private static void Fade(UIElement element, TimeSpan duration, float from = 0f, float to = 1f)
    {
        var visual = ElementCompositionPreview.GetElementVisual(element);
        visual.Opacity = from;
        var animation = visual.Compositor.CreateScalarKeyFrameAnimation();
        animation.Duration = duration;
        animation.InsertKeyFrame(1f, to);
        visual.StartAnimation(nameof(Visual.Opacity), animation);
    }

    private static void CoverFlow(UIElement current, UIElement? previous)
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
        visual.RotationAngleInDegrees = 28;
        visual.StartAnimation(nameof(Visual.RotationAngleInDegrees), rotation);
        if (previous is not null)
        {
            Fade(previous, TimeSpan.FromMilliseconds(120), 1f, 0f);
        }
    }

    private void StopActiveAnimations()
    {
        if (_activeVisual is null)
        {
            return;
        }

        _activeVisual.StopAnimation(nameof(Visual.Offset));
        _activeVisual.StopAnimation(nameof(Visual.Opacity));
        _activeVisual.StopAnimation(nameof(Visual.Scale));
        _activeVisual.StopAnimation(nameof(Visual.RotationAngleInDegrees));
        _activeVisual = null;
        _activeElement = null;
    }
}
