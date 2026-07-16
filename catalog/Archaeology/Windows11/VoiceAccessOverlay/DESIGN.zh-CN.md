# Voice Access Overlay 现代化设计

## 原型结构与 Design DNA

ListeningStatusBar、RecognizedCommandFeedback、NumberLabels、GridOverlay、Correction/Help 面板和麦克风状态构成系统级辅助层。 保留：语音状态始终可见、命令即时反馈、目标编号把任意 UI 转成可说地址、失败可纠正、无需触摸精确定位。

## 明确废弃

Gallery 明确抛弃系统级覆盖、全局输入注入、麦克风采集、语音识别、模仿 Voice Access 品牌/安全提示和遮盖其他应用；只展示本地静态交互模型。

## 输入与焦点

模拟可由按钮/键盘触发 Listening、Numbers 和 Grid；不注册全局热键。焦点不移动到数字标签，Esc 关闭最内层覆盖并恢复触发按钮。 自动更新不得抢焦点或更改 Automation 名称；关闭覆盖层恢复明确触发点。

## 响应式、RTL 与自动化

数字标签避让目标和边缘，网格按本地 Demo 面积重算；RTL 只影响文本，不改变数字读法。High Contrast 使用实色边界且标签不遮挡焦点。 200% 文本缩放不裁切状态/退出路径，Automation 提供角色、状态、位置和下一动作。

## 主题与 High Contrast

只使用 ThemeResource；High Contrast 不依赖 Acrylic/Mica/阴影/透明度，Reduced Motion 去除滑动/缩放但保留状态。

## 版权与资产禁区

不得提交 Windows 11 截图、系统图标、壁纸、声音、字体、新闻内容、品牌文本或安装包提取资产；只用原创几何/假数据。

