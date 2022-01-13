using Furion.Schedule;
using Microsoft.AspNetCore.Mvc;

namespace Furion.SchedulerSamples.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ISchedule _schedule;
        public TestController(ISchedule schedule)
        {
            _schedule = schedule;
        }

        [HttpPost]
        public void AddJob()
        {
            _schedule.AddJob(JobBuilder.Create<TestPeriodJob>()
                                             .WithIdentity("test_job")
                                             .BindTriggers(
                                                TriggerBuilder.Period(2000).WithIdentity("test_trg"))
                                             );

            _schedule.NotifyChanges();
        }

        [HttpPost]
        public void RemoveJob()
        {
            _schedule.RemoveJob("test_job");
        }

        [HttpPost]
        public void UpdateTrigger()
        {
            _schedule.GetJob("test_job")
                     ?.UpdateTrigger("test_trg", t =>
                     {
                         t.SetNumberOfRuns(10000);
                     });

            //_schedule.NotifyChanges();
        }
    }
}