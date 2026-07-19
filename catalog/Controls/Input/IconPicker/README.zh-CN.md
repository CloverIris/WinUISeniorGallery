# IconPicker

`IconPicker` 是应用显式注册图标目录的安全选择器。它提供搜索、分类、收藏、最近使用和可提交选择，不扫描 DLL/EXE、系统资源或字体文件，也不替宿主持久化用户偏好。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Input/IconPicker

实现目录：`src/WinUI3.Senior.Controls/Input/IconPicker`

## 实现准备
当前实现覆盖 `IconPicker`、`IconPickerSource`、`IconPickerItem`；图标源由应用注入，查询默认 200ms 去抖，未知或无 glyph 项不能提交，选择后维护有界内存 Recent，收藏过滤不访问外部存储。Glyph 原样保留方向语义，RTL 镜像信息由目录项显式声明。Gallery 接入与自动化验证后续补齐。
