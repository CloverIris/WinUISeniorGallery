# MediaCenterExperience Specification

## Goal

组合 `MediaCenterGrid` 与详情覆盖层，提供 Category → Item 浏览和明确的 Playback 意图事件。

## Non-goals

不创建播放器、不读取媒体文件、不执行导航、不联网、不持久化分类或播放状态。

## Public API

`MediaCenterItem`、`MediaCenterCategory`、`Categories`、`SelectedCategoryIndex`、`SelectedCategory`、`SelectedItem`、`IsDetailsOpen`、`State`、`SetCategories`、`SelectCategory`、`SelectItem`、`CloseDetails`、`RequestPlayback`。`SelectionRequested` 同时用于详情选择和播放意图，宿主必须自行区分上下文。

## Behavior and visual tree

`PART_Grid` 显示当前分类；选择条目进入 Details；Play 只触发事件并进入 Playback 状态；Close 返回 Browse。空分类为 Empty，重复 ID 去重，未知分类/条目返回 false。
