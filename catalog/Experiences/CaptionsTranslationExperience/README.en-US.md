# CaptionsTranslationExperience

The Chinese document is normative. This `in-progress` lab implements only host-provided caption/translation revision merging, fallback, and `TimedTextView` projection.

## Status

in-progress / lab / P2

## Boundary

No ASR, translation provider, SRT/VTT/LRC parsing, network access, or claim that displayed text belongs to real media. Providers and hosts own those capabilities.

## Ownership

Implementation: `src/WinUI3.Senior.Media/CaptionsTranslation/CaptionsTranslationExperience.cs`; demo: `src/WinUI3.Senior.Gallery/Pages/CaptionsTranslationPage.xaml*`.
