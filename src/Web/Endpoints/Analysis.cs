using Application.DTOs.Analysis;
using Application.PriceCandles.Queries.Analysis;

using Microsoft.AspNetCore.Http.HttpResults;

namespace SORMAnalytics.Web.Endpoints;

public class Analysis : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
       // groupBuilder.MapGet(GetGoldenCross).CacheOutput();
        groupBuilder.MapPost(GetBatchAnalysis).CacheOutput();
    }
    public async Task<Ok<BatchAnalysisDto>> GetBatchAnalysis(ISender sender, [AsParameters] GetBatchAnalysisQuery query, ILogger<Analysis> logger)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }
}
