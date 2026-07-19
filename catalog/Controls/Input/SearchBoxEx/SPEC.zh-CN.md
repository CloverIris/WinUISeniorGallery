# SearchBoxEx 规格

## 目标

提供隐私优先的搜索输入：文本、历史、可取消建议 provider、去抖、提交和键盘焦点由控件管理；搜索执行、数据访问和持久化由宿主管理。

## 非目标

不访问文件/账户历史，不联网，不替宿主执行查询，不在卸载后继续投递建议。

## 公共 API

`Text`、`PlaceholderText`、`Debounce=250ms`、`MinimumPrefixLength=1`、`IsHistoryEnabled=false`、`QueryCommand`、`SuggestionProvider`、`SearchState`、`IsSuggestionListOpen`、`Suggestions` 和 `SearchHistory`。`RefreshSuggestionsAsync` 取消上一次请求并以版本号拒绝迟到结果；`SubmitAsync` 发布 `QuerySubmitted` 并可调用 `QueryCommand`。`ClearHistory` 只清除内存历史。

## 状态和输入

状态为 `Idle/Loading/Results/Error`。达到最小前缀后按 Debounce 触发 provider；provider 为空时回退到可选的内存历史。编辑框 Down 将焦点移入建议列表，Enter 发布建议调用后提交，Escape 关闭列表，F6 聚焦搜索。

## 模板与生命周期

`PART_EditBox` 必需；`PART_SuggestionList`、`PART_ProgressRing` 可选。缺少可选部件不影响 API。控件卸载时停止去抖、取消 provider、递增请求版本并停止进度指示；provider 替换会取消旧请求并重新调度当前查询。

## 失败与无障碍

provider 异常进入 Error 并发布 `SearchError`；处理方可抑制默认 HelpText。结果列表使用 Polite live setting，建议拥有稳定文本和可见焦点。中文、英文、RTL、高对比度和 Reduced Motion 不改变查询语义。

## 当前边界

工作项仍为 `in-progress/lab/P1`；趋势搜索、语音输入和持久化历史需另行评审。
