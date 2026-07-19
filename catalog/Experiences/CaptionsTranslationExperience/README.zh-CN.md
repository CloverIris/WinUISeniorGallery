# CaptionsTranslationExperience

中文规范源。当前为 `in-progress` lab，只实现宿主提供的字幕/翻译 Revision 合并、回退和 `TimedTextView` 投影。

## Status

in-progress / lab / P2

## Boundary

不实现 ASR、翻译 Provider、SRT/VTT/LRC 解析、网络或真实媒体字幕声明；这些能力由未来 Provider 抽象和宿主负责。

## Ownership

实现：`src/WinUI3.Senior.Media/CaptionsTranslation/CaptionsTranslationExperience.cs`；演示：`src/WinUI3.Senior.Gallery/Pages/CaptionsTranslationPage.xaml*`。
