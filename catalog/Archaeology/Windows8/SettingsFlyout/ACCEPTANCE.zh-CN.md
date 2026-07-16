# Settings Flyout 研究展示验收

- Given 展项打开，When 选择设置层级，Then 显示 Root/Detail 结构和 `windowing.settings-panel` 链接，不写入设置。
- Given Detail 或 Dirty，When Esc/Back/Close，Then 按一层返回并恢复焦点，说明未保存模拟数据被丢弃。
- Given 窄窗、RTL、200% DPI、High Contrast 或 Reduced Motion，When 打开/关闭，Then 回退可读、方向正确、焦点可见且无动画依赖。

自动化验证路由、状态、焦点、accessible name、无系统 Settings/网络/持久化调用；覆盖中英/RTL 与三档 DPI，发布只含审查资产。
