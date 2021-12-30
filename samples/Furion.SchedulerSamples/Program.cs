using Furion.SchedulerSamples;

var builder = WebApplication.CreateBuilder(args).UseFurion();

// Add services to the container.

builder.Services.AddSchedulerJob(builder =>
{
    builder.AddJob<TestCronJob>("cron_job", builder =>
    {
        builder.AddCronTrigger("* * * * *");
    });

    builder.AddJob<TestSimpleJob>("simple_job", builder =>
    {
        builder.AddSimpleTrigger(10000)
               .AddCronTrigger("* * * * * *", Furion.TimeCrontab.CronStringFormat.WithSeconds);
    });
    //builder.AddJob<TestCronWithSecondsJob>();
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
