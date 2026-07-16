# MediaCenterGrid 规范

## 目标

定义面向电视、遥控器和客厅距离的虚拟化海报网格，二维焦点移动时放大当前项并呈现详情。

## 宿主与窗口边界

保留在当前页面视觉树，不创建窗口、不全屏、不播放。宿主提供 ItemsSource、模板、导航和详情；窗口、播放与后台加载归外部服务。

## 非目标

不实现媒体索引、推荐、图片缓存、播放队列或 DRM。

## 候选表面与闭锁条件

候选概念为 ItemsSource、ItemTemplate、SelectedItem、FocusedItem、Columns、FocusScale、DetailsTemplate、ItemInvoked；不是承诺 API。ready 前冻结选择/焦点、异步项目、滚动对齐和 Automation。

## 状态图

```text
Empty --> Loading --> Ready\nReady --> FocusMoving --> Ready\nReady --> DetailsVisible --> Ready\nLoading/Ready --> PartialError\nAny --> Unloaded
```

## 模板部件与视觉树候选

候选视觉树：ScrollViewer → ItemsRepeater → 虚拟化海报，另有 DetailsPresenter、FocusChrome、Loading/Empty/Error Presenter；放大只用渲染变换。

## 行为与失败模式

方向输入按几何邻项移动，不因异步图片跳焦；焦点滚入安全区再加载详情。刷新按稳定 ID 保焦，ID 缺失选最近可见邻项。

## Ready 晋级门禁

冻结 API/默认值/线程、模板 Contract、转换表、销毁/取消、资源、性能和 AutomationPeer，并同步双语 API/ID 后才可 ready。

