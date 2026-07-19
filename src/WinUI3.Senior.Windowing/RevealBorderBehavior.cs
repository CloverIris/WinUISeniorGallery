using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;

namespace WinUI3.Senior.Windowing;

/// <summary>
/// Pointer-aware reveal state that can be consumed by a template or composition layer. The
/// behavior only reports geometry; it does not allocate a CompositionVisual or hard-code colors.
/// </summary>
public sealed class RevealBorderController : IDisposable
{
    private FrameworkElement? _element;
    private bool _isPointerOver;
    private double _revealRadius = 96;
    private double _highlightOpacity = 0.12;
    private bool _isReducedMotion;
    private bool _isHighContrast;

    public bool IsPointerOver => _isPointerOver;
    public Point PointerPosition { get; private set; }
    public double RevealRadius
    {
        get => _revealRadius;
        set
        {
            var normalized = Math.Clamp(value, 0, 1024);
            if (Math.Abs(_revealRadius - normalized) < double.Epsilon)
            {
                return;
            }
            _revealRadius = normalized;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public double HighlightOpacity
    {
        get => _highlightOpacity;
        set
        {
            var normalized = Math.Clamp(value, 0, 1);
            if (Math.Abs(_highlightOpacity - normalized) < double.Epsilon)
            {
                return;
            }
            _highlightOpacity = normalized;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsReducedMotion
    {
        get => _isReducedMotion;
        set
        {
            if (_isReducedMotion == value)
            {
                return;
            }
            _isReducedMotion = value;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool IsHighContrast
    {
        get => _isHighContrast;
        set
        {
            if (_isHighContrast == value)
            {
                return;
            }
            _isHighContrast = value;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool ShouldRender => _isPointerOver && !_isHighContrast && _element is not null;
    public double EffectiveOpacity => ShouldRender && !_isReducedMotion ? _highlightOpacity : 0;
    public double EffectiveRadius => Math.Max(0, _revealRadius);
    public event EventHandler? Changed;

    public void Attach(FrameworkElement element)
    {
        ArgumentNullException.ThrowIfNull(element);
        if (ReferenceEquals(_element, element))
        {
            return;
        }
        Detach();
        _element = element;
        _element.PointerEntered += OnPointerEntered;
        _element.PointerExited += OnPointerExited;
        _element.PointerMoved += OnPointerMoved;
        _element.Unloaded += OnElementUnloaded;
    }

    public void Detach()
    {
        if (_element is null)
        {
            return;
        }
        _element.PointerEntered -= OnPointerEntered;
        _element.PointerExited -= OnPointerExited;
        _element.PointerMoved -= OnPointerMoved;
        _element.Unloaded -= OnElementUnloaded;
        _element = null;
        _isPointerOver = false;
        PointerPosition = default;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose() => Detach();

    /// <summary>Returns the reveal ellipse in element coordinates for a composition/template layer.</summary>
    public Rect GetRevealBounds(double elementWidth, double elementHeight)
    {
        var diameter = EffectiveRadius * 2;
        var x = Math.Clamp(PointerPosition.X - EffectiveRadius, 0, Math.Max(0, elementWidth - diameter));
        var y = Math.Clamp(PointerPosition.Y - EffectiveRadius, 0, Math.Max(0, elementHeight - diameter));
        return new Rect(x, y, Math.Min(diameter, Math.Max(0, elementWidth)), Math.Min(diameter, Math.Max(0, elementHeight)));
    }

    private void OnPointerEntered(object sender, PointerRoutedEventArgs args)
    {
        _isPointerOver = true;
        PointerPosition = args.GetCurrentPoint((UIElement)sender).Position;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs args)
    {
        _isPointerOver = false;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs args)
    {
        PointerPosition = args.GetCurrentPoint((UIElement)sender).Position;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    private void OnElementUnloaded(object sender, RoutedEventArgs args)
    {
        _isPointerOver = false;
        Changed?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>Attached helper for declarative reveal behavior in a XAML template.</summary>
public static class RevealBorderBehavior
{
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached(
        "IsEnabled", typeof(bool), typeof(RevealBorderBehavior), new PropertyMetadata(false, OnIsEnabledChanged));

    public static readonly DependencyProperty RevealRadiusProperty = DependencyProperty.RegisterAttached(
        "RevealRadius", typeof(double), typeof(RevealBorderBehavior), new PropertyMetadata(96d, OnVisualSettingChanged));

    public static readonly DependencyProperty HighlightOpacityProperty = DependencyProperty.RegisterAttached(
        "HighlightOpacity", typeof(double), typeof(RevealBorderBehavior), new PropertyMetadata(0.12d, OnVisualSettingChanged));

    public static readonly DependencyProperty IsReducedMotionProperty = DependencyProperty.RegisterAttached(
        "IsReducedMotion", typeof(bool), typeof(RevealBorderBehavior), new PropertyMetadata(false, OnEnvironmentChanged));

    public static readonly DependencyProperty IsHighContrastProperty = DependencyProperty.RegisterAttached(
        "IsHighContrast", typeof(bool), typeof(RevealBorderBehavior), new PropertyMetadata(false, OnEnvironmentChanged));

    public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);
    public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);
    public static void SetRevealRadius(DependencyObject obj, double value) => obj.SetValue(RevealRadiusProperty, value);
    public static double GetRevealRadius(DependencyObject obj) => (double)obj.GetValue(RevealRadiusProperty);
    public static void SetHighlightOpacity(DependencyObject obj, double value) => obj.SetValue(HighlightOpacityProperty, value);
    public static double GetHighlightOpacity(DependencyObject obj) => (double)obj.GetValue(HighlightOpacityProperty);
    public static void SetIsReducedMotion(DependencyObject obj, bool value) => obj.SetValue(IsReducedMotionProperty, value);
    public static bool GetIsReducedMotion(DependencyObject obj) => (bool)obj.GetValue(IsReducedMotionProperty);
    public static void SetIsHighContrast(DependencyObject obj, bool value) => obj.SetValue(IsHighContrastProperty, value);
    public static bool GetIsHighContrast(DependencyObject obj) => (bool)obj.GetValue(IsHighContrastProperty);

    public static RevealBorderController? GetController(DependencyObject obj) => obj.ReadLocalValue(ControllerProperty) as RevealBorderController;

    private static readonly DependencyProperty ControllerProperty = DependencyProperty.RegisterAttached(
        "Controller", typeof(RevealBorderController), typeof(RevealBorderBehavior), new PropertyMetadata(null));

    private static void OnIsEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not FrameworkElement element)
        {
            return;
        }
        if ((bool)args.NewValue)
        {
            var controller = new RevealBorderController();
            controller.RevealRadius = GetRevealRadius(element);
            controller.HighlightOpacity = GetHighlightOpacity(element);
            controller.IsReducedMotion = GetIsReducedMotion(element);
            controller.IsHighContrast = GetIsHighContrast(element);
            controller.Attach(element);
            element.SetValue(ControllerProperty, controller);
        }
        else if (element.ReadLocalValue(ControllerProperty) is RevealBorderController controller)
        {
            controller.Dispose();
            element.ClearValue(ControllerProperty);
        }
    }

    private static void OnEnvironmentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is FrameworkElement element && element.ReadLocalValue(ControllerProperty) is RevealBorderController controller)
        {
            controller.IsReducedMotion = GetIsReducedMotion(element);
            controller.IsHighContrast = GetIsHighContrast(element);
        }
    }

    private static void OnVisualSettingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is FrameworkElement element && element.ReadLocalValue(ControllerProperty) is RevealBorderController controller)
        {
            controller.RevealRadius = GetRevealRadius(element);
            controller.HighlightOpacity = GetHighlightOpacity(element);
        }
    }
}
