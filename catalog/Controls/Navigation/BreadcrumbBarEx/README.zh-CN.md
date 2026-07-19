# BreadcrumbBarEx

这是中文规范源。当前实现处于 in-progress；模板和宿主解析器仍可在评审阶段细化。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Navigation/BreadcrumbBarEx

## 实现准备
可编辑、键盘友好且支持宿主自定义解析器的路径面包屑。控件不创建导航窗口、不解析文件系统权限；提交失败保持原路径并发出 NavigationFailed。
