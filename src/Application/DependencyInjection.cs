using System.Reflection;

using Application.Common.Services.DataShaping;
using Application.Common.Services.Sorting;
using Application.DTOs.Analysis;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SORMAnalytics.Domain.Entities;
using SORMAnalytics.Domain.Interfaces;
using SORMAnalytics.Domain.Services;

namespace SORMAnalytics.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
        builder.Services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
        builder.Services.AddSingleton<ICalculatePriceCandle, CalculatePriceCandles>();
        builder.Services.AddSingleton<IBatchAnalyser, BatchAnalysisService>();
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        builder.Services.AddSingleton<ISortMappingDefinition>(
            BatchAnalysisMapping.SortMapping);
        builder.Services.AddTransient<SortMappingProvider>();
        builder.Services.AddTransient<DataShapingService>();
    }
}
