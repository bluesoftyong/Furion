using Furion.TimeCrontab;
using Microsoft.AspNetCore.Mvc;

namespace Furion.TimeCrontabSamples;

[Route("api/[controller]/[action]")]
[ApiController]
public class TimeCrontabController : ControllerBase
{
    [HttpPost]
    public DateTime GetNextOccurrence([FromBody] string cron)
    {
        var crontabSchedule = CrontabSchedule.Parse(cron);
        return crontabSchedule.GetNextOccurrence(DateTime.UtcNow);
    }
}