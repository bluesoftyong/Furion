# `Configuration` 模块

🟡 **[文档手册](https://gitee.com/dotnetchina/Furion/tree/experimental/handbook/Configuration) | [使用示例](https://gitee.com/dotnetchina/Furion/tree/experimental/samples/Furion.ConfigurationSamples) | [模块源码](https://gitee.com/dotnetchina/Furion/tree/experimental/framework/Furion/Configuration) | [单元测试](https://gitee.com/dotnetchina/Furion/tree/experimental/tests/Furion.UnitTests/ConfigurationTests) | [集成测试](https://gitee.com/dotnetchina/Furion/tree/experimental/tests/Furion.IntegrationTests/ConfigurationTests)**

## 关于 `配置`

在大多应用程序中，存在一些独立于程序外且可移植的键值对数据，这类数据统称配置。

`配置` 有以下特点：

- `独立于程序的只读变量`

首先，配置是独立于程序的，同一份程序在不同配置下会有不同的行为。其次，配置对于程序是只读的，程序通过配置提供程序读取配置来改变自身行为，但程序不应该取改变配置。

- `存在于应用整个生命周期`

配置贯穿应用的整个生命周期，应用在启动时通过读取配置进行初始化，也可以在运行时根据配置调整行为。

- `配置可以有多种提供方式`

配置也有很多提供方式，常见的有 `环境变量`，`设置文件`，`命令行参数`，`内存配置`、`数据库存储配置`、`目录文件` 等。

- `配置可以实现集中管控`

简单的来说就是将配置信息集中管控，也就是我们常说的 `配置中心`，该方式可以在多个应用之间实现 `高可用`，`实时性`，`治理`、`多环境多集群` 管理等。

## 配置提供程序

在 `Furion` 框架中，默认支持以下配置提供程序：

- `文件配置提供程序`：支持 `.json`、`.xml`，`.ini` 配置文件。
- `环境变量提供程序`：可从系统环境变量、用户环境变量读取配置。
- `命令行参数提供程序`：支持命令行方式启动应用并且传入 `args` 参数。
- `内存 .NET 对象提供程序`：支持将集合数据存在到内存中供应用读取。
- `目录文件 Key-per-file 提供程序`：使用目录的文件作为配置键值对，该键为文件名，该值为文件内容。

除此之外，`Furion` 框架也提供强大的自定义配置提供程序行为，支持从数据库、`Redis` 等任何存储介质提供配置信息。

## `IConfiguration` 接口

在 `Furion` 框架中，提供 `IConfiguration` 接口读取配置信息，可在启用初始化时、运行时等地方获取其实例。

### `IConfiguration` 实例化

- 在 `WebApplicationBuilder` 中获取

```cs
var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
```

- 在 `HostBuilder` 中获取

```cs
Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((context, configurationBuilder) =>
    {
        IConfiguration configuration = context.Configuration;
    });
```

- 在 `构造函数注入` 获取

```cs
private readonly IConfiguration _configuration;
public IOCClass(IConfiguration configuration)
{
    _configuration = configuration;
}
```

- 在 `Startup.cs` 中获取

```cs
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // ...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // ...
    }
}
```

- 在 `Razor Pages` 中获取

```cs
@page
@model Test5Model
@using Microsoft.Extensions.Configuration
@inject IConfiguration Configuration
```

- 在 `Mvc 视图` 中获取

```cs
@using Microsoft.Extensions.Configuration
@inject IConfiguration Configuration
```

在 `选项配置` 中获取

```cs
services.AddOptions<MyOptions>()
    .Configure<IConfiguration>((option, configuration) => {
        // ...
    })
    .PostConfigure<IConfiguration>((option, configuration) => {
        // ...
    });
    .Validate<IConfiguration>((option, configuration) => {
        // ...
    });
```

### `IConfiguration` 常见方法

- `GetSection(key)`：获取子节点 `IConfigurationSection` 实例，该实例同样继承 `IConfiguration` 接口。
- `Get(key)`：获取节点对象值，返回 `object` 类型。
- `Get<T>(key)`：获取节点对象值，返回 `T` 类型。
- `GetValue(type, key)`：获取单个值，返回 `object` 类型。
- `GetValue(type, key, defaultValue)`：获取单个值，返回 `object` 类型，如果值不存在返回默认值。
- `GetValue<T>(key)`：获取单个值，返回 `T` 类型。
- `GetValue<T>(key, defaultValue)`：获取单个值，返回 `T` 类型，如果值不存在返回默认值。
- `Exists(key)`：判断节点是否存在，返回 `bool` 类型

除上述方法外，`IConfiguration` 接口也提供了索引获取方式，如：`configuration[key]`，该节点总是返回 `string` 类型。

接下来，我们使用 `values.json` 作为配置文件演示 `IConfiguration` 一些常见使用。

- `values.json` 内容

```json
{
  "String": "String",
  "Boolean": true,
  "Boolean2": false,
  "Int": 2,
  "Long": 33333333333333333,
  "Float": -20.2,
  "Decimal": 40.32,
  "Enum": "Male",
  "Enum2": 0,
  "Array": [1, 2, 3, 4],
  "Array2": {
    "0": "one",
    "1": "two",
    "2": "three",
    "3": "four"
  },
  "Dictionary": {
    "key1": "value1",
    "key2": "value2",
    "somekey": "someValue"
  },
  "Object": {
    "Name": "Furion",
    "Version": "Next"
  }
}
```

- 读取配置

```cs
// string 类型
configuration.Get<string>("String");    // => String 

// bool 类型
configuration.Get<bool>("Boolean"); // => true
configuration.Get<bool>("Boolean2");    // => false

// int 类型
configuration.Get<int>("Int");  // => 2

// long 类型
configuration.Get<long>("Long");    // => 33333333333333333

// float 类型
configuration.Get<float>("Float");  // => -20.2

// decimal 类型
configuration.Get<decimal>("Decimal");  // => 40.32

// 枚举 类型
configuration.Get<Gender>("Enum");  // => Gender.Male
configuration.Get<Gender>("Enum2"); // => Gender.Male

// 数组类型
configuration.Get<int[]>("Array");  // => [1, 2, 3, 4]
configuration.Get<string[]>("Array2");  // => ["one", "two", "three", "four"]

// 字典类型
configuration.Get<Dictionary<string, string>>("Dictionary");    // { [key1] = "value1", [key2] = "value2", [somekey] = "someValue" }

// 对象类型
configuration.Get<ObjectModel>("Object");   // => { Name: "Furion", Version: "Next" }

// 索引读取方式
configuration["Object:Version"];    // => Next

// 判断键是否存在
configuration.Exists("Object:Author");  // => false

// 获取单个值，如果值不存在返回默认值
configuration.GetValue<string>("Object:Author", "百小僧"); // => 百小僧
```
