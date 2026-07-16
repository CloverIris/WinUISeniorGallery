# Media Center Grid 现代化设计

## 原型视觉与保留项

GridViewport、虚拟化海报行列、FocusChrome、Page/SectionHeader 和 DetailsOverlay；选中与焦点可分离。 保留：稳定二维空间、10-foot 焦点、内容优先海报、焦点移动驱动详情、返回路径清晰。

## 抛弃项

抛弃固定列数、低分辨率位图、遥控器唯一输入、放大导致布局抖动和全量实现；使用响应列、许可图片、渲染变换和虚拟化。

## 输入与焦点

D-pad/摇杆按几何邻项，A/Enter 调用，B/Esc 返回；鼠标、触摸和键盘同等。Automation 读行列、标题、选中与展开详情。 自动动画不得抢焦点或更改 Automation 名称。

## 响应式与本地化

按最小卡片宽度重排 1–8 列，保留稳定 ID 焦点；文本缩放减少列数，不缩小字体；High Contrast 用系统焦点环。 中文、英文、长文本和 RTL 用真实资源测试。

## 主题、动效与无障碍

Demo 只用 ThemeResource；High Contrast 不依赖图片/透明度，Reduced Motion 去除视差/缩放/滑动但保留状态；目标至少 44×44 epx。

## 资产禁区

禁止产品截图、海报/封面、商标、声音、原字体和安装包提取资源；只用原创几何、许可兼容素材和文字结构图。

