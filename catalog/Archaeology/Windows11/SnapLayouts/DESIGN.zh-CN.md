# Snap Layouts 现代化设计

## 原型结构与 Design DNA

系统入口包含 MaximizeButton flyout、LayoutTemplates、Zone hit regions、当前窗口预览和后续窗口填充分组；它属于 Shell 非客户区。 保留：布局选择在操作点附近、以图形而非比例数字表达、键盘与指针同等、先预览后提交、常见多任务布局可快速发现。

## 明确废弃

现代 Demo 明确抛弃模拟最大化按钮 flyout、移动真实 OS 窗口、创建 Snap Group、覆盖系统热区和使用 Windows 商标图形；只提炼应用内 Dock 预览。

## 输入与焦点

研究页演示指针悬停、点击、方向键和 Enter，但不注册 Win+Z；Esc 取消预览并恢复拖拽源焦点。 自动更新不得抢焦点或更改 Automation 名称；关闭覆盖层恢复明确触发点。

## 响应式、RTL 与自动化

Demo 按宿主内容区生成 2–6 个区域，窄窗口减少方案；RTL 镜像区域顺序，DPI/显示器变化重算应用内坐标。 200% 文本缩放不裁切状态/退出路径，Automation 提供角色、状态、位置和下一动作。

## 主题与 High Contrast

只使用 ThemeResource；High Contrast 不依赖 Acrylic/Mica/阴影/透明度，Reduced Motion 去除滑动/缩放但保留状态。

## 版权与资产禁区

不得提交 Windows 11 截图、系统图标、壁纸、声音、字体、新闻内容、品牌文本或安装包提取资产；只用原创几何/假数据。

