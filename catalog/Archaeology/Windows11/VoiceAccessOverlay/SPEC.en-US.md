# Voice Access Overlay Research Specification

## Historical Prototype Structure

Windows 11 Voice Access provides a top status bar, live command feedback, speakable hints, and Show numbers/Show grid overlays for voice-driven targeting. ListeningStatusBar, RecognizedCommandFeedback, NumberLabels, GridOverlay, Correction/Help panels, and microphone state form a system-level accessibility layer.

## Historical States and Focus

```text
Off --> Starting --> Listening <--> Sleeping\nListening --> Recognizing --> CommandAccepted/CommandRejected --> Listening\nListening --> NumbersVisible/GridVisible --> Listening\nAny --> MicrophoneBlocked/Error/Off
```

Buttons/keyboard simulate Listening, Numbers, and Grid without global hotkeys. Focus never moves to number labels; Escape closes innermost overlay and restores trigger.

## Preserved Design DNA

Persistent voice state, immediate command feedback, numbered targets converting UI into speakable addresses, correctable failure, and no precision touch requirement.

## Modernization and Discarded Boundary

Gallery explicitly discards system overlay, global input injection, microphone capture, speech recognition, imitation of Voice Access branding/security prompts, and covering other apps; show a local static interaction model only.

## Modern Feature Owner

Owner registry: unassigned; this archaeology item provides no stable API.

The current feature manifest assigns no modern owner/public_api_name, so the exhibit has no stable Live API Demo and shows an interactive research simulation only. Before specified, establish and review a generic VoiceCommandFeedback/TargetOverlay owner. Archaeology dependencies point only Gallery/Archaeology to owner; stable modules never reference exhibits.

## Gallery Research Tree

```text
ExhibitPage
|- Header (Windows 11 era, proposed/pending)
|- HistoricalStructureAndStateDiagram
|- DesignDnaAndDiscardedBoundary
|- ModernDemo (ResearchSimulation)
|- InputAccessibilityResponsiveMatrix
`- SourcesCopyrightAndDisclaimer
```

## Failure and Promotion Locks

Missing owner/capability/data retains static research and never fakes system behavior. Before specified, lock sources, owner, asset license, differences, Automation semantics, and platform exclusion zone.
