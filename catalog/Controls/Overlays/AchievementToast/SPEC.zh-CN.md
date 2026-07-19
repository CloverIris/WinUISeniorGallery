# AchievementToast Specification

## Goal

目标：定义一个窗口内 FIFO 的庆祝提示队列。`AchievementToast` 只负责呈现和生命周期，宿主负责请求内容和动作；它不创建窗口、不持久化或上传数据。

## Non-goals

不实现跨窗口路由、系统通知、音效、联网成就服务或持久化历史。

## Public API

`AchievementToastRequest.Normalize` 约束 Id/Title、Progress[0,1]、Duration 500ms–10min；`ShowAsync` 返回 `AchievementToastResult`，公开 `Dismiss`、`InvokeAction`、`Dispose`。状态为 `Queued/Showing/Visible/Closing/HostDestroyed`。

## State model

同一实例最多显示一条，后续请求 FIFO 排队，默认队列上限 32；重复当前 Id 不插入。计时结束进入 Closing 并解析当前任务，随后展示下一条。

## Template parts and visual tree

必需 `PART_Root`；可选 `PART_Icon`、`PART_Title`、`PART_Description`、`PART_Progress`。缺失可选部件只关闭对应视觉；宿主销毁时队列和当前任务以 `HostDestroyed` 结束。

## Behavior and failure modes

无可恢复异常时不阻塞后续队列；取消 token 只取消等待方，不撤销已显示视觉。Reduced Motion 由模板/主题处理，不改变队列顺序。

## Open Decisions

Open Decisions：是否需要动作按钮数据模型、跨窗口聚合服务和完整成就持久化；当前实现保持窗口本地。

## Proposed implementation baseline
`Title`, `Description`, `Icon`, `Progress`, `Rarity`, `Duration=6s`; service `ShowAsync(target,request)`. states `Queued/Showing/Visible/Closing`, parts icon/text/progress. 同窗FIFO，最多一条，窗口销毁返回 HostDestroyed。
