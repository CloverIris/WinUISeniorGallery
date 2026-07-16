# Controls Minimal

未打包的 WinUI 3 开发示例，用于手动验证 `CarouselView`，不属于正式 Gallery，也不包含网络、媒体或业务后端。

## 前置条件

- x64 Windows 10 1809 或更新版本。
- 已安装与项目匹配的 Windows App SDK 2.2 runtime。项目以 `WindowsPackageType=None` 运行，并通过 Windows App SDK bootstrap 自动初始化；示例不会安装系统依赖。

## 覆盖内容

示例固定生成 5、20 或 1000 个本地确定性数据项，并暴露 Bounded/Loop、Slide/Fade/CoverFlow、自动播放、相邻预览、RTL、系统窗口活跃状态和运行时实现元素计数。Reduced Motion 预览会切换为 Fade；正式 Reduced Motion 降级由 Windows 系统设置驱动。示例直接引用正式 `CarouselView`，不使用反射或业务后端。

运行：

```powershell
dotnet run --project .\samples\ControlsMinimal\ControlsMinimal.csproj -r win-x64
```
