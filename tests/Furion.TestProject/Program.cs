using Furion.TestProject.Controllers;
using Furion.TestProject.Filters;
using Furion.TestProject.Services;

var builder = WebApplication.CreateBuilder(args).UseFurion();

builder.Configuration.AddFile("furion.json optional=true reloadOnChange=true");

builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
{
    ["Memory:Item"] = "MemoryValue"
});

builder.Configuration.AddKeyPerFile(Path.Combine(Directory.GetCurrentDirectory(), "key-per-file"));

builder.Services.AsServiceBuilder(builder.Configuration, builder.Host.Properties)
    .AddNamedService<ITestNamedService, Test1NamedService>("test1", ServiceLifetime.Transient)
    .AddNamedService<ITestNamedService, Test2NamedService>("test2", ServiceLifetime.Transient)
    .Build();

builder.Services.AsServiceBuilder(builder.Configuration, builder.Host.Properties)
    .AddNamedService<ITestNamedService, Test1NamedService>("test3", ServiceLifetime.Transient)
    .AddNamedService<ITestNamedService, Test2NamedService>("test4", ServiceLifetime.Transient)
    .Build();

// Add services to the container.
builder.Services.AddTransient<IAutowriedService, AutowriedService>();

// ×¢²á HttpContext ·ÃÎÊÆ÷
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers().AddAutowired();
builder.Services.AddScoped<TestControllerFilter>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Furion.TestProject", Version = "v1" });
});

builder.Services.AddAppOptions<TestSettingsOptions>(builder.Configuration, options =>
{
    options.Name ??= "Furion";
});
builder.Services.AddAppOptions<Test2SettingsOptions>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Furion.TestProject v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
