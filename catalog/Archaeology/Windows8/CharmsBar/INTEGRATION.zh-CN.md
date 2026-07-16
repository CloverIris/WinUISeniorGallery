# Charms Bar 研究展示集成

## 归属与宿主边界

现代 API、资源键、模板部件和窗口生命周期均由 `windowing.edge-command-panel` 及 Windowing Contracts 决定。本展项只能读取其公开文档，在 Gallery 内呈现静态说明或本地演示状态；不得引用系统级 edge API、注册快捷键、创建窗口或要求任何 capability。

## 生命周期、线程与诊断

演示数据为本地只读 JSON/Markdown；页面卸载时取消讲解动画和焦点恢复请求。UI 状态只在 UI 线程变化，不保存用户内容，不联网，不遥测。宿主不存在、面板未实现、backdrop 不支持或请求被拒绝时，记录为 Gallery 本地诊断并显示降级文案。

## 资源与隐私

使用 Gallery 通用主题资源和自制占位图；不得使用 Windows 原图标、截图或提取资产。任何未来外部链接均需在访问时声明离开 Gallery；本展项无权限、账户、媒体或个人数据边界。
