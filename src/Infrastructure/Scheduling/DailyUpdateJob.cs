using Application.PriceCandles.Commands.PriceCandles;

using MediatR;

namespace SORMAnalytics.Infrastructure.Scheduling;

public class DailyUpdateJob
{
    private readonly ISender _sender;

    public DailyUpdateJob(IServiceProvider serviceProvider, ISender sender)
    {
        _sender = sender;
    }

    public async Task Execute()
    {
        await _sender.Send(new CreatePriceCandleCommand());
    }
}
