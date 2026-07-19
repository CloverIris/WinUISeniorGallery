using System;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;

namespace WinUI3.Senior.Media;

internal sealed class MediaTimelineAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider
{
    private readonly MediaTimeline _owner;

    public MediaTimelineAutomationPeer(MediaTimeline owner) : base(owner) => _owner = owner;

    public bool IsReadOnly => _owner.Mode == MediaTimelineMode.Live || !_owner.IsSeekEnabled || !_owner.IsEnabled || _owner.Maximum < _owner.Minimum;
    public double LargeChange => Math.Max(0d, _owner.LargeKeyboardStep.TotalSeconds);
    public double Maximum => _owner.Maximum.TotalSeconds;
    public double Minimum => _owner.Minimum.TotalSeconds;
    public double SmallChange => Math.Max(0d, _owner.KeyboardStep.TotalSeconds);
    public double Value => _owner.Position.TotalSeconds;

    protected override string GetClassNameCore() => nameof(MediaTimeline);
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Slider;
    public void SetValue(double value)
    {
        if (IsReadOnly)
        {
            throw new InvalidOperationException("The live or disabled timeline is read-only.");
        }
        if (value < Minimum || value > Maximum)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        _owner.SetValueFromAutomation(value);
    }

    internal void RaiseValueChanged() => RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, null, Value);
}
