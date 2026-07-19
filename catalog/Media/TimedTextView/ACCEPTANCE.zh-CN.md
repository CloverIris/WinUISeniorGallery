# TimedTextView 验收

## 功能场景

- Given 时间为 Segment 的 Start，Then 该 Segment 活动；Given 时间等于 End，Then 该 Segment 不再活动。
- Given 两个重叠 Segment，Then Start 最晚者活动，Start 相同时按稳定输入顺序。
- Given Karaoke 有有效 Word，Then 只高亮当前 Word/已播放部分；无有效 Word 时高亮整段。
- Given Bilingual 同段有 `TranslatedText`，Then 原文和翻译均显示；翻译缺失时无空白第二行。
- Given 用户调用 Segment，Then 只发一次 `SegmentInvoked`，控件不修改 `Position`。
- Given 手动滚动，Then 自动居中暂停 5 秒且可主动恢复。

## ASR 修订

- 同 Document 更高 Revision 原子替换 Interim 文本并保持稳定 ID 焦点。
- 相同或较低 Revision 被忽略；Document ID 改变后允许 Revision 重新开始。
- 新快照删除 Segment 后虚拟项、索引和 Automation 树均移除旧 ID。
- UI 不记录或泄露识别文本，卸载后不消费迟到更新。

## 大文档与性能

- 10,000 个 Segment 使用 `ItemsRepeater` 虚拟化，不创建等量 XAML 元素；默认只实现视口和前后缓冲。
- 位置以每秒 10 次更新时，不对未变化活动 Segment 重发布事件或重建列表。
- 100 次完整文档修订后无旧快照事件订阅；滚动锚点偏移不超过活动项一行高度。

## 输入、主题与无障碍

- 验证 SingleLine、ScrollingLyrics、Karaoke、Bilingual 的鼠标、触摸、键盘和手柄路径。
- Light、Dark、High Contrast、Reduced Motion、RTL/混合方向、100%–300% DPI 和 200% 文本缩放均无裁切。
- 屏幕阅读器只在活动 Segment 改变时公告，不逐 Word 公告；虚拟化 ListItem 提供位置、时间、语言和修订状态。

## 自动化

单元测试归一化、时间边界、重叠选择、Revision、轨道回退和翻译回退；UI 测试虚拟化、滚动暂停、焦点锚定、模板降级和 AutomationPeer；使用虚拟时钟和不可变假文档。

## 实现证据

- 2026-07-16：Media Release x64、Gallery Debug/Release x64 构建成功；Media 自动化测试通过 9/9。
- Gallery 使用明确标记的合成 Document 与虚拟位置控制；不解析、上传或保存字幕内容。
- 10,000 项、辅助技术与主题/RTL/DPI 矩阵待 review 手工验收，保持 `in-progress`。
