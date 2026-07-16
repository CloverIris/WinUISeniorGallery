# CarouselView Acceptance

## Current validation record

- Passed: Controls Release x64, Gallery Debug x64, Gallery Release x64, ControlsMinimal Debug x64, Catalog Validator (124 work items), and Architecture Tests (4/4).
- Built: the WinUI Unit Test App test host builds at Debug x64 with zero errors and retains `UITestMethod`, `WinUITestTarget`, Windows App SDK bootstrap, and x64 RID configuration.
- Outstanding: the current `dotnet test` VSTest launch path does not start the WinUI Unit Test App, so it cannot supply the XAML UI thread required by `UITestMethod`; this is not a Carousel assertion failure. UI cases must run through the Visual Studio Test Explorer WinUI Unit Test App path, and the 1,000-item P95 and theme/DPI manual acceptance must be recorded before the item may enter `review`.

## Functional scenarios

- Given 1000 lightweight logical items and the default buffer, when first presented and navigated 100 times, then `RealizedElementCount` reflects only the selected item and its buffer rather than growing linearly with total item count.
- Given Loop with the final item selected, when `MoveNext` runs, then exactly one selection change reaches index 0 and the source collection is not copied; given Bounded at the final item, when `MoveNext` or autoplay expires, then the final item remains selected and repeated timing stops.
- Given `ItemsSource`, `ItemTemplate`, or `ItemTemplateSelector` is replaced at runtime, when the next layout runs, then the repeater synchronizes the new configuration without precreating all containers.
- Given the selected object remains after a collection change, when another item is inserted, removed, or moved, then that object remains selected; given the selected object is removed, when a successor or new final item exists, then it is selected according to the specification; given an empty collection, then selection is `-1`.
- Given autoplay is enabled, when the control is invisible, `IsHostWindowActive=false`, focus is within, pointer hovers, or a user drags, then it pauses in fixed precedence; when the condition recovers, then it waits a full interval before moving.
- Given RTL, when left/right keys, a swipe, and Previous/Next buttons are invoked, then visual direction mirrors while index rules remain consistent.

## Automation and input

- Given a selected item, when Enter, Space, or controller A is used, then `ItemInvoked` has the correct `Item`, `Index`, and `InputDeviceKind`; an unreliably identifiable precision touchpad reports `Unknown`.
- Given horizontal wheel or Shift+wheel, when it scrolls, then navigation occurs and the event is handled; given ordinary vertical wheel, then the event continues to bubble.
- Given the required repeater is absent, when the template applies, then a clear exception is thrown; given an optional part is absent, then the control remains operable with that feature degraded.
- Given user navigation, when SelectionChanged occurs, then the AutomationPeer exposes Selection, Scroll, and ItemContainer and the Live Region announces once; given autoplay, then it does not announce.

## Quality matrix

- Light, Dark, High Contrast; Chinese, English, RTL; 320/800/1920 effective pixels; 100%/150%/200% DPI; text scaling and Reduced Motion remain operable.
- Mouse, wheel, keyboard, touch, precision touchpad, and Xbox controller have defined behaviour or an explicit `Unknown` device report.
- In Release x64 with 1000 lightweight items and 100 consecutive navigations, target P95 UI frame time is at most 25ms; tests record machine, DPI, window width, template, and measurement.
- After unload, no growing timer, event subscription, Composition batch, or realized-element reference remains.

## Automated tests

- Pure logic: index mapping, empty/single item, bounded/loop, parameter validation, collection selection repair, pause precedence, and full-interval reset.
- XAML UI thread: template parts, optional-part degradation, 1000-item realization count, keyboard RTL, item invocation, focus/hover/window activity, Automation, and no autoplay announcements.
- Before delivery, run Controls Release x64, Gallery Debug/Release x64, Catalog Validator, Architecture Tests, and the Controls test host; failure keeps `in-progress` and records evidence.

## Current evidence

The status remains `in-progress`. This revision records implementation targets only; real build, UI-test, performance, and Gallery-manual-validation results must be added by the root agent, and no `review` status is permitted before they pass.
