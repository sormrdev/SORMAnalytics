using Application.DTOs.Analysis;
using Application.PriceCandles.Queries.Analysis;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using SORMAnalytics.Web.Swagger;

using Swashbuckle.AspNetCore.Filters;

namespace SORMAnalytics.Web.Endpoints;

public class Analysis : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        // groupBuilder.MapGet(GetGoldenCross).CacheOutput();
        groupBuilder.MapPost(GetBatchAnalysis).CacheOutput();
    }

    [SwaggerRequestExample(typeof(BatchAnalysisParamsDto), typeof(BatchAnalysisParamsDtoExample))]
    public async Task<Ok<BatchAnalysisDto>> GetBatchAnalysis(ISender sender, [FromBody] BatchAnalysisParamsDto requestParams, ILogger<Analysis> logger)
    {
        var result = await sender.Send(new GetBatchAnalysisQuery(requestParams));
        return TypedResults.Ok(result);
    }
}
