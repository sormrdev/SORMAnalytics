using Carter;

using Scalar.AspNetCore;

using SORMAnalytics.Infrastructure.Data.Extensions;
using SORMAnalytics.Web;

var builder = WebApplication.CreateBuilder(args);

builder.AddApplicationServices();
builder.AddInfrastructureServices();
builder.AddWebServices();
builder.AddAuthenticationServices();

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
    app.MapOpenApi();
    // Access this at http://localhost:PORT/scalar/v1
    app.MapScalarApiReference();
    await app.ApplyMigrationsAsync();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseExceptionHandler(options => { });

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.UseOutputCache();

app.MapEndpoints();
app.MapCarter();

app.Run();
