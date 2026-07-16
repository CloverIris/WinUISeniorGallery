# CarouselView Integration

## Global Contracts

Use the Theme, Motion, Input, and Accessibility Contracts. Resource keys are registered only in the Controls `Generic.xaml`. Gallery, samples, and application hosts may use only public properties, events, and commands; they must not reach internal implementation elements or `CarouselVirtualizingLayout`.

## Host and window boundary

A host must write `IsHostWindowActive` from its own `Window.Activated`/deactivation path. The control never locates a current window, creates a window, requests activation, or routes state to another window. Each `CarouselView` owns independent autoplay timing, selection state, and diagnostic count; unload stops its timer and cancels Composition animation.

## Data, threading, and lifecycle

`ItemsSource`, collection notifications, template properties, navigation, and all dependency properties run on the UI thread. A background producer must first dispatch changes to the owning `DispatcherQueue`. After replacement of an item source or template, the internal repeater synchronizes during its next layout without copying data. `RealizedElementCount` is a Lab diagnostic snapshot, not a stable layout promise, and must not drive business logic.

## Platform, permissions, and degradation

The control depends only on Windows App SDK XAML, ItemsRepeater, input events, and Composition; it requires no network, file, media, location, microphone, or capability declaration. When Composition or animation preferences are unavailable, it uses a short fade or no animation. Window inactivity, invisibility, focus, hover, and user interaction pause autoplay according to the specification.

## Privacy and diagnostics

The control collects no telemetry, records no user input, stores no data, and sends no network request. Diagnostics expose only `AutoplayPauseReason` and `RealizedElementCount`; a host that records them is responsible for consent and privacy policy.
