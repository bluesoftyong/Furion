using Furion.TimeCrontab;
using Microsoft.AspNetCore.Mvc;

namespace Furion.TimeCrontabSamples;

[Route("api/[controller]/[action]")]
[ApiController]
public class TimeCrontabController : ControllerBase
{
    [HttpPost]
    public DateTime GetNextOccurrence([FromBody] string cron, CronStringFormat format = CronStringFormat.Default)
    {
        var crontab = Crontab.Parse(cron, format);
        return crontab.GetNextOccurrence(DateTime.Now);
    }

    [HttpPost]
    public string Parse([FromBody] string cron, CronStringFormat format = CronStringFormat.Default)
    {
        var crontab = Crontab.Parse(cron, format);
        return crontab.ToString();
    }
}