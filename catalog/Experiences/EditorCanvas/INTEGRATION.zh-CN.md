# EditorCanvas Integration

## Dependencies

`WinUI3.Senior.Core` 的 CanvasDocumentController/CanvasViewportController；`WinUI3.Senior.Controls` 的 EditorCanvas。Gallery 只提供合成对象和人工调试入口。

## Global contracts and resources

不得重复定义全局 Contract 或资源键。当前无异步工作；宿主销毁前应将 `Document` 置空并解除事件。默认不声明额外权限，不收集遥测或用户内容。

## Platform APIs and capabilities

No extra capability by default.

## Lifecycle and threading

文档控制器可跨线程提交，但 WinUI Document 绑定和渲染必须回到 UI 线程；宿主销毁时解除 `Changed` 订阅，迟到变更只更新已存在的控制器而不访问窗口。

## 所有权、边界与生命周期
宿主拥有文档格式、保存、字体/图片许可。Core 的 Normalize 拒绝非法几何和重复 ID；EditorCanvas 不创建窗口、不保存路径、不持有文件句柄。
