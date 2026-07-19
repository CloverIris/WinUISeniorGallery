# GuideMenuExperience

The Chinese document is normative. This `in-progress` lab implements host-neutral layered Guide navigation and a Gallery experiment page.

## Status

in-progress / lab / P2

## Scope

The host supplies a `GuideNode` tree. The control owns open, enter, back, Escape close, and leaf events, but never performs navigation, window changes, or command side effects.

## Ownership

Implementation: `src/WinUI3.Senior.Controls/Experiences/GuideMenu/GuideMenuExperience.cs`; demo: `src/WinUI3.Senior.Gallery/Pages/GuideMenuPage.xaml*`.
