# SearchBoxEx

这是中文规范源。当前实现处于 in-progress；API 仍允许在后续评审中扩展，但取消、版本过滤和隐私边界已经锁定。

## Status

in-progress / lab / P2

## Documents

- SPEC.zh-CN.md
- DESIGN.zh-CN.md
- INTEGRATION.zh-CN.md
- ACCEPTANCE.zh-CN.md

## Agent ownership

catalog/Controls/Input/SearchBoxEx

## 实现准备
分类建议、历史与趋势的搜索框。默认不持久化历史；提供商由宿主注入，查询使用可取消的防抖请求，旧请求不得覆盖新请求。
