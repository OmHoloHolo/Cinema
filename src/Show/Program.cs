using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Show.Api.Configurations;
using Show.Domain;
using Show.Infrastructure;
using Show.Migration;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
var port = builder.Configuration.GetRequiredSection("Port").Get<int>();

builder.Logging.ClearProviders().AddConsole();
builder.WebHost.UseSetting(WebHostDefaults.ServerUrlsKey, $"http://localhost:{port}");
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddInfrastructureServices(builder.Configuration)
    .AddDomainServices()
    .AddMigrationServices();

var app = builder.Build();

using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<MigrationService>().Migrate();

app.UseSwagger();
app.UseSwaggerUI();
app.ConfigureRoutes();
app.Run();