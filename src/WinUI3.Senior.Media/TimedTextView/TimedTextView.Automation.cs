using Microsoft.UI.Xaml.Automation.Peers;

namespace WinUI3.Senior.Media;

public sealed partial class TimedTextView
{
    protected override AutomationPeer OnCreateAutomationPeer() => new TimedTextViewAutomationPeer(this);
}
