# Zune Big Typography 集成

## 现代 owner 与依赖

唯一现代 owner 是 controls.big-title（BigTitle）。展项只通过公开、已登记的 owner 表面配置 Demo；稳定模块不得反向引用 Archaeology。

## Gallery 数据与平台

原型结构、DNA 和差异来自本地 Markdown；Demo 使用匿名确定性假数据。展项不访问历史服务、账号、媒体库或系统 Shell；异步素材请求必须可取消且有许可元数据。

## 输入、窗口与生命周期

Demo 属于 Gallery 当前 XamlRoot，不创建/迁移 Window。标题默认非交互，不进入 Tab；若绑定返回/导航则必须有按钮语义和标准焦点，不让整块装饰文字成为隐形命中区。 页面卸载时取消动画、延迟和 Provider，忽略迟到结果并恢复导航焦点。

## 降级与资源

owner 缺失或平台效果不可用时保留文本结构图、输入矩阵和 Sources。不得在 Archaeology 定义 BigTitle 的样式键/API 副本；历史 preset 只存在于 Gallery 配置。

## 版权与隐私

license_review 保持 pending；所有演示资产需记录作者、许可和来源。不得上传用户媒体/历史，Gallery 不收集展项交互遥测。

