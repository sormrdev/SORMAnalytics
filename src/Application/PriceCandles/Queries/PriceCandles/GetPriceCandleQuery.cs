using System.Dynamic;

using Application.Common.Services.DataShaping;
using Application.DTOs.Common;
using Application.DTOs.PriceCandles;

using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SORMAnalytics.Application.Common.Exceptions;
using SORMAnalytics.Application.Common.Interfaces;

namespace Application.PriceCandles.Queries.PriceCandles;

public record GetPriceCandleQuery(PriceCandleParameters parameters) : IRequest<PaginationResult<ExpandoObject>>;

public class GetPriceCandleQueryHandler : IRequestHandler<GetPriceCandleQuery, PaginationResult<ExpandoObject>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly DataShapingService _dataShapingService;

    public GetPriceCandleQueryHandler(IApplicationDbContext context, IMapper mapper, DataShapingService dataShaper)
    {
        _context = context;
        _mapper = mapper;
        _dataShapingService = dataShaper;
    }

    public async Task<PaginationResult<ExpandoObject>> Handle(GetPriceCandleQuery request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(request.parameters.Fields) && !_dataShapingService.ValidateFields<PriceCandleDto>(request.parameters.Fields))
        {
            throw new GenericException(400, $"The provided data shaping fields aren't valid '{request.parameters.Fields}'");
        }
        var query = _context.PriceCandles
                .Where(c => c.Symbol == request.parameters.Symbol);

        var data = await query
                .Skip((request.parameters.Page - 1) * request.parameters.PageSize)
                .Take(request.parameters.PageSize)
                .ToListAsync(cancellationToken);

        int totalCount = await query
                .CountAsync(cancellationToken);

        var dto = _mapper.Map<IEnumerable<PriceCandleDto>>(data).OrderByDescending(q => q.Timestamp);

        var paginationResult = new PaginationResult<ExpandoObject>()
        {
            Items = _dataShapingService.ShapeDataCollection(dto, request.parameters.Fields),
            Page = request.parameters.Page,
            PageSize = request.parameters.PageSize,
            TotalCount = totalCount,
        };

        return paginationResult;
    }
}