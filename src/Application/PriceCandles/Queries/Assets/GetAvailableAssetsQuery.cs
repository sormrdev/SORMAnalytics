using MediatR;

using Microsoft.EntityFrameworkCore;

using SORMAnalytics.Application.Common.Interfaces;

namespace Application.PriceCandles.Queries.Assets;

public class GetAvailableAssetsQuery : IRequest<IEnumerable<string>> { }

public class GetAvailableAssetsQueryHandler : IRequestHandler<GetAvailableAssetsQuery, IEnumerable<string>>
{
    private readonly IApplicationDbContext _context;
    public GetAvailableAssetsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<string>> Handle(GetAvailableAssetsQuery request, CancellationToken cancellationToken)
    {
        return await _context.AssetsToFetch
                .Select(a => a.Symbol)
                .ToListAsync(cancellationToken);
    }
}