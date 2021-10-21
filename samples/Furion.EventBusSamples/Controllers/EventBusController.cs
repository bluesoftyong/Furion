using Furion.EventBus;
using Microsoft.AspNetCore.Mvc;

namespace Furion.EventBusSamples.Controllers;

/// <summary>
/// EventBus 模块测试
/// </summary>
[Route("api/[controller]/[action]")]
[ApiController]
public class EventBusController : ControllerBase
{
    private readonly IEventPulisher _eventPulisher;
    public EventBusController(IEventPulisher eventPulisher)
    {
        _eventPulisher = eventPulisher;
    }

    /// <summary>
    /// 发送创建用户消息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task SendCreateUser()
    {
        await _eventPulisher.PublishAsync("User:Create", new
        {
            Name = "Furion"
        });
    }

    /// <summary>
    /// 发送更新用户消息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task SendUpdateUser()
    {
        await _eventPulisher.PublishAsync("User:Update", new
        {
            Id = 2,
            Name = "先知"
        });
    }
}