using Microsoft.UI.Xaml.Automation.Peers;

namespace WinUI3.Senior.Media;

/// <summary>Exposes the container as a semantic timed-text reader without announcing every word update.</summary>
internal sealed class TimedTextViewAutomationPeer : FrameworkElementAutomationPeer
{
    public TimedTextViewAutomationPeer(TimedTextView owner)
        : base(owner)
    {
    }

    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.Text;

    protected override string GetClassNameCore() => nameof(TimedTextView);

    protected override string GetNameCore() => "Timed text";
}
