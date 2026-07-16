# Settings Flyout 设计考古与现代化

保留：不离开任务的临时设置层、清晰标题/返回和分级设置。拒绝：设置只藏在系统边缘、面板无关闭出口、窄宽度塞入完整表单。现代 `SettingsPanel` 应由应用明确发现入口，宽窗在逻辑结束侧显示，窄窗回退到底部/居中页。

Light/Dark/High Contrast 保证标题、分隔和焦点可辨；RTL 使用逻辑 end；100/150/200% DPI 和文本缩放不截断。Reduced Motion 立即呈现；Tab 顺序为入口、标题、返回、设置、关闭，Esc 关闭一层；Narrator 宣告层级、未保存状态及失败原因。
