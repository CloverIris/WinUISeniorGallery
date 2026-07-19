# FileCard Acceptance

## Current gate

This is an in-progress lab item: local implementation is allowed but the API is not stable; Chinese/English headings, stable IDs, and API names remain synchronized.

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Given / When / Then
Given a synthetic Descriptor When loaded Then no file is opened; Given a null Thumbnail When rendered Then metadata/actions remain available; Given PreviewRequested When the host leaves Handled=false Then the control does not read anything; Given a disabled action When invoked Then no event is raised; Given Light/Dark/High Contrast/RTL/200% DPI Then name, SizeText, and actions remain readable.
