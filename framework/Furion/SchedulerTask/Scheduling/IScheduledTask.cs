namespace Furion.SchedulerTask;

public interface IScheduledTask
{
    string Schedule { get; }
    Task ExecuteAsync(CancellationToken cancellationToken);
}
