# Media Center Overlay Menu 集成

## 现代 owner 与依赖

唯一 owner 是 controls.overlay-menu（OverlayMenu）；展项只配置已登记公开表面，稳定模块不得引用 Archaeology。

## Gallery 数据与平台

原型/DNA/差异来自本地 Markdown，Demo 用匿名确定性假数据。不访问历史服务、账号、媒体库或系统 Shell；异步素材可取消且带许可元数据。

## 输入、窗口与生命周期

Demo 属于当前 Gallery XamlRoot，不创建/迁移 Window。Menu/Enter/A 打开，D-pad 导航，B/Esc 逐层返回；鼠标/触摸/键盘等价。菜单、焦点或子弹层存在时不自动隐藏。 卸载取消动画/延迟/Provider，忽略迟到结果并恢复导航焦点。

## 降级与资源

owner 或效果不可用时保留结构图、输入矩阵和 Sources。不得复制 OverlayMenu API/样式键；历史 preset 仅是 Gallery 配置。

## 版权与隐私

license_review 保持 pending；演示资产记录作者/许可/来源。禁止上传用户媒体/历史，不收集展项遥测。

