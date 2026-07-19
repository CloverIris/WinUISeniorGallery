# CaptionsTranslationExperience Acceptance

## Current gate

当前为 `in-progress`；Gallery 使用合成文本，Provider/ASR/翻译实现明确未纳入。

## Given / When / Then

- Given 当前 Revision=2，When 应用 Revision=1，Then 返回 false 且文档和状态不回退。
- Given Source 与 Translation 轨，When 应用新 Revision，Then TimedTextView 可进入 Bilingual 并保留两个轨。
- Given ErrorCode，When 应用 Revision，Then 状态为 Degraded 且最后可接受文本仍可见。
- Given 空模板部件，When 控件运行，Then 纯逻辑 Revision 仍可应用，缺失视觉只降级。
