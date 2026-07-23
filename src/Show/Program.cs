using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Show.Api.Configurations;
using Show.Domain;
using Show.Infrastructure;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
var port = builder.Configuration.GetRequiredSection("Port").Get<int>();

builder.WebHost.UseSetting(WebHostDefaults.ServerUrlsKey, $"http://localhost:{port}");
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddInfrastructureServices(builder.Configuration)
    .AddDomainServices();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.ConfigureRoutes();
app.Run();
