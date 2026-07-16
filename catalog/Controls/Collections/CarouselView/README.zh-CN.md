# CarouselView

`CarouselView` 是面向 Hero Banner、专辑封面、引导页和相邻内容预览的虚拟化单选轮播控件。它提供有限或循环导航、自动播放、鼠标/键盘/触摸/手柄输入，以及可替换的过渡效果；它不获取数据、不缓存图像、不导航页面，也不播放媒体。

## 状态

- 工作项：`controls.carousel-view`
- 成熟度：`lab`；优先级：`P0`；实现状态：`in-progress`
- 包与命名空间：`WinUI3.Senior.Controls` / `WinUI3.Senior.Controls`
- Gallery 路由：`/controls/carousel-view`

## 范围

实现只保留当前选中项和 `RealizationBuffer` 指定的相邻源索引。Loop 通过模运算映射索引，绝不构造重复集合。`CarouselView` 继承可在 C# 中实例化的 `ListView`；它保留 `Selector` 的单选语义，因此不是直接继承不可激活 `Selector` 的类型。

## 依赖与所有权

它依赖 Theme、Motion、Input 和 Accessibility Contracts。实现 agent 只能修改 `feature.json` 的 `owned_paths`。公共 Contract 不足时必须将工作项标记为 `blocked`，不得私建不兼容 API。

## 文档

- [规格](SPEC.zh-CN.md)
- [设计](DESIGN.zh-CN.md)
- [集成](INTEGRATION.zh-CN.md)
- [验收](ACCEPTANCE.zh-CN.md)

中文是规范源；英文文件是同步翻译。

## 实现门槛

此项处于 `in-progress`。只有控件、模板、自动化、Gallery 实验页、未打包 Sample、测试和验收证据全部完成后，维护者才可推进至 `review`。
