# Property Grid 集成

## Given / When / Then 补充
- Given 历史/现代并排，When 切换状态/输入，Then 结构、焦点和废弃行为逐项可追溯。
- Given 窄窗、RTL、高对比度或 Reduced Motion，Then 核心任务可完成且 Narrator 获得名称/角色/状态/位置。
- Given 来源或许可未通过，Then 仅显示自制占位，不得新增 API 或进入 review/stable。

## 依赖方向

依赖 controls.property-grid，只允许 Archaeology/Gallery 指向稳定模块。本展项只拥有历史研究、现代化说明和 Gallery 演示规格；稳定实现由 controls.property-grid（PropertyGrid）拥有。

## 数据与平台

Gallery 使用确定性本地假数据。文件系统、Shell、系统字体、搜索索引、反射或异步数据源只能通过宿主 Provider 接入；展项不自行扫描设备或修改数据。

## 全局契约

使用 Theme、Motion、Input、Accessibility、Navigation、Localization、Resources 和 ExternalApis Contract。异步请求带版本/取消，关闭页面后忽略迟到结果。

## 降级与版权

平台 API 不可用时使用安全假数据并解释限制。不得提取系统 DLL 图标、复制 Windows 截图/声音/字体或提交第三方内容；发布前 license_review 保持 pending。
