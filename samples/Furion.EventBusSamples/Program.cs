using Furion.EventBusSamples;
using Furion.EventBusSamples.Subscribers;

var builder = WebApplication.CreateBuilder(args).UseFurion();

// Add services to the container.

// 注册事件总线
builder.Services.AddEventBus(builder =>
{
    builder.AddSubscriber<UserEventSubscriber>()
        .AddMonitor<EventHandlerMonitor>()
        .AddExecutor<EventHandlerExecutor>();
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
