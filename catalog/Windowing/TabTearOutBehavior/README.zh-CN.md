# TabTearOutBehavior

## 实现备注

当前控制器跟踪按下、拖动、请求和取消状态，使用二维距离阈值，报告拖动进度，支持取消及宿主主动请求撕出；窗口创建和内容迁移仍由宿主负责。

把标签页的数据/视图模型从一个窗口事务化转移到新窗口或另一 TabHost，并支持失败回滚。

## 状态与范围

- 状态：in-progress / lab / P2
- 依赖：contracts.windowing
- 不允许实现；候选 API/模板名仅用于评审。

## 宿主与窗口边界

绝不跨 Window 重父级化 XAML 元素；源/目标宿主提供序列化描述、ContentFactory 和窗口工厂。Behavior 只协调事务和焦点。

## Agent 所有权

仅 catalog/Windowing/TabTearOutBehavior；SPEC/DESIGN/INTEGRATION/ACCEPTANCE 分别锁定职责、视觉、平台生命周期和验收。
