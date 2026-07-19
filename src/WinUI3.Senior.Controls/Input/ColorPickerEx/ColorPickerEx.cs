using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Windows.UI;
using WindowsColor = Windows.UI.Color;

namespace WinUI3.Senior.Controls;

public enum ColorPickerColorSpace
{
    Rgb,
    Hsv,
    Hsl
}

public sealed class ColorPickerValidationEventArgs : EventArgs
{
    public ColorPickerValidationEventArgs(string text, string error) { Text = text; Error = error; }
    public string Text { get; }
    public string Error { get; }
}

public sealed class ColorPickerColorChangedEventArgs(WindowsColor oldColor, WindowsColor newColor) : EventArgs
{
    public WindowsColor OldColor { get; } = oldColor;
    public WindowsColor NewColor { get; } = newColor;
}

public interface IColorEyedropperProvider
{
    Task<WindowsColor?> PickAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// A dependency-property backed color editor with deterministic text parsing,
/// bounded history and RGB/HSV/HSL channel conversion. Screen capture remains host-owned.
/// </summary>
public sealed class ColorPickerEx : Control
{
    public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(WindowsColor), typeof(ColorPickerEx), new PropertyMetadata(Colors.White, OnColorChanged));
    public static readonly DependencyProperty ColorSpaceProperty = DependencyProperty.Register(nameof(ColorSpace), typeof(ColorPickerColorSpace), typeof(ColorPickerEx), new PropertyMetadata(ColorPickerColorSpace.Rgb));
    public static readonly DependencyProperty IsAlphaEnabledProperty = DependencyProperty.Register(nameof(IsAlphaEnabled), typeof(bool), typeof(ColorPickerEx), new PropertyMetadata(true, OnAlphaEnabledChanged));
    public static readonly DependencyProperty HexTextProperty = DependencyProperty.Register(nameof(HexText), typeof(string), typeof(ColorPickerEx), new PropertyMetadata("#FFFFFFFF"));
    public static readonly DependencyProperty MaxHistoryLengthProperty = DependencyProperty.Register(nameof(MaxHistoryLength), typeof(int), typeof(ColorPickerEx), new PropertyMetadata(12, OnHistoryLengthChanged));

    private string? _validationError;

    public ColorPickerEx()
    {
        DefaultStyleKey = typeof(ColorPickerEx);
        Palette = new ObservableCollection<WindowsColor>();
        History = new ObservableCollection<WindowsColor>();
        EyedropperProvider = null;
        UpdateHexText(Color);
    }

    public WindowsColor Color { get => (WindowsColor)GetValue(ColorProperty); set => SetValue(ColorProperty, value); }
    public ColorPickerColorSpace ColorSpace { get => (ColorPickerColorSpace)GetValue(ColorSpaceProperty); set => SetValue(ColorSpaceProperty, value); }
    public bool IsAlphaEnabled { get => (bool)GetValue(IsAlphaEnabledProperty); set => SetValue(IsAlphaEnabledProperty, value); }
    public string HexText { get => (string?)GetValue(HexTextProperty) ?? string.Empty; set => SetValue(HexTextProperty, value); }
    public int MaxHistoryLength { get => (int)GetValue(MaxHistoryLengthProperty); set => SetValue(MaxHistoryLengthProperty, value); }
    public ObservableCollection<WindowsColor> Palette { get; }
    public ObservableCollection<WindowsColor> History { get; }
    public IColorEyedropperProvider? EyedropperProvider { get; set; }
    public bool IsPicking { get; private set; }
    public bool IsValid => string.IsNullOrEmpty(_validationError);
    public string? ValidationError => _validationError;

    public event EventHandler? ColorChanged;
    public event EventHandler<ColorPickerColorChangedEventArgs>? ColorChangedDetailed;
    public event EventHandler? ColorCommitted;
    public event EventHandler<ColorPickerValidationEventArgs>? ValidationFailed;
    public event EventHandler? HistoryChanged;

    protected override AutomationPeer OnCreateAutomationPeer() => new ColorPickerAutomationPeer(this);

    public bool TryCommitHexText()
    {
        if (!TryParse(HexText, out var parsed, out var error))
        {
            SetValidationError(error);
            return false;
        }

        if (!IsAlphaEnabled) parsed.A = Color.A;
        SetValidationError(null);
        if (!TrySetColor(parsed, addToHistory: true)) return false;
        ColorCommitted?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool TrySetColor(WindowsColor color, bool addToHistory = true)
    {
        if (!IsAlphaEnabled) color.A = Color.A;
        Color = color;
        if (addToHistory) AddToHistory(color);
        SetValidationError(null);
        return true;
    }

    public void SelectPaletteColor(WindowsColor color) => TrySetColor(color, addToHistory: true);

    public void AddToHistory(WindowsColor color)
    {
        var existingIndex = History.ToList().FindIndex(item => item.A == color.A && item.R == color.R && item.G == color.G && item.B == color.B);
        if (existingIndex >= 0) History.RemoveAt(existingIndex);
        History.Insert(0, color);
        TrimHistory();
        HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ClearHistory()
    {
        if (History.Count == 0) return;
        History.Clear();
        HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    public async Task<bool> PickColorAsync(CancellationToken cancellationToken = default)
    {
        if (EyedropperProvider is null) return false;
        IsPicking = true;
        try
        {
            var picked = await EyedropperProvider.PickAsync(cancellationToken).ConfigureAwait(true);
            return picked.HasValue && TrySetColor(picked.Value, addToHistory: true);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { return false; }
        finally { IsPicking = false; }
    }

    public void SetChannels(double first, double second, double third, byte? alpha = null)
    {
        first = Math.Clamp(first, 0, ColorSpace == ColorPickerColorSpace.Rgb ? 255 : 360);
        second = Math.Clamp(second, 0, ColorSpace == ColorPickerColorSpace.Rgb ? 255 : 100);
        third = Math.Clamp(third, 0, ColorSpace == ColorPickerColorSpace.Rgb ? 255 : 100);
        var color = ColorSpace switch
        {
            ColorPickerColorSpace.Hsv => FromHsv(first, second / 100d, third / 100d, alpha ?? Color.A),
            ColorPickerColorSpace.Hsl => FromHsl(first, second / 100d, third / 100d, alpha ?? Color.A),
            _ => Color.FromArgb(alpha ?? Color.A, (byte)Math.Round(first), (byte)Math.Round(second), (byte)Math.Round(third))
        };
        TrySetColor(color, addToHistory: false);
    }

    public (double First, double Second, double Third, byte Alpha) GetChannels()
    {
        return ColorSpace switch
        {
            ColorPickerColorSpace.Hsv => ToHsvWithAlpha(Color),
            ColorPickerColorSpace.Hsl => ToHslWithAlpha(Color),
            _ => (Color.R, Color.G, Color.B, Color.A)
        };
    }

    public static string FormatHex(WindowsColor color, bool includeAlpha = true) => includeAlpha
        ? $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}"
        : $"#{color.R:X2}{color.G:X2}{color.B:X2}";

    public static bool TryParse(string? text, out WindowsColor color, out string error)
    {
        color = Colors.Transparent;
        error = string.Empty;
        if (string.IsNullOrWhiteSpace(text)) { error = "Enter a color value."; return false; }
        var value = text.Trim();
        if (value.StartsWith('#'))
        {
            value = value[1..];
            var shortLength = value.Length;
            if (shortLength is 3 or 4) value = string.Concat(value.Select(ch => $"{ch}{ch}"));
            if (value.Length is not (6 or 8) || !uint.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var number)) { error = "Use #RGB, #RGBA, #RRGGBB or #AARRGGBB."; return false; }
            color = shortLength == 4
                ? Color.FromArgb((byte)number, (byte)(number >> 24), (byte)(number >> 16), (byte)(number >> 8))
                : value.Length == 6
                    ? Color.FromArgb(255, (byte)(number >> 16), (byte)(number >> 8), (byte)number)
                    : Color.FromArgb((byte)(number >> 24), (byte)(number >> 16), (byte)(number >> 8), (byte)number);
            return true;
        }

        var open = value.IndexOf('(');
        if (open > 0 && value.EndsWith(')'))
        {
            var name = value[..open].Trim().ToLowerInvariant();
            var values = value[(open + 1)..^1].Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (name is "rgb" or "rgba" && values.Length is 3 or 4 && TryByte(values[0], out var r) && TryByte(values[1], out var g) && TryByte(values[2], out var b))
            {
                byte a = 255;
                if (values.Length == 4 && !TryAlpha(values[3], out a))
                {
                    error = "Alpha must be between 0 and 1 or 0 and 255.";
                    return false;
                }
                color = Color.FromArgb(a, r, g, b); return true;
            }
            if (name is "hsv" or "hsva" or "hsl" or "hsla" && values.Length is 3 or 4
                && TryDouble(values[0], 0, 360, out var h)
                && TryDouble(values[1], 0, 100, out var s)
                && TryDouble(values[2], 0, 100, out var l))
            {
                byte a = 255;
                if (values.Length == 4 && !TryAlpha(values[3], out a))
                {
                    error = "Alpha must be between 0 and 1 or 0 and 255.";
                    return false;
                }
                color = name.StartsWith("hsv", StringComparison.Ordinal)
                    ? FromHsv(h, s / 100d, l / 100d, a)
                    : FromHsl(h, s / 100d, l / 100d, a);
                return true;
            }
        }
        error = "Unsupported color syntax.";
        return false;
    }

    private void UpdateHexText(WindowsColor color) => SetValue(HexTextProperty, FormatHex(color, IsAlphaEnabled));
    private void SetValidationError(string? error)
    {
        _validationError = error;
        if (error is not null) ValidationFailed?.Invoke(this, new ColorPickerValidationEventArgs(HexText, error));
    }
    private void TrimHistory()
    {
        var max = Math.Clamp(MaxHistoryLength, 0, 128);
        while (History.Count > max) History.RemoveAt(History.Count - 1);
    }
    private static bool TryByte(string text, out byte value)
    {
        var trimmed = text.Trim();
        if (trimmed.EndsWith('%') && double.TryParse(trimmed[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var percent))
        {
            value = (byte)Math.Round(Math.Clamp(percent, 0, 100) * 2.55);
            return true;
        }
        return byte.TryParse(trimmed, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }
    private static bool TryDouble(string text, double min, double max, out double value)
        => double.TryParse(text.TrimEnd('%'), NumberStyles.Float, CultureInfo.InvariantCulture, out value)
            && double.IsFinite(value) && value >= min && value <= max;
    private static bool TryAlpha(string text, out byte value)
    {
        var trimmed = text.Trim();
        if (trimmed.EndsWith('%') && double.TryParse(trimmed[..^1], NumberStyles.Float, CultureInfo.InvariantCulture, out var percent))
        {
            value = (byte)Math.Round(Math.Clamp(percent, 0, 100) * 2.55);
            return true;
        }
        if (!double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out var numeric) || !double.IsFinite(numeric))
        {
            value = 255;
            return false;
        }
        value = (byte)Math.Round(Math.Clamp(numeric <= 1 ? numeric * 255 : numeric, 0, 255));
        return true;
    }
    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var picker = (ColorPickerEx)d;
        picker.UpdateHexText((WindowsColor)e.NewValue);
        picker.ColorChanged?.Invoke(picker, EventArgs.Empty);
        picker.ColorChangedDetailed?.Invoke(picker, new ColorPickerColorChangedEventArgs((WindowsColor)e.OldValue, (WindowsColor)e.NewValue));
    }
    private static void OnAlphaEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var picker = (ColorPickerEx)d;
        picker.UpdateHexText(picker.Color);
    }
    private static void OnHistoryLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ColorPickerEx)d).TrimHistory();

    private static (double First, double Second, double Third) ToHsv(WindowsColor c)
    {
        var r = c.R / 255d; var g = c.G / 255d; var b = c.B / 255d; var max = Math.Max(r, Math.Max(g, b)); var min = Math.Min(r, Math.Min(g, b)); var delta = max - min;
        var hue = delta == 0 ? 0 : max == r ? 60 * (((g - b) / delta) % 6) : max == g ? 60 * ((b - r) / delta + 2) : 60 * ((r - g) / delta + 4);
        if (hue < 0) hue += 360;
        return (hue, max == 0 ? 0 : delta / max * 100, max * 100);
    }
    private static (double First, double Second, double Third) ToHsl(WindowsColor c)
    {
        var r = c.R / 255d; var g = c.G / 255d; var b = c.B / 255d; var max = Math.Max(r, Math.Max(g, b)); var min = Math.Min(r, Math.Min(g, b)); var delta = max - min; var light = (max + min) / 2; var sat = delta == 0 ? 0 : delta / (1 - Math.Abs(2 * light - 1));
        var hue = delta == 0 ? 0 : max == r ? 60 * (((g - b) / delta) % 6) : max == g ? 60 * ((b - r) / delta + 2) : 60 * ((r - g) / delta + 4); if (hue < 0) hue += 360;
        return (hue, sat * 100, light * 100);
    }
    private static (double First, double Second, double Third, byte Alpha) ToHsvWithAlpha(WindowsColor color)
    {
        var channels = ToHsv(color);
        return (channels.First, channels.Second, channels.Third, color.A);
    }
    private static (double First, double Second, double Third, byte Alpha) ToHslWithAlpha(WindowsColor color)
    {
        var channels = ToHsl(color);
        return (channels.First, channels.Second, channels.Third, color.A);
    }
    private static WindowsColor FromHsv(double hue, double saturation, double value, byte alpha)
    {
        var c = value * saturation; var x = c * (1 - Math.Abs(hue / 60 % 2 - 1)); var m = value - c; var (r, g, b) = hue switch { < 60 => (c, x, 0d), < 120 => (x, c, 0d), < 180 => (0d, c, x), < 240 => (0d, x, c), < 300 => (x, 0d, c), _ => (c, 0d, x) }; return Color.FromArgb(alpha, ToByte(r + m), ToByte(g + m), ToByte(b + m));
    }
    private static WindowsColor FromHsl(double hue, double saturation, double lightness, byte alpha)
    {
        var c = (1 - Math.Abs(2 * lightness - 1)) * saturation; var x = c * (1 - Math.Abs(hue / 60 % 2 - 1)); var m = lightness - c / 2; var (r, g, b) = hue switch { < 60 => (c, x, 0d), < 120 => (x, c, 0d), < 180 => (0d, c, x), < 240 => (0d, x, c), < 300 => (x, 0d, c), _ => (c, 0d, x) }; return Color.FromArgb(alpha, ToByte(r + m), ToByte(g + m), ToByte(b + m));
    }
    private static byte ToByte(double value) => (byte)Math.Round(Math.Clamp(value, 0, 1) * 255);
}

internal sealed class ColorPickerAutomationPeer : FrameworkElementAutomationPeer
{
    private readonly ColorPickerEx _owner;
    public ColorPickerAutomationPeer(ColorPickerEx owner) : base(owner) => _owner = owner;
    protected override string GetClassNameCore() => nameof(ColorPickerEx);
    protected override string GetNameCore() => $"{nameof(ColorPickerEx)} {_owner.HexText}";
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Custom;
}
