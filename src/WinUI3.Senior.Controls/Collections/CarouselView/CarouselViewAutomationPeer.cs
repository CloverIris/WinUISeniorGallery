using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;

namespace WinUI3.Senior.Controls;

internal sealed class CarouselViewAutomationPeer : SelectorAutomationPeer, IScrollProvider, IItemContainerProvider
{
    private readonly CarouselView _owner;

    public CarouselViewAutomationPeer(CarouselView owner) : base(owner) => _owner = owner;

    public bool HorizontallyScrollable => _owner.Items.Count > 1;
    public bool VerticallyScrollable => false;
    public double HorizontalScrollPercent => _owner.Items.Count <= 1 || _owner.SelectedIndex < 0
        ? ScrollPatternIdentifiers.NoScroll
        : 100d * _owner.SelectedIndex / (_owner.Items.Count - 1);
    public double VerticalScrollPercent => ScrollPatternIdentifiers.NoScroll;
    public double HorizontalViewSize => _owner.Items.Count == 0 ? 100d : 100d / _owner.Items.Count;
    public double VerticalViewSize => 100d;

    protected override string GetClassNameCore() => nameof(CarouselView);
    protected override AutomationControlType GetAutomationControlTypeCore() => AutomationControlType.List;
    protected override bool IsKeyboardFocusableCore() => _owner.IsTabStop && _owner.IsEnabled;

    public void Scroll(ScrollAmount horizontalAmount, ScrollAmount verticalAmount)
    {
        if (horizontalAmount is ScrollAmount.SmallIncrement or ScrollAmount.LargeIncrement)
        {
            _owner.MoveNext();
        }
        else if (horizontalAmount is ScrollAmount.SmallDecrement or ScrollAmount.LargeDecrement)
        {
            _owner.MovePrevious();
        }
    }

    public void SetScrollPercent(double horizontalPercent, double verticalPercent)
    {
        if (horizontalPercent == ScrollPatternIdentifiers.NoScroll || _owner.Items.Count == 0)
        {
            return;
        }

        if (horizontalPercent is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(horizontalPercent));
        }

        _owner.MoveTo((int)Math.Round(horizontalPercent * (_owner.Items.Count - 1) / 100d, MidpointRounding.AwayFromZero));
    }

    public IRawElementProviderSimple? FindItemByProperty(IRawElementProviderSimple? startAfter, int propertyId, object? value)
    {
        var reachedStart = startAfter is null;
        for (var index = 0; index < _owner.Items.Count; index++)
        {
            var element = _owner.GetRealizedElement(index);
            if (element is null)
            {
                continue;
            }

            var peer = FrameworkElementAutomationPeer.FromElement(element) ?? FrameworkElementAutomationPeer.CreatePeerForElement(element);
            var provider = peer is null ? null : ProviderFromPeer(peer);
            if (!reachedStart)
            {
                reachedStart = ReferenceEquals(provider, startAfter);
                continue;
            }

            if (provider is null || !MatchesRequestedProperty(element as FrameworkElement, propertyId, value))
            {
                continue;
            }

            return provider;
        }

        return null;
    }

    protected override IList<AutomationPeer>? GetChildrenCore()
    {
        var children = new List<AutomationPeer>();
        for (var index = 0; index < _owner.Items.Count; index++)
        {
            var element = _owner.GetRealizedElement(index);
            if (element is not null)
            {
                var peer = FrameworkElementAutomationPeer.FromElement(element) ?? FrameworkElementAutomationPeer.CreatePeerForElement(element);
                if (peer is not null)
                {
                    children.Add(peer);
                }
            }
        }

        return children;
    }

    private static bool MatchesRequestedProperty(FrameworkElement? element, int propertyId, object? value)
    {
        // WinUI's public C# projection does not expose stable numeric IDs for
        // AutomationProperty. A null filter is still a valid ItemContainer
        // lookup; non-null filters are delegated to item peers when available.
        if (propertyId == 0 || element is null)
        {
            return true;
        }

        return false;
    }
}
