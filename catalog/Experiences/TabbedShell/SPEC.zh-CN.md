# TabbedShell Specification

## Goal

目标：提供一个不创建窗口的标签容器，统一管理标签选择、可取消关闭、顺序调整和宿主拖出请求。

## Non-goals

不负责创建/迁移 Window、恢复崩溃进程、保存标签内容或执行业务导航。

## Public API

`TabbedShellItem(Id, Header, Content, IconGlyph, CanClose, IsPinned)`；`TabbedShell.Items` 为窗口本地集合；`SelectedItem` 只读绑定入口；`AddTab`、`SelectTab`、`CloseTab`、`MoveTab`、`SelectNext`、`RequestTearOut`。事件为 `SelectionChanged`、`TabClosing`（可取消）、`TabClosed`、`TabReordered`、`TearOutRequested`。

## State model

`Empty`（无标签）、`HasSelection`（有当前标签）；关闭时先进入可取消的 Closing 请求，宿主接受拖出后由宿主建立 Transferring 状态，控件本身不改变集合。

## Template parts and visual tree

必需 `PART_TabList`（ListView）和 `PART_ContentPresenter`（ContentPresenter）。缺少部件时保留数据 API，但不保证视觉呈现；关闭按钮只触发宿主中立的 CloseTab。

## Behavior and failure modes

Ctrl+Tab/Shift+Ctrl+Tab 循环选择，Ctrl+W 请求关闭，Ctrl+T 请求拖出，Ctrl+1…9 选择前九项。重复 ID、未知标签、不可关闭标签和被 `TabClosing.Cancel` 拒绝时不改变集合；最后一个标签关闭后 SelectedItem 为 null。

## Open Decisions

当前仍需专项评审：跨窗口恢复协议、崩溃恢复、拖出失败后的回滚和持久化策略。上述事项不阻塞本地实验实现。

## 场景、数据与视觉树
模型 Tab(Id,Title,ContentKey,Dirty,WindowId)；树 `TabStrip→ContentHost`；状态 Empty/Ready/Dragging/Transferring/Closing，关闭可取消，迁移原子提交。
