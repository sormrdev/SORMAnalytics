using System.Dynamic;

using Carter.Response;

using Microsoft.AspNetCore.Http.HttpResults;

using Microsoft.AspNetCore.Mvc;

using Web.Infrastructure;
using Web.Services.HATEOAS;
using Application.PriceCandles.Commands.PriceCandles;
using Application.PriceCandles.Queries.Assets;
using Application.DTOs.Common;
using Application.DTOs.PriceCandles;
using Application.PriceCandles.Queries.PriceCandles;

namespace SORMAnalytics.Web.Endpoints;

public class Assets : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetPriceCandle);
        groupBuilder.MapGet(GetAvailableAssets, "AvailableAssets").CacheOutput();
        groupBuilder.MapPost(CreatePriceCandles).RequireAuthorization(policy =>
            policy.RequireRole("Admin"));
    }
    public async Task GetPriceCandle(
         LinkService linkService,
         ISender sender,
         [AsParameters] PriceCandleParameters parameters,
         HttpResponse httpResponse,
         [FromHeader(Name = "Accept")] string? accept)
    {
        var result = await sender.Send(new GetPriceCandleQuery(parameters));

        if (CustomMediaTypeNames.Application.isHateoasJson(accept))
        {
            var hyper = new HypermediaResponse<PaginationResult<ExpandoObject>>
            {
                Data = result,
                Links = linkService.CreatePriceCandleLinks(
                    parameters,
                    result.HasNextPage,
                    result.HasPreviousPage)
            };
            await httpResponse.Negotiate(hyper);
            return;
        }
        await httpResponse.Negotiate(result);
    }
    public async Task<Ok<IEnumerable<string>>> GetAvailableAssets(ISender sender, [AsParameters] GetAvailableAssetsQuery query)
    {
        var result = await sender.Send(query);

        return TypedResults.Ok(result);
    }
    public async Task<Ok> CreatePriceCandles(ISender sender, [AsParameters] CreatePriceCandleCommand command)
    {
        await sender.Send(command);

        return TypedResults.Ok();
    }
}
