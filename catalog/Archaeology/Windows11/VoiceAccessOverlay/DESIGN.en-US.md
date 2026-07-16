# Voice Access Overlay Modern Design

## Prototype Structure and Design DNA

ListeningStatusBar, RecognizedCommandFeedback, NumberLabels, GridOverlay, Correction/Help panels, and microphone state form a system-level accessibility layer. Preserve: Persistent voice state, immediate command feedback, numbered targets converting UI into speakable addresses, correctable failure, and no precision touch requirement.

## Explicitly Discarded

Gallery explicitly discards system overlay, global input injection, microphone capture, speech recognition, imitation of Voice Access branding/security prompts, and covering other apps; show a local static interaction model only.

## Input and Focus

Buttons/keyboard simulate Listening, Numbers, and Grid without global hotkeys. Focus never moves to number labels; Escape closes innermost overlay and restores trigger. Automatic updates never steal focus or rename Automation; closing an overlay restores an explicit trigger.

## Responsive, RTL, and Automation

Number labels avoid targets/edges and grid recomputes for local demo bounds; RTL affects text, not spoken number order. High Contrast uses solid borders and labels do not obscure focus. At 200% text, status/exit never clips; Automation exposes role, state, position, and next action.

## Theme and High Contrast

Use ThemeResource only. High Contrast does not depend on Acrylic/Mica/shadow/transparency; Reduced Motion removes slide/scale but preserves state.

## Copyright and Asset Exclusion Zone

Commit no Windows 11 screenshot, system icon, wallpaper, sound, font, news content, brand text, or extracted package asset; use owned geometry/fake data.

