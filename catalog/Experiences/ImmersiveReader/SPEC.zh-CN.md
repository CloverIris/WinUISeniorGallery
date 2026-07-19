# ImmersiveReader Specification

## Goal

目标：用虚拟化段落块提供沉浸阅读、焦点锚定、行聚焦和宿主中立的朗读请求。

## Non-goals

不解析文件、不改变原文、不创建 SpeechSynthesizer，也不持久化阅读位置。

## Public API

`ReaderBlock(Id, Text, IsHeading, HeadingLevel, Tag)`；`ImmersiveReader.SetBlocks`、`FocusBlock`、`MoveFocus`、`ToggleReading`；属性 `FocusMode`、`FontScale`、`IsLineFocusEnabled`；事件 `BlockInvoked`、`FocusChanged`、`SpeechRequested`（可标记 Handled）。

## State model

`Empty`、`Ready`、`Idle/Reading`；焦点变化只移动视图和公告当前块，朗读状态表示请求状态，不代表平台语音已执行。

## Template parts and visual tree

必需 `PART_ItemsRepeater` 和 `PART_LiveRegion`。ItemsRepeater 负责大文档实现窗口；缺少部件时数据与焦点 API 仍可用。

## Behavior and failure modes

重复 Id 保留第一块；空文本不进入视觉集合。Up/Down 移动焦点，Enter/Space 公告用户调用，R 发出朗读请求；字体缩放限制 0.8–2.5，行聚焦只改变视觉不改变模型。

## Open Decisions

仍需评审：句子/单词级模型、真实 Speech Provider、文档解析和隐私策略；当前实现聚焦块级 P2 实验。

## 场景、数据与视觉树
模型 Document→Block→Sentence→Word；树 `Toolbar→DocumentPresenter→FocusMask`；状态 Loading/Reading/Speaking/Paused/Error，原文不可被显示设置修改。
