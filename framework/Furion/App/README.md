# `App` 模块

🟡 **[使用文档](https://gitee.com/dotnetchina/Furion/tree/experimental/framework/Furion/App) | [模块源码](https://gitee.com/dotnetchina/Furion/tree/experimental/framework/Furion/App) | [单元测试](https://gitee.com/dotnetchina/Furion/tree/experimental/tests/Furion.UnitTests/AppTests)**

`App` 模块是 `Furion` 框架默认添加的模块，该模块提供了 `Furion` 框架全局配置及主机服务对象操作。

`App` 模块包含 `IApp` 服务注册及 `AppSettingsOptions` 选项注册。**默认情况下无需注册该服务。** 如需手动注册，可添加以下注册：

```cs
services.AddApp(configuration);
```

## `IApp` 服务接口

`IApp` 是 `App` 模块对外提供的服务接口，注册为 `单例` 服务。通过该接口可以获取主机常用服务对象，包括：

- 属性
  - `ServiceProvider`：获取根服务提供器，通过该属性可以解析 `单例` 服务、`瞬时` 服务以及创建新的服务范围解析 `范围` 服务。
  - `Configuration`：获取应用配置信息，包括文件配置、内存配置、环境配置、`Key-per-file` 配置以及自定义配置提供程序。
  - `Environment`：获取当前主机环境，如开发环境、生产环境等。也可以获取当前运行程序的内容根目录。
  - `Host`：获取主机对象，通过 `Services` 属性可以解析服务
- 方法
  - `GetService(Type)`：解析服务，支持已注册和未注册服务，如果服务已注册，返回 `object` 实例，否则返回 `default`。
  - `GetRequiredService(Type)`：解析服务，只支持已注册服务，如果服务已注册，返回 `object` 实例，否则抛 `InvalidOperationException` 异常。
  - `GetService<T>()`：解析服务，支持已注册和未注册服务，如果服务已注册，返回 `T` 实例，否则返回 `default`。
  - `GetRequiredService<T>()`：解析服务，只支持已注册服务，如果服务已注册，返回 `T` 实例，否则抛 `InvalidOperationException` 异常。

### `IApp` 使用例子

```cs
using Microsoft.AspNetCore.Mvc;

namespace Furion.Samples.AppSamples;

/// <summary>
/// App 模块 IApp 服务使用示例
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class IAppSamplesController : ControllerBase
{
    private readonly IApp _app;
    public IAppSamplesController(IApp app)
    {
        _app = app;
    }

    [HttpPost]
    public void Tests()
    {
        // 解析服务
        var app = _app.ServiceProvider.GetRequiredService<IApp>();
        Console.WriteLine(_app == app);

        // 读取配置
        var environmentVariablesPrefix = _app.Configuration["AppSettings:EnvironmentVariablesPrefix"];
        Console.WriteLine(environmentVariablesPrefix);

        // 判断环境
        var isDevelopment = _app.Environment.IsDevelopment();
        Console.WriteLine(isDevelopment);

        // 通过主机对象服务解析服务
        var app1 = _app.Host.Services.GetRequiredService<IApp>();
        Console.WriteLine(app1);

        // 直接解析服务
        var notRegisterService = _app.GetService<INotRegisterService>();
        var registerService = _app.GetRequiredService(typeof(IRegisterService));
        Console.WriteLine(notRegisterService);
        Console.WriteLine(registerService);
    }
}
```

## `AppSettingsOptions` 配置选项

`AppSettingsOptions` 配置选项是 `App` 模块对外提供的配置模块，默认情况下，只能配置到 `appsettings.json` 或 `appsettings.{environment}.json` 中。配置根节点名称为：`AppSettings`。

`AppSettingsOptions` 提供 `Furion` 框架初始化配置属性：

- `EnvironmentVariablesPrefix`：配置环境配置提供器变量（节点）前缀，`string` 类型，默认为 `FURION_`。另外，节点和下级节点采用 `__` 连接，如：`FURION_AppSettings__节点__下级节点__下下下级节点`。
- `CustomizeConfigurationFiles`：配置框架启动自动添加的文件配置，`string[]?` 类型，默认为 `default`。

### `CustomizeConfigurationFiles` 配置说明

`Furion` 框架为了方便开发者快速添加文件配置文件，提供了配置项语法糖。如：

- 如果配置项以 `@` 或 `~` 开头，则默认拼接 `启动项目根目录`，如：

```json
{
    "AppSettings: {
        "CustomizeConfigurationFiles": [ "@furion.json", "~furion.json" ]
    }
}
```

那么 `furion.json` 文件最终查找路径为：`启动项目根目录/furion.json`，如：`D:/Furion.Samples/furion.json`。

- 如果配置项以 `&` 或 `.` 开头，则默认拼接 `程序执行目录`，如：

```json
{
    "AppSettings: {
        "CustomizeConfigurationFiles": [ "&furion.json", ".furion.json" ]
    }
}
```

那么 `furion.json` 文件最终查找路径为：`启动项目根目录/furion.json`，如：`D:/Furion.Samples/bin/furion.json`。

- 如果配置项以 `/` 或 `!` 开头，则认为这是一个绝对路径，如：

```json
{
    "AppSettings: {
        "CustomizeConfigurationFiles": [ "!D:/furion.json", "/D:/furion.json" ]
    }
}
```

那么 `furion.json` 文件最终查找路径为：`启动项目根目录/furion.json`，如：`D:/furion.json`。

- 除此之外，则默认拼接 `启动项目根目录`，与 `@ 或 /` 配置方式一致，如：

```json
{
    "AppSettings: {
        "CustomizeConfigurationFiles": [ "furion.json", "furion.json" ]
    }
}
```

那么 `furion.json` 文件最终查找路径为：`启动项目根目录/furion.json`，如：`D:/Furion.Samples/furion.json`。

除了上述配置项前缀提供了语法支持外，`Furion` 框架还提供类型 `命令操作符` 的可选参数配置文件添加方式。如：

```cs
{
    "AppSettings: {
        "CustomizeConfigurationFiles": [ "furion.json includeEnvironment=true optional=false reloadOnChange=false" ]
    }
}
```

配置语法为：`文件名 [includeEnvironment|optional|reloadOnChange]=[true|false]`。

配置项可选参数说明：

- `includeEnvironment`：是否自动将该配置文件应用与主机环境，`bool` 类型，默认 `true`。如配置了 `furion.json includeEnvironment=true`，那么自动添加 `furion.{environment}.json` 配置，根据不同环境自动切换。
- `optional`：是否不检查配置文件存在物理硬盘，`bool` 类型，默认 `true`，也就是即使文件不存在也可以添加，同时支持文件由无到有自动刷新 `IConfiguration` 配置对象。
- `reloadOnChange`：是否文件发生改变自动刷新 `IConfiguration` 配置对象，`bool` 类型，默认 `true`。

### `AppSettingsOptions` 使用例子

```cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Furion.Samples.AppSamples;

/// <summary>
/// App 模块 AppSettingsOptions 使用示例
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class AppSettingsOptionsSamplesController : ControllerBase
{
    private readonly IOptions<AppSettingsOptions> _options;
    private readonly IOptionsSnapshot<AppSettingsOptions> _optionsSnapshot;
    private readonly IOptionsMonitor<AppSettingsOptions> _optionsMonitor;
    public AppSettingsOptionsSamplesController(IOptions<AppSettingsOptions> options
        , IOptionsSnapshot<AppSettingsOptions> optionsSnapshot
        , IOptionsMonitor<AppSettingsOptions> optionsMonitor)
    {
        _options = options;
        _optionsSnapshot = optionsSnapshot;
        _optionsMonitor = optionsMonitor;
    }

    [HttpPost]
    public void Tests()
    {
        // 配置更改不会刷新
        var appSettings1 = _options.Value;
        Console.WriteLine(appSettings1.EnvironmentVariablesPrefix);

        // 配置更改后下次请求应用
        var appSettings2 = _optionsSnapshot.Value;
        Console.WriteLine(appSettings2.EnvironmentVariablesPrefix);

        // 配置更改后，每次调用都能获取最新配置
        var appSettings3 = _optionsMonitor.CurrentValue;
        Console.WriteLine(appSettings3.EnvironmentVariablesPrefix);
    }
}
```
