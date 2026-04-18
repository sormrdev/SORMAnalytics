using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Application.Common.Interfaces;

using Infrastructure.Data;
using Infrastructure.JWT;
using Infrastructure.Settings;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

using SORMAnalytics.Application.Common.Interfaces;
using SORMAnalytics.Infrastructure.Data;
using SORMAnalytics.Infrastructure.FMP;

namespace SORMAnalytics.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("SORMADb");

        builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(options =>
            options
            .UseNpgsql(connString,
                       npsqlOptions => npsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Candles))
            .UseSnakeCaseNamingConvention());

        builder.Services.AddDbContext<IIdentityDbContext, ApplicationIdentityDbContext>(options =>
           options
           .UseNpgsql(connString,
                      npsqlOptions => npsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Identity))
           .UseSnakeCaseNamingConvention());

        builder.Services.AddOptions<FmpOptions>()
            .BindConfiguration(FmpOptions.SectionName);

        builder.Services.AddHttpClient<IPriceCandleProvider, FmpDailyService>();

        builder.Services.AddTransient<ITokenProvider, TokenProvider>();

        builder.Services.AddSingleton<IJwtAuthOptions>(sp =>
           sp.GetRequiredService<IOptions<JwtAuthOptions>>().Value);
    }

    public static void AddAuthenticationServices(this IHostApplicationBuilder builder)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

        builder.Services
            .AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>();

        builder.Services.Configure<JwtAuthOptions>(builder.Configuration.GetSection("Jwt"));

        JwtAuthOptions jwtAuthOptions = builder.Configuration.GetSection("Jwt").Get<JwtAuthOptions>()!;

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtAuthOptions.Issuer,
                    ValidAudience = jwtAuthOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.Key)),

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    RoleClaimType = "role",
                    NameClaimType = "sub"
                };
            });

        builder.Services.AddAuthorization();
    }

    public static async Task SeedRolesAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = ["Admin", "User"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public static async Task SeedAdminAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        var email = config["AdminBootstrap:Email"];

        if (string.IsNullOrWhiteSpace(email))
            return;

        var user = await userManager.FindByEmailAsync(email);

        if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
        {
            await userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
