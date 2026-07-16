# WindowingIndexPage Specification

## 元数据与不变量

- Feature ID: `pages.windowing-index-page`
- Canonical route: `/windowing`
- Route provider: `route.gallery.windowing`
- Data source: Windowing catalog + current-window capability snapshot
- Included entries: windowing, behavior
- Status remains proposed until implementation and evidence pass review.
- Stable IDs, routes, API names, and resource keys remain English.
- The page uses services scoped to its owning window.

## 目标与非目标

- Users can understand purpose, maturity, and availability without running the feature.
- Every target route comes from registered catalog data.
- Every state provides a visible recovery or explanation.
- The page never mutates the catalog or asset manifest.
- Proposed and preview items are never presented as stable.
- Expensive media/window/provider capabilities never start implicitly.
- No UIElement or page instance crosses window boundaries.
- No vendor SDK type enters the page model.

## 路由与深链接

- Route: `/windowing`
- Allowed parameters: `feature, support`
- Unknown parameters are ignored and recorded once.
- Invalid enum values fall back to the default view.
- Parameters are normalized into stable ordering.
- Back and Forward restore filters, stable selection, and scroll anchor.
- A deep link creates an instance in the target window scope.
- Parameters are never treated as file paths, type names, or external URIs.

## 信息架构与视觉树

1. Page heading and concise purpose.
2. Status summary for catalog/version/capability.
3. Page-specific search, filters, or primary controls.
4. Loading/empty/error/permission state presenter.
5. Virtualized content list, grid, workbench, or legal body.
6. Recovery and secondary actions.
- The heading is the first semantic content node.
- State presenter geometry is stable across transitions.
- Cards expose one primary action and separate metadata.
- Decorative previews are excluded from Automation.

## 页面模型

### WindowingCatalogEntry

- Id: stable identity from the catalog or read-only provider.
- TitleResourceKey: localized visible and Automation title.
- DescriptionResourceKey: localized concise explanation.
- TargetRoute: registered canonical route, never concatenated.
- Status: proposed, specified, ready, in-progress, blocked, review, or done.
- Maturity: lab, preview, or stable.
- Revision: snapshot revision used for latest-wins updates.
- Optional presentation data never changes identity.

### WindowCapabilitySnapshot

- Immutable after publication to the view.
- Carries a monotonically increasing snapshot version.
- Selection uses stable IDs rather than object identity.
- Unknown values remain explicit Unknown values.
- Snapshot replacement preserves surviving selection.

## 页面状态机

### Loading

- Show heading and skeletons after a 100 ms anti-flash delay.
- Loading owns a cancellation token tied to navigation and window close.

### Loaded

- Apply one immutable snapshot on the owning Dispatcher.
- Announce result count once without stealing focus.

### Empty

- 保留能力摘要且不创建额外窗口.
- Recovery remains keyboard accessible.

### Error

- Show a stable error code, correlation ID, Retry, and Return home.
- Invalid entries are isolated while valid entries remain usable.

### Permission or capability

- 不枚举其他进程窗口或请求系统权限.
- Unsupported capability remains discoverable with its fallback.

### Stale

- Catalog revision changes show a non-blocking Refresh action.
- Refresh never jumps while the user edits or scrolls.

## 行为与命令

- Primary command will 先打开详情，再由详情请求窗口操作.
- Pointer, Enter, Space, and gamepad A invoke the same command once.
- Repeated activation is coalesced while navigation is pending.
- Escape closes only the top transient surface.
- Light-dismiss restores focus to its invoker.
- Refresh preserves stable selection and nearest section anchor.
- Ctrl+F focuses search/filter only when the page defines it.
- Unconsumed wheel, arrow, and gamepad input bubbles to the shell.

## 失败与边界

- Catalog unavailable: show Retry and a correlation ID.
- Missing localized string: fall back to en-US or stable ID.
- Missing preview asset: use a themed placeholder, never network download.
- Invalid target route: disable activation but retain readable metadata.
- Obsolete async generation: discard without UI update.
- Callback after window close: cancel rather than redirect to another window.
- Duplicate stable ID: keep the first valid item and report diagnostics.
- Cleanup releases subscriptions, timers, transient surfaces, and heavy assets.
