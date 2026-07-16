# Cover Flow 现代化设计

归属基线：Cover Flow 在本项目中属于媒体软件设计史；Apple 于 2006-09-12 在 iTunes 7 公告中将其列为新功能，它不是 Zune 原创设计。

## 原型视觉与保留项

CenterCover、左右 PerspectiveNeighbors、Reflection/Shadow、水平索引与当前媒体信息构成 3D 风格浏览器；中心项是唯一主要调用目标。 保留：中心项层级极强、邻项表达序列、滚动与透视连续、封面浏览具有物理感和快速扫视能力。

## 抛弃项

抛弃倒影作为必需、过度 3D/晕动、全量封面纹理、只支持横向 LTR 和把它归入 Zune；使用虚拟化、可关效果、Reduced Motion 平面模式和 RTL。

## 输入与焦点

滚轮/拖动/触摸保留，左右键/手柄切中心，Enter/A 调用；Narrator 采用线性列表/选中项语义，不朗读透视。 自动动画不得抢焦点或更改 Automation 名称。

## 响应式与本地化

窄屏减少邻项和透视，宽屏限制纹理尺寸；Reduced Motion/High Contrast 使用平面轮播和标准焦点，RTL 镜像序列。 中文、英文、长文本和 RTL 用真实资源测试。

## 主题、动效与无障碍

Demo 只用 ThemeResource；High Contrast 不依赖图片/透明度，Reduced Motion 去除视差/缩放/滑动但保留状态；目标至少 44×44 epx。

## 资产禁区

禁止产品截图、海报/封面、商标、声音、原字体和安装包提取资源；只用原创几何、许可兼容素材和文字结构图。
