using Furion.SchedulerSamples;

var builder = WebApplication.CreateBuilder(args).UseFurion();

// Add services to the container.

builder.Services.AddSchedulerJob(builder =>
{
    builder.AddJob<TestCronJob>("cron_job", builder =>
    {
        builder.AddCronTrigger("* * * * *");
    });

    builder.AddJob<TestPeriodJob>("period_job", builder =>
    {
        builder.AddPeriodTrigger(10000)
               .AddCronTrigger("* * * * * *", Furion.TimeCrontab.CronStringFormat.WithSeconds);
    });
    builder.AddJob<TestCronJob2>("cron_job_2", builder => builder.AddPeriodTrigger(3000));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
