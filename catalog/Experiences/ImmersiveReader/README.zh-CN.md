# ImmersiveReader

这是中文规范源。当前为 `in-progress`，已实现虚拟化阅读块、焦点锚定和宿主朗读请求。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Experiences/ImmersiveReader

实现路径：`src/WinUI3.Senior.Controls/Experiences/ImmersiveReader`。

## 场景准备度
支持段落/标题模型、行聚焦、字体缩放、键盘焦点移动和可取消的朗读边界请求。文本来源、朗读 Provider 和隐私策略仍由宿主负责。
