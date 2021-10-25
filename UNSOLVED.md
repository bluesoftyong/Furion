## Furion 探索版之未解之谜

---

### `EventBus` 事件总线待解决问题：（提出：2021.10.24，完成：2021.10.25）

在 `EventBusHostedService` 后台事件总线服务中对具体 `Handler` 调用不够灵活，原因是在 `BackgroundProcessing` 方法中直接调用，这样就暴露了几个问题：

1、如果后续这个 `Handler` 需要添加 `策略机制`，如 `失败重试`，`超时取消` 等等功能，那么 `BackgroundProcessing` 方法就会变的越来越臃肿。

```
解决方案：

应该抽象出 `IEventHandlerExecutor` 接口，如果开发者没有实现这个接口，则直接调用，否则交由开发者处理。

`IEventHandlerExecutor` 接口可以这样设计：

public interface IEventHandlerExecutor
{
    Task ExecuteAsync(EventHandlerExecutingContext context, Func<EventHandlerExecutingContext, Task> handler);
}

这样开发者可以自行实现调用或增加策略，如：

public class EventHandlerExecutor : IEventHandlerExecutor
{
    public async Task ExecuteAsync(EventHandlerExecutingContext context, Func<EventHandlerExecutingContext, Task> handler)
    {
        if (context.Source.EventId == "User:Delete")
        {
            Console.WriteLine("我是执行器~~~~");
            await handler(context);
        }
        else
        {
            await handler(context);
        }
    }
}

```

通过这样设计，把调用方法主动权交给开发者，这样灵活性可以达到无限放大。

2、监视器设计不够灵活，目前是在 `IEventSubscriber` 实现类中实现 `IEventHandlerMonitor` 接口，这样带来的问题是如果多个订阅者都需要监视则出现大量重复代码

```
解决方案：

1. 改造 EventBusOptionsBuilder，所有事件总线的服务全部在这个构建器中完成，保证 `IEventHandlerMonitor` 单一性。
```

解决这两个问题之后，`EventBus` 事件总线模块才算真正完成核心设计。
