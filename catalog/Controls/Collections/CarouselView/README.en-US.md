# CarouselView

`CarouselView` is a virtualized single-selection carousel for hero banners, album covers, onboarding, and adjacent-content previews. It supplies bounded or looping navigation, autoplay, mouse/keyboard/touch/controller input, and replaceable transitions; it does not fetch data, cache images, navigate pages, or play media.

## Status

- Work item: `controls.carousel-view`
- Maturity: `lab`; priority: `P0`; implementation status: `in-progress`
- Package and namespace: `WinUI3.Senior.Controls` / `WinUI3.Senior.Controls`
- Gallery route: `/controls/carousel-view`

## Scope

The implementation retains only the current item and source-index neighbours requested by `RealizationBuffer`. Looping maps indexes with modulo arithmetic and never creates a duplicate collection. `CarouselView` derives from the C#-activatable `ListView`; it retains `Selector` single-selection semantics and therefore does not derive directly from the non-activatable `Selector` type.

## Dependencies and ownership

It depends on the Theme, Motion, Input, and Accessibility Contracts. An implementation agent may change only the `owned_paths` in `feature.json`. Missing public Contracts require a `blocked` work item, not a private incompatible API.

## Documents

- [Specification](SPEC.en-US.md)
- [Design](DESIGN.en-US.md)
- [Integration](INTEGRATION.en-US.md)
- [Acceptance](ACCEPTANCE.en-US.md)

Chinese is the normative source; English files are synchronized translations.

## Implementation gate

This item is `in-progress`. Only after the control, template, automation, Gallery lab, unpackaged sample, tests, and acceptance evidence are complete may a maintainer advance it to `review`.
