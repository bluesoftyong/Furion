using Furion.TestProject.Controllers;

var builder = WebApplication.CreateBuilder(args).Inject();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Furion.TestProject", Version = "v1" });
});

builder.Services.AddAppOptions<TestSettingsOptions>(builder.Configuration.GetSection("TestSettings"), options =>
{
    options.Name ??= "Furion";
});
builder.AddAppOptions<Test2SettingsOptions>();

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
