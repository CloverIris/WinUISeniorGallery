# SemanticZoomView 规格

## 目标

在详细列表与分组概览之间切换，分组键和标题由宿主 selector 提供；控件只拥有分组投影、模式、焦点组和输入语义。

## 公共 API

`ItemsSource`、`Mode=Detail`、`IsZoomEnabled=true`、`FocusedGroupIndex`、`IsReducedMotion`、`GroupKeySelector`、`GroupTitleSelector`、`Groups`；方法 `RebuildGroups`、`ZoomOut`、`ZoomIn`、`InvokeGroup`。`ZoomChanged` 在模式提交后发布，`GroupInvoked` 在概览项被调用时发布。

## 分组与状态

空/null 分组键归一化为 `#`；组按当前区域不区分大小写排序，空列表保持 `FocusedGroupIndex=-1`。概览模式允许键盘/鼠标滚轮选择组，Enter 或加号回到详细模式；Escape/减号进入概览。RTL 镜像左右组导航。

## 模板与降级

`PART_ZoomedInView`、`PART_ZoomedOutView` 必需，缺失任一部件抛明确模板异常；`PART_Viewport` 可选。Reduced Motion 时状态切换不播放动画。控件不创建第二份数据源，只把 `_groups` 绑定给概览。

## 当前边界

工作项为 `in-progress/lab/P1`；捏合手势、跨组定位和持久化缩放状态另行评审。
