# People Card 集成

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 依赖方向

依赖 experiences.people-card；仅 Archaeology/Gallery 指向稳定模块，稳定模块不得引用展项。本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 experiences.people-card（PeopleCard）拥有。

## 数据与服务

Gallery 使用匿名确定性假数据。Microsoft Graph、Azure、文件系统、朗读或协作服务只能由宿主通过独立 Provider 注入，并负责身份验证、同意、取消和速率限制。

## 全局契约

使用 Theme、Motion、Input、Accessibility、Navigation、Localization、Resources 与 ExternalApis Contract。所有异步结果必须绑定请求版本，关闭页面后忽略迟到响应。

## 降级与版权

离线或权限不足时保留本地结构与说明。禁止提取 Office/Microsoft 365 图标、截图、声音、模板和字体；演示资产必须自有或许可兼容，发布前 license_review 保持 pending。
