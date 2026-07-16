# GuideMenuExperience Integration

## Dependencies

windowing.edge-command-panel

## Global contracts and resources

不得重复定义全局 Contract 或资源键。异步操作必须可取消；宿主销毁后不得继续回调。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

Cancellation and host destruction must be handled.

## 所有权、边界与生命周期
组合 EdgeCommandPanel；宿主提供命令/导航，体验不拦截系统Guide键。卸载取消命令并恢复底层焦点。
