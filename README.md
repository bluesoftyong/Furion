# Furion 探索版

> 在过去一年，实现 `Furion` 从无到有，编写文档已逾百万字，过程心酸开源人自知。
>
> 这一路日夜兼程，嘲讽辱骂也常伴眼耳，即使辛苦无奈、想过放弃，但为了那微不足道的存在感依然努力着。
>
> 当然，也收获了不少...越来越多拥趸者，越发精湛技术能力，更高层次思维模式，还有许多跨界跨行朋友。
>
> 在 《开源指北》中，我曾说道：“开源如同人的脸，好坏一面便知，缺点可能会受到嘲讽批评，优点也会收获赞扬尊重。别担心，他们正在塑造更好的你。”
>
> 所以，这一次再坚持努力一把，重塑 Furion 重塑自己。或许未来在某个 IT 圈但凡有人谈起 .NET 还能瞄到 Furion 的影子。

---

🎉 探索 Furion 未来更多可能性，实现无第三方依赖的版本。

- 作者：[百小僧](https://gitee.com/monksoul)
- 日期：2021 年 08 月 30 日

## 环境

- IDE ：[Microsoft Visual Studio Enterprise 2022 Preview (64 位) 版本 17.0.0 Preview 3.1](https://visualstudio.microsoft.com/zh-hans/vs/preview/)
- SDK ：[.NET SDK 6 Daily Version](https://github.com/dotnet/installer#installers-and-binaries)
- 语言：[C# 10](https://docs.microsoft.com/zh-cn/dotnet/csharp/whats-new/csharp-10)

## 状态

我们创建了一个详细的列表来轻松显示 Furion 探索版本的状态和演变。

| 图标 | 描述     |
| ---- | -------- |
| ⚠️   | 待定     |
| ⏳   | 进行中   |
| ✅   | 完成     |
| 💔   | 随时抛弃 |

## 概述

要跟踪正在进行的进度，请过滤处理程序标签。

### ✅ 应用 / App

🟡 **[文档手册](https://gitee.com/dotnetchina/Furion/tree/experimental/handbook/App) | [使用示例](https://gitee.com/dotnetchina/Furion/tree/experimental/samples/Furion.AppSamples) | [模块源码](https://gitee.com/dotnetchina/Furion/tree/experimental/framework/Furion/App) | [单元测试](https://gitee.com/dotnetchina/Furion/tree/experimental/tests/Furion.UnitTests/AppTests) | [集成测试](https://gitee.com/dotnetchina/Furion/tree/experimental/tests/Furion.IntegrationTests/AppTests)**

| 功能                                                      | 状态 |
| --------------------------------------------------------- | ---- |
| `IApp` 单例服务                                           | ✅   |
| `IApp` 支持解析服务、读取配置、获取环境信息               | ✅   |
| `AppSettings` 配置文件节点                                | ✅   |
| `App` 模块内置 `FURION_` 前缀环境配置和自定义配置文件装载 | ✅   |
| `App` 模块支持缺省和手动注册                              | ✅   |
| `App` 模块单元测试                                        | ✅   |
| `App` 模块集成测试                                        | ✅   |
| `App` 模块文档                                            | ✅   |
| `App` 模块使用例子                                        | ✅   |

### ✅ 配置 / Configuration

🟡 **[文档手册](https://gitee.com/dotnetchina/Furion/tree/experimental/handbook/Configuration) | [使用示例](https://gitee.com/dotnetchina/Furion/tree/experimental/samples/Furion.ConfigurationSamples) | [模块源码](https://gitee.com/dotnetchina/Furion/tree/experimental/framework/Furion/Configuration) | [单元测试](https://gitee.com/dotnetchina/Furion/tree/experimental/tests/Furion.UnitTests/ConfigurationTests) | [集成测试](https://gitee.com/dotnetchina/Furion/tree/experimental/tests/Furion.IntegrationTests/ConfigurationTests)**

| 功能                                                | 状态 |
| --------------------------------------------------- | ---- |
| 支持 `文件配置提供程序`，如 `.json`、`.xml`，`.ini` | ✅   |
| 支持 `内存 .NET 对象提供程序`                       | ✅   |
| 支持 `目录文件提供程序`                             | ✅   |
| 支持 `环境变量提供程序`                             | ✅   |
| 支持 `命令行参数提供程序`                           | ✅   |
| 支持自定义提供程序，如 `.txt` 文件提供程序          | ✅   |
| 支持 `IConfiguration` 接口统一读取配置              | ✅   |
| 支持 `AddFile` 自动识别文件类型及文件路径操作符解析 | ✅   |
| 支持各种数据配置转换读取                            | ✅   |
| 支持可选配置、基于环境配置、配置更改监听            | ✅   |
| 支持 `ChangeToken` 配置全局更改监听                 | ✅   |
| `Configuration` 模块单元测试                        | ✅   |
| `Configuration` 模块集成测试                        | ✅   |
| `Configuration` 模块文档                            | ✅   |
| `Configuration` 模块使用例子                        | ✅   |

### ⏳ 选项 / Options

🟡 **[文档手册](https://gitee.com/dotnetchina/Furion/tree/experimental/handbook/Options) | [使用示例](https://gitee.com/dotnetchina/Furion/tree/experimental/samples/Furion.OptionsSamples) | [模块源码](https://gitee.com/dotnetchina/Furion/tree/experimental/framework/Furion/Options) | [单元测试](https://gitee.com/dotnetchina/Furion/tree/experimental/tests/Furion.UnitTests/OptionsTests) | [集成测试](https://gitee.com/dotnetchina/Furion/tree/experimental/tests/Furion.IntegrationTests/OptionsTests)**

| 功能                                                                                                | 状态 |
| --------------------------------------------------------------------------------------------------- | ---- |
| 支持配置直接绑定或转换成 `Options` 对象                                                             | ✅   |
| 支持选项接口 `IOptions, IOptionsSnapshot, IOptionsMonitor` 读取                                     | ✅   |
| 支持命名选项                                                                                        | ✅   |
| 支持选项从配置绑定或通过依赖注入绑定                                                                | ✅   |
| 支持选项配置、后期配置                                                                              | ✅   |
| 支持选项单个值验证、多值多场景验证                                                                  | ✅   |
| 支持特性 `[OptionsBuilder]` 配置选项                                                                | ✅   |
| 支持单个或多个选项构建器类型配置                                                                    | ✅   |
| 支持通过 `IConfigureOptionsBuilder, IPostConfigureOptionsBuilder,IValidateOptionsBuilder ` 配置选项 | ✅   |
| 支持选项绑定非公开属性或字段                                                                        | ✅   |
| 支持主机启动期间访问选项                                                                            | ✅   |
| `Options` 模块单元测试                                                                              | ✅   |
| `Options` 集成单元测试                                                                              | ⏳   |
| `Options` 模块文档                                                                                  | ⏳   |
| `Options` 模块使用例子                                                                              | ⏳   |

### ⏳ 事件总线 / EventBus

🟡 **[文档手册](https://gitee.com/dotnetchina/Furion/tree/experimental/handbook/EventBus) | [使用示例](https://gitee.com/dotnetchina/Furion/tree/experimental/samples/Furion.EventBusSamples) | [模块源码](https://gitee.com/dotnetchina/Furion/tree/experimental/framework/Furion/EventBus) | [单元测试](https://gitee.com/dotnetchina/Furion/tree/experimental/tests/Furion.UnitTests/EventBusTests) | [集成测试](https://gitee.com/dotnetchina/Furion/tree/experimental/tests/Furion.IntegrationTests/EventBusTests)**

| 功能                                               | 状态 |
| -------------------------------------------------- | ---- |
| 支持自定义事件源                                   | ✅   |
| 支持自定义事件存取器                               | ✅   |
| 支持事件发布                                       | ✅   |
| 支持定义多个事件订阅者                             | ✅   |
| 支持事件订阅者定义多个处理程序（含一对一，一对多） | ✅   |
| 支持事件订阅过滤器（执行前，执行后，执行异常）     | ✅   |
| 支持事件处理程序取消                               | ✅   |
| 支持事件总线调用日志输出                           | ✅   |
| `EventBus` 模块单元测试                            | ⏳   |
| `EventBus` 模块集成测试                            | ⏳   |
| `EventBus` 模块文档                                | ⏳   |
| `EventBus` 模块使用例子                            | ⏳   |

### ⏳ 任务队列 / TaskQueue

| 功能                                               | 状态 |
| -------------------------------------------------- | ---- |
| 支持任务队列容量配置                               | ✅   |
| 支持任务队列 `IBackgroundTaskQueue` 入栈、出栈操作 | ✅   |
| 支持任务队列开始、暂停、取消操作                   | ✅   |
| 支持任务队列异步操作                               | ✅   |
| 支持任务队列日志输出                               | ✅   |
| 支持任务异常捕获                                   | ✅   |
| `TaskQueue` 模块单元测试                           | ⏳   |
| `TaskQueue` 集成单元测试                           | ⏳   |
| `TaskQueue` 模块文档                               | ⏳   |
| `TaskQueue` 模块使用例子                           | ⏳   |

### ⏳ 任务调度 / SchedulerTask

| 功能                                     | 状态 |
| ---------------------------------------- | ---- |
| 支持间隔定时任务                         | ✅   |
| 支持 `Cron` 表达式任务                   | ✅   |
| 支持 `串行` 和 `并行` 定时任务           | ✅   |
| 支持任务异常捕获                         | ✅   |
| 支持定时任务 `开始`、`暂停`、`取消` 控制 | ✅   |
| 支持任务调度日志输出                     | ✅   |
| `SchedulerTask` 模块单元测试             | ⏳   |
| `SchedulerTask` 集成单元测试             | ⏳   |
| `SchedulerTask` 模块文档                 | ⏳   |
| `SchedulerTask` 模块使用例子             | ⏳   |

### ⏳ 依赖注入 / Dependency Injection

| 功能                                                                           | 状态 |
| ------------------------------------------------------------------------------ | ---- |
| 替换 .NET 内置服务提供器工厂、并兼容 .NET 原生所有功能                         | ✅   |
| 支持单例、作用域、瞬时生存周期注册                                             | ✅   |
| 支持构造函数注入                                                               | ✅   |
| 支持函数参数 `[FromServices]` 注入                                             | ✅   |
| 支持属性 `[AutowiredServices]` 注入                                            | ✅   |
| 支持字段 `[AutowiredServices]` 注入                                            | ⏳   |
| 支持包装 `.NET` 原生 `IServiceCollection` 和 `IServiceProvider` 对象           | ✅   |
| 支持依赖接口解析生存周期接口并注册                                             | ✅   |
| 支持依赖接口工厂注册服务                                                       | ✅   |
| 支持特性配置依赖接口注册方式                                                   | ⏳   |
| 支持配置外部程序集扫描注册                                                     | ✅   |
| 支持泛型注入                                                                   | ✅   |
| 支持多实现注入                                                                 | ✅   |
| 支持 `INamedServiceProvider`、`IAppServiceProvider` 服务提供器，解析多服务实例 | ✅   |
| 支持 `AOP` 切面拦截                                                            | ⏳   |
| 依赖注入集成测试                                                               | ✅   |
| 依赖注入使用文档                                                               | ⚠️   |
| 依赖注入使用例子                                                               | ⚠️   |

### ⏳ 模块 / Module

| 功能                                | 状态 |
| ----------------------------------- | ---- |
| 支持特性配置                        | ⚠️   |
| 支持 `IHostBuilderService` 配置服务 | ✅   |
| 支持扫描程序集自动调用              | ✅   |

## 规范

- 采用一切为了依赖注入的设计模式
- 尽可能避免静态类和静态属性定义
- 尽可能保证方法原子性、职责单一性
- 所有 `private` 字段须以 `_` 开头且采用 `小驼峰命名`
- 所有类名、接口须采用 `大驼峰命名`，类型名采用 `名词` 拼接，接口须以 `I` 开头
- 如果类不考虑被继承，设置为 `sealed` 私有类，同时尽可能添加 `partial` 修饰符
- 如果类只是用来标记某个领域的属性，应采用 `结构` 定义
- 所有能够在类构造函数初始化的字典都必须采用 `private readonly` 修饰
- 如果类的属性无需外部赋值设置，应设置为 `{get;}` 或 `{get; internal set;}`
- 所有事件类须以 `EventArgs` 结尾
- 所有特性类须以 `Attribute` 结尾，且明确指定 `AttributeUsage` 特性
- 所有类型拓展类必须以 `{类型}Extensions.cs` 命名，且命名空间均遵循 `Furion.模块.Extensions` 格式，泛型类遵循 `{类型}OfTExtensions.cs` 命名
- 框架内部所有 `Helper` 拓展类及方法都应该是 `internal` 修饰
- `IServiceCollection` 拓展方法须以 `Add` 开头
- `IApplicationBuilder` 拓展方法须以 `Use` 开头
- 对 `.NET` 的拓展尽可能保持和原生一样的命名空间
- 所有中间件须以 `Middleware` 结尾
- 所有筛选器须以 `Filter` 或 `AsyncFilter` 结尾
- 框架内部默认实现须以 `App` 开头
- 所有私有的方法禁止定义为拓展方法
- 所有选项类须以 `SettingsOptions` 结尾
- 尽可能在方法第一行判断参数空，统一采用 `if (parameter == null) throw new ArgumentNullException(nameof(parameter));`
- 框架内部主动抛出内部异常均采用 `throw new InvalidOperationException($"消息.");` 方式
- 框架尽可能在需要输出和调试地方使用 `Trace.WriteLine(消息);` 输出
- 所有三元表达式代码须以 `?` 和 `:` 之前换行
- 进行多条件 `||` 或 `&&` 判断时，每个组须之前换行
- 所有 `Lambda` 操作代码，如果操作方法大于 `1` 个，须在每一个 `.Lambada<>` 之前换行
- 构造函数如果定义超过一个参数，则在第二个开始换行，且以 `,` 开头
- 框架内部尽可能保证模块零依赖性，除了公共 `Helpers`
- 框架内所有的类、属性、方法、接口、事件、字段等成员必须采用 `///` 注释
- 方法内的代码尽可能一组逻辑空一行且添加组注释，采用代码上行 `//` 注释
- 框架内每个文件头部须添加开源协议描述头
- `///` 注释尽可能简单命令，如需更多说明，须在 `<remarks></remarks>` 中编写，如果需要多行，采用 `<para></para>` 或 `<code>/</code>`
- 框架中所有非服务类如果实现某接口方法，须采用 `显式实现`，避免外部调用
- 所有测试方法须以 `Test` 开头，同时建议采用 `[Theory]`，而不是 `[Fact]`
- 所有后台执行操作必须提供日志输出
- 方法参数超过两个则从第二个开始换行且`,` 置前面

## 测试

参照 [https://github.com/dotnet/aspnetcore/tree/main/src/DefaultBuilder/test/Microsoft.AspNetCore.Tests](https://github.com/dotnet/aspnetcore/tree/main/src/DefaultBuilder/test/Microsoft.AspNetCore.Tests)
