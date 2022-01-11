using Furion.SchedulerSamples;
using Furion.TimeCrontab;

var builder = WebApplication.CreateBuilder(args).UseFurion();

// Add services to the container.
builder.Services.AddScoped<ITestScopedService, TestScopedService>();
builder.Services.AddTransient<ITestTransientService, TestTransientService>();

builder.Services.AddSchedule(builder =>
{
    builder.AddJob<TestCronJob>(builder =>
    {
        builder.AddCronTrigger("* * * * *")
               .ConfigureDetail(j =>
               {
                   j.WithExecutionLog = true;
               })
               .WithIdentity("cron_job");
    });

    builder.AddJob<TestPeriodJob>(builder =>
    {
        builder.AddPeriodTrigger(5000)
               .AddCronTrigger("*/17 * * * * *", CronStringFormat.WithSeconds)
               .WithIdentity("period_trigger");
    });
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
