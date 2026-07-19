# AchievementToast Acceptance

## Current gate

This is an in-progress lab item: local implementation is allowed but the API is not stable. Chinese and English headings, stable IDs, and API names remain synchronized.

## Common matrix

Light, Dark, High Contrast, DPI, keyboard, mouse, touch, Narrator, Reduced Motion, Chinese, English, and RTL.

## Automated scenarios
Given two windows When each calls ShowAsync Then queues remain isolated; Given 32+ requests When queued Then overflow returns queue-full; Given the current item When timeout/Dismiss occurs Then it completes once and advances; Given Dispose When work is pending Then all waiters resolve HostDestroyed; Given theme or Reduced Motion changes Then content remains readable without timer leaks.
