using System.Text;

using Application.Common.Interfaces;

using Infrastructure.Data;
using Infrastructure.JWT;
using Infrastructure.Settings;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAuthOptions.Key))

                };
            });

        builder.Services.AddAuthorization();
    }
}
