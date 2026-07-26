using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Booking.Api.Configurations;
using Booking.Application;
using Booking.Infrastructure;
using Booking.Migration;

var builder = WebApplication.CreateBuilder(args);
var port = builder.Configuration.GetRequiredSection("Port").Get<int>();
var jwtSecret = builder.Configuration["Jwt:Secret"]!;

builder.Logging.ClearProviders().AddConsole();
builder.WebHost.UseSetting(WebHostDefaults.ServerUrlsKey, $"http://localhost:{port}");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
    });
builder.Services
    .AddAuthorization()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                []
            }
        });
    })
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices()
    .AddMigrationServices();

var app = builder.Build();

using var scope = app.Services.CreateScope();
scope.ServiceProvider.GetRequiredService<MigrationService>().Migrate();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.ConfigureRoutes(app.Services.GetRequiredService<ILogger<WebApplication>>());
app.Run();
