namespace WinUI3.Senior.Media;

/// <summary>Template-facing immutable segment projection used by the virtualized lyrics list.</summary>
internal sealed class TimedTextSegmentItem
{
    public TimedTextSegmentItem(TimedTextSegment segment, bool isActive)
    {
        Segment = segment;
        IsActive = isActive;
    }

    public TimedTextSegment Segment { get; }
    public bool IsActive { get; set; }
    public string Text => Segment.Text;
    public string? Translation => Segment.TranslatedText;
    public string AutomationName => $"{Segment.Start:c} {Segment.Text}";
}
