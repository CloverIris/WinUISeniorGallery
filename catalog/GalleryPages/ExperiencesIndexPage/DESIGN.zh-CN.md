# ExperiencesIndexPage Design

## 视觉原则

- Fluent Modern is the default visual language.
- Historical/media styling appears only in labeled preview regions.
- Heading, description, primary action, and status form fixed hierarchy.
- All visual values come from semantic ThemeResource tokens.
- Explanatory text is constrained to readable measure.
- State badges combine text with icon or shape.
- Decorative imagery carries no unique information.
- Cards use content-driven height.

## 响应式布局

- Narrow 320–639 epx: one column and filter sheet.
- Medium 640–1023 epx: one or two columns by minimum card width.
- Wide 1024+ epx: bounded content with optional side rail.
- 200% text scale selects the next narrower layout before clipping.
- 400% display scale preserves every primary action.
- Height below 480 epx keeps heading readable while body scrolls.
- DPI/orientation change preserves stable selection and anchor.
- Required status and legal text is never hidden at narrow width.
- Minimum hit target is 40×40 epx; preferred touch target is 44×44.

## 视觉树与组件

- Root has one page scroll owner unless a workbench explicitly owns nested scrolling.
- H1 precedes filters, result summary, content, and footer.
- Loading skeletons approximate final card geometry.
- Empty/error states reuse the content region rather than dialogs.
- Every card has localized title, description, status, and one command.
- Secondary metadata is a semantic list.
- Inline help follows its control in reading order.
- Preview assets resolve through the manifest only.
- Missing assets use a themed semantic placeholder.

## 焦点与交互

- Primary activation will 打开体验规格，依赖完整时才允许运行演示.
- Keyboard/deep-link arrival focuses H1; pointer arrival preserves pointer semantics.
- Focus order follows visual/document order.
- Refresh never steals focus.
- Removed focused content transfers focus to nearest surviving item.
- Transient surfaces restore focus to their invoking button.
- Disabled actions have adjacent visible explanations.
- Shift+F10 exposes the same secondary actions as pointer context menus.
- Double click/tap never executes twice.

## 输入矩阵

- Mouse click activates; wheel scrolls nearest eligible container.
- Hover never reveals required-only information.
- Tab traverses logical order; Enter/Space activate; Escape dismisses.
- Arrow keys follow active list/grid semantics.
- Touch tap activates and pan scrolls; no edge-only gesture is required.
- Touchpad precision scrolling follows system settings.
- Gamepad XY focus has explicit overrides at region boundaries.
- Pen tap matches touch; barrel actions have keyboard alternatives.
- Unconsumed input bubbles to the shell.

## 自动化与无障碍

- Root Automation name includes ExperiencesIndexPage.
- H1 and sections expose Heading semantics.
- Cards expose Name, Description, Status, PositionInSet, and Invoke.
- Skeletons are excluded from the accessibility tree.
- Loaded result count is announced once with polite priority.
- Error is announced once; correlation ID is selectable.
- Filter announcements coalesce for 300 ms.
- Focus visuals use system resources and remain visible.
- Color, position, material, and motion are never sole information channels.
- Reduced Motion exposes identical commands and state.

## 主题、High Contrast 与 RTL

- Light and Dark share semantic hierarchy.
- High Contrast removes Acrylic/Mica dependency and shadows.
- Selected/error states retain visible system-color boundaries.
- RTL mirrors layout, chevrons, and navigation order.
- IDs, code, versions, and product names preserve semantic direction.
- Historical imagery is not mechanically mirrored.
- Missing theme tokens fall back to WinUI system resources.
- Live theme changes preserve page instance and selection.

## 动效与过渡

- Loading-to-content uses opacity without moving reading position.
- Reduced Motion removes translation, parallax, and shimmer.
- Animation never gates command completion.
- Interrupted transitions settle on the latest state.
- Error changes never flash more than three times per second.
- High Contrast disables skeleton shimmer.
- Unload cancels composition callbacks.
- Automation state updates are independent of visual duration.
