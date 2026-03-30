using Application.DTOs.Common;
using Application.DTOs.PriceCandles;

using SORMAnalytics.Application.Common.Exceptions;

namespace Web.Services.HATEOAS;
public class LinkService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
{
    public LinkDto Create(string endpointName, string rel, string method, object? values = null)
    {
        var href = linkGenerator.GetUriByName(httpContextAccessor.HttpContext!, endpointName, values);
        return new LinkDto
        {
            Href = href ?? throw new GenericException(404, "Some error occured with hypermedia values."),
            Rel = rel,
            Method = method
        };
    }
    public List<LinkDto> CreatePriceCandleLinks(PriceCandleParameters query, bool hasNextPage, bool hasPreviousPage)
    {
        List<LinkDto> links =
        [
            Create("GetPriceCandle", "self", HttpMethods.Get, new 
            {
                page = query.Page,
                pageSize = query.PageSize,
                symbol = query.Symbol,
                fields = query.Fields
            }),
            Create("GetBatchAnalysis", "batchAnalysis", HttpMethods.Post)
        ];
        if (hasNextPage)
        {
            links.Add(Create("GetPriceCandle", "nextPage", HttpMethods.Get, new
            {
                page = query.Page + 1,
                pageSize = query.PageSize,
                symbol = query.Symbol,
                fields = query.Fields
            }));
        }
        if (hasPreviousPage)
        {
            links.Add(Create("GetPriceCandle", "previousPage", HttpMethods.Get, new
            {
                page = query.Page - 1,
                pageSize = query.PageSize,
                symbol = query.Symbol,
                fields = query.Fields
            }));
        }
        return links;
    }
}
