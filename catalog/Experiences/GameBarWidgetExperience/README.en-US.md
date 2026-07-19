# GameBarWidgetExperience

The Chinese document is normative. This `in-progress` lab provides host-owned floating-widget lifecycle and recoverable click-through interaction.

## Status

in-progress / lab / P2

## Scope

The control manages Interactive, ClickThrough, Minimized, and Closed states. Click-through always requires a recovery hotkey and host acknowledgement; the control does not alter hit testing or create windows.

## Ownership

Implementation: `src/WinUI3.Senior.Windowing/GameBarWidgetExperience.cs`; demo: `src/WinUI3.Senior.Gallery/Pages/GameBarWidgetPage.xaml*`.
