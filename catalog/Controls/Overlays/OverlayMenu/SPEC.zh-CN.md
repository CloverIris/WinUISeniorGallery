# OverlayMenu Specification

## Goal

目标：提供不离开当前内容上下文的层级覆盖菜单。菜单拥有打开状态和导航栈，宿主拥有项目数据与最终动作。

## Non-goals

不实现窗口创建、系统级 Charms、路由持久化或云命令。

## Public API

`OverlayMenu.Items`、`IsOpen`、`Modality(Modal/NonModal)`、`Placement(Right/Left/Bottom/Center)`、`IsBackButtonVisible`；方法 `Open`、`Close`、`NavigateBack`、`Invoke`；事件 `Opened`、`Closed`、`ItemInvoked`、`NavigationChanged`。

## State model

根菜单打开时路径为空；拥有 Children 的项目进入子菜单，叶项目触发事件。Modal 叶项目未被 Handled 时自动关闭；NonModal 保持打开。Esc 优先返回父级，根级再关闭。

## Template parts and visual tree

必需 `PART_Panel` 和 `PART_Items`；`PART_Scrim`、`PART_BackButton` 可选。缺失 Scrim 时仍可使用 NonModal；缺失 BackButton 时保留 Esc 返回。

## Behavior and failure modes

空 Items 仍可打开但显示空状态；禁用项目不触发事件；宿主改变 Items 后可再次 Open 重新读取根集合。控件不接管焦点之外的窗口内容。

## Open Decisions

Open Decisions：焦点循环的自动化细节、Bottom/Center placement 的窄窗回退和深层导航动画时长。

## Proposed implementation baseline
`Items`, `IsOpen`, `Modality=Modal`, `Placement=Right`, `Open/Close/NavigateBack`; states `Closed/Opening/Open/Submenu`, parts `PART_Scrim`, `PART_Panel`, `PART_Items`, `PART_BackButton`. Esc逐层返回，Modal焦点循环并恢复。
