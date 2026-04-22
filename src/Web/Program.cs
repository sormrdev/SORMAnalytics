using Carter;

using System.Reflection;

using Hangfire;

using Scalar.AspNetCore;

using SORMAnalytics.Infrastructure.Data.Extensions;
using SORMAnalytics.Web;

using Swashbuckle.AspNetCore.Filters;

using Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();
builder.AddAuthenticationServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

builder.AddHangfire();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    //// Access this at http://localhost:PORT/scalar/v1
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapScalarApiReference();
    await app.ApplyMigrationsAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

await app.SeedRolesAsync();
await app.SeedAdminAsync();

app.UseHangfireDashboard();

app.UseExceptionHandler(options => { });

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.MapEndpoints();
app.MapCarter();

app.Services.ScheduleJobs();

app.Run();
