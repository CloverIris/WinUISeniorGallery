# MixviewExperience Integration

## Contracts and resources

依赖 `foundation.motion-system` 的 Reduced Motion 语义；不新增全局资源键。节点和内容由宿主传入，控件不访问文件、网络或媒体服务。

## Host boundary

宿主负责导航、推荐、持久化和内容生命周期。控件只通过 `NodeSelected`、`Closed` 通知宿主，不创建窗口或后台单例。

## Lifecycle and threading

`SetNodes`、`Open`、`Close`、`SelectNode` 应在 UI 线程调用；卸载后模板引用仅用于当前实例，宿主应停止外部更新。

## Failure and privacy

无效节点由 `ArgumentException` 报告；未知选择返回 false。无权限、无遥测、无上传、无持久化。
