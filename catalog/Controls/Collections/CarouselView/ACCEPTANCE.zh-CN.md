# CarouselView 验收

## 本轮验证记录

- 已通过：Controls Release x64、Gallery Debug x64、Gallery Release x64、ControlsMinimal Debug x64、Catalog Validator（124 个工作单元）和 Architecture Tests（4/4）。
- 已构建：WinUI Unit Test App 测试宿主可在 Debug x64 下以 0 error 构建，并保留 `UITestMethod`、`WinUITestTarget`、Windows App SDK bootstrap 与 x64 RID 配置。
- 未完成：当前 `dotnet test` 的 VSTest 启动路径不会启动 WinUI Unit Test App，因此无法为 `UITestMethod` 提供 XAML UI 线程；该 CLI 限制不是 Carousel 断言失败。必须经 Visual Studio Test Explorer 的 WinUI Unit Test App 路径完成 UI 用例，并补充 1000 项 P95 与主题/DPI 人工验收，才可进入 `review`。

## 功能场景

- Given 1000 个轻量逻辑项和默认缓冲，When 首次呈现并连续导航 100 次，Then `RealizedElementCount` 只反映选中项及前后缓冲，不随总项数线性增长。
- Given Loop 且选中末项，When `MoveNext`，Then 恰好一次选择变化到索引 0，且源集合未复制；Given Bounded 末项，When `MoveNext` 或自动播放到期，Then 保持末项并停止重复计时。
- Given `ItemsSource`、`ItemTemplate` 或 `ItemTemplateSelector` 在运行时替换，When 下一次布局，Then Repeater 同步新配置且不会预创建全部容器。
- Given 选中对象在集合变更后仍存在，When 插入、删除或移动其他项，Then 仍选中同一对象；Given 选中对象被删除，When 存在后继或新末项，Then 按规格选择它；Given 集合为空，Then 选择为 `-1`。
- Given 自动播放启用，When 控件不可见、`IsHostWindowActive=false`、焦点在内、指针悬停或用户拖拽，Then 以固定优先级暂停；When 条件恢复，Then 等待完整间隔后才移动。
- Given RTL，When 左右键、滑动和 Previous/Next 按钮触发，Then 视觉方向镜像但索引规则一致。

## 自动化与输入

- Given 选中项，When Enter、Space 或手柄 A，Then `ItemInvoked` 的 `Item`、`Index` 与 `InputDeviceKind` 正确；无法可靠识别精确触控板时为 `Unknown`。
- Given 水平滚轮或 Shift+滚轮，When 滚动，Then 导航并标记事件已处理；Given 普通垂直滚轮，Then 事件继续冒泡。
- Given 必需 Repeater 缺失，When 应用模板，Then 抛出明确异常；Given 任一可选部件缺失，Then 控件可操作但对应功能降级。
- Given 用户导航，When SelectionChanged，Then AutomationPeer 提供 Selection、Scroll、ItemContainer 且 Live Region 公告一次；Given 自动播放，Then 不公告。

## 质量矩阵

- Light、Dark、High Contrast；中文、英文、RTL；320/800/1920 有效像素；100%/150%/200% DPI；文本缩放和 Reduced Motion 都可操作。
- 鼠标、滚轮、键盘、触摸、精确触控板和 Xbox 手柄均有对应行为或明确 `Unknown` 设备报告。
- Release x64、1000 个轻量项连续导航 100 次，P95 UI 帧目标不超过 25ms；测试记录运行机器、DPI、窗口宽度、模板和测量值。
- 卸载后不存在增长的计时器、事件订阅、Composition 批次或已实现元素引用。

## 自动化测试

- 纯逻辑：索引映射、空/单项、边界/循环、参数验证、集合选择修复、暂停优先级和完整间隔重置。
- XAML UI 线程：模板缺件、可选部件降级、1000 项实现数量、键盘 RTL、项目调用、焦点/悬停/窗口活跃状态、Automation 和无自动播放公告。
- 交付前运行 Controls Release x64、Gallery Debug/Release x64、Catalog Validator、Architecture Tests 与 Controls 测试宿主；失败时保持 `in-progress` 并记录失败证据。

## 当前证据

状态仍为 `in-progress`。本轮只记录实现目标；实际构建、UI 测试、性能测量和 Gallery 人工验收结果必须在根 agent 完成后写入，未通过前不得标记 `review`。
