using AutoMapper;
using Hangfire;
using Orbit.BLL.DI;
using Orbit.BLL.Interfaces;
using Orbit.BLL.Services;
using Orbit.DAL.Context;
using Orbit.PL.Authentication;
using Orbit.PL.Configuration;
using Orbit.Console.Extensions;
using Orbit.PL.Extensions;
using Orbit.PL.Middleware;
using Orbit.PL.Services;
using Orbit.Providers.DI;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Register DbContext
builder.Services.AddDbContext<OrbitDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.ConfigureWarnings(warnings => warnings.Log(RelationalEventId.PendingModelChangesWarning));
});

// Register Business Logic (MediatR, Services, Matching)
builder.Services.AddBusinessLogic();

// Register AutoMapper
builder.Services.AddAutoMapper(cfg => { }, typeof(Orbit.BLL.DI.DependencyInjection).Assembly, typeof(Program).Assembly);

// Register Posting Providers
builder.Services.AddLinkedInProvider();

// Register Orbit Console
builder.Services.AddOrbitConsole();

builder.Services.AddHangfireInfrastructure(builder.Configuration);

// Register Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
    options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
})
.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
    ApiKeyAuthenticationOptions.DefaultScheme, null);

builder.Services.AddAuthorization();

// Bind Strongly-Typed Configuration Options
builder.AddAppConfiguration();

// Register Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHangfireDashboard("/hangfire");
    app.Services.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/api/health", () => Results.Ok("Healthy")).AllowAnonymous();
app.MapControllers();
app.MapOrbitConsole();

await app.MigrateDatabaseAsync();
await app.SyncHangfireSchedulesAsync();

app.Run();
