using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args).Inject();  // ×¢Èë Furion
var app = builder.Build();
app.Run();
