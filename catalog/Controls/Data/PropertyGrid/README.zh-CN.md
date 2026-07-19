# PropertyGrid

这是中文规范源。当前为 in-progress 实现工作单元；实现仍需后续模板、视觉验收与测试收尾。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Data/PropertyGrid

## 实现准备
当前实现包含反射元数据、分类/名称/声明顺序、文本筛选、编辑事务、DataAnnotations/IDataErrorInfo 校验、可选的 `IPropertyGridEditorProvider`，以及局部 Undo/Redo 历史栈。模板、可视化编辑器和自动化测试仍待收尾。
