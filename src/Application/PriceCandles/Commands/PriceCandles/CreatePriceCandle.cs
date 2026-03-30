using Application.PriceCandles.Queries.Assets;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SORMAnalytics.Application.Common.Interfaces;
using SORMAnalytics.Domain.Interfaces;

namespace Application.PriceCandles.Commands.PriceCandles;

public class CreatePriceCandleCommand : IRequest {}
public class CreatePriceCandleCommandHandler : IRequestHandler<CreatePriceCandleCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPriceCandleProvider _candleProvider;
    private readonly IMediator _mediator;
    private readonly ICalculatePriceCandle _candleCalculator;
    public CreatePriceCandleCommandHandler(IApplicationDbContext context, IPriceCandleProvider candleProvider, IMediator mediator, ICalculatePriceCandle candleCalculator)
    {
        _context = context;
        _candleProvider = candleProvider;
        _mediator = mediator;
        _candleCalculator = candleCalculator;
    }
    public async Task Handle(CreatePriceCandleCommand request, CancellationToken cancellationToken)
    {
        var assetsToFetch = await _mediator.Send(new GetAvailableAssetsQuery(), cancellationToken);
        foreach(var symbol in assetsToFetch)
        {
            var candles = await _candleProvider.GetPriceCandleAsync(symbol, cancellationToken);
            var candlesList = candles.OrderBy(c => c.Timestamp).ToList();

            var existingDates = await _context.PriceCandles
                                .Where(x => x.Symbol == symbol)
                                .Select(x => x.Timestamp)
                                .ToHashSetAsync(cancellationToken);

             var newCandles = candles
                                .Where(c => !existingDates.Contains(c.Timestamp))
                                .ToList();

            if (newCandles.Count == 0)
                continue;

            _candleCalculator.Calculate(candlesList);

            _context.PriceCandles.AddRange(newCandles);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }
}
