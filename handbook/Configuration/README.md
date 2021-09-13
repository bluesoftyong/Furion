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
- `环境变量提供程序`：可从系统环境变量、用户环境变量读取配置
- `命令行参数提供程序`：支持命令行方式启动应用并且传入 `args` 参数
- `内存 .NET 对象提供程序`：支持将集合数据存在到内存中供应用读取
- `目录文件 Key-per-file 提供程序`：使用目录的文件作为配置键值对，该键为文件名，该值为文件内容。

除此之外，`Furion` 框架也提供强大的自定义配置提供程序行为，支持从数据库、`Redis` 等任何存储介质提供配置信息。
