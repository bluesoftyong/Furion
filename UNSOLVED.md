## Furion 探索版之未解之谜

-----

### `EventBus` 事件总线待解决问题：（2021.10.24）

在 `EventBusHostedService` 后台事件总线服务中对具体 `Handler` 调用不够灵活，原因是在 `BackgroundProcessing` 方法中直接调用，这样就暴露了几个问题：

1、如果后续这个 `Handler` 需要添加 `策略机制`，如 `失败重试`，`超时取消` 等等功能，那么 `BackgroundProcessing` 方法就会变的越来越臃肿。

```
解决方案：

应该抽象出 `IEventHandlerPolicy` 接口，如果开发者没有实现这个接口，则直接调用，否则交由开发者处理。

`IEventHandlerPolicy` 接口可以这样设计：

public interface IEventHandlerPolicy
{
    Task ExecuteAsync(EventSubscribeExecutingContext context, Func<EventSubscribeExecutingContext, CancellationToken, Task> hanlder, CancellationToken cancellationToken);
}

这样开发者可以自行实现调用或增加策略，如：

public class RetryEventHandlerPolicy : IEventHandlerPolicy
{
    public async Task ExecuteAsync(EventSubscribeExecutingContext context, Func<EventSubscribeExecutingContext, CancellationToken, Task> hanlder, CancellationToken cancellationToken)
    {
       await RetryAsync(async () => 
       { 
           await handler(context, cancellationToken);
       }, 3);
    }
}

```

通过这样设计，把调用方法主动权交给开发者，这样灵活性可以达到无线放大。

2、过滤器设计不够灵活，目前是在 `IEventSubscriber` 实现类中实现 `IEventSubscriberFilter` 接口，这样带来的问题是如果有多个 `IEventSubscriber` 实现类，那么就会出现大量重复代码。

```
解决方案：

1. 添加 [EventSubcribeFilter(typeof(Filter))] 特性
通过该特性指定单个订阅者过滤器，同时这个 Filter 需实现 IEventSubscriber 接口。

2. 为 EventBusOptions 选项添加 Filters 属性
通过该属性注册全局 IEventSubscriber 过滤器
之后在 BackgroundProcessing 方法中通过尾递归算法实现类似中间件过滤器
```

解决这两个问题之后，`EventBus` 事件总线模块才算真正完成核心设计。