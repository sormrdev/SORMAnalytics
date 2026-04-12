using Application.Common.Services.Sorting;
using Application.DTOs.Analysis;
using Application.DTOs.PriceCandles;

using AutoMapper;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using SORMAnalytics.Application.Common.Exceptions;
using SORMAnalytics.Application.Common.Interfaces;
using SORMAnalytics.Application.Common.Logging;
using SORMAnalytics.Domain.Entities;
using SORMAnalytics.Domain.Interfaces;

namespace Application.PriceCandles.Queries.Analysis;
public record GetBatchAnalysisQuery(BatchAnalysisParamsDto requestParams) : IRequest<BatchAnalysisDto>;
public class GetBatchAnalysisHandler : IRequestHandler<GetBatchAnalysisQuery, BatchAnalysisDto>
{
    private readonly IBatchAnalyser _analyser;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<GetBatchAnalysisHandler> _logger;
    private readonly IValidator<BatchAnalysisParamsDto> _validator;
    private readonly SortMappingProvider _sortMappingProvider;

    public GetBatchAnalysisHandler(IBatchAnalyser analyser, 
        IApplicationDbContext context, 
        IMapper mapper, 
        ILogger<GetBatchAnalysisHandler> logger, 
        IValidator<BatchAnalysisParamsDto> validator,
        SortMappingProvider sortMappingProvider)
    {
        _analyser = analyser;
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
        _sortMappingProvider = sortMappingProvider;
    }

    public async Task<BatchAnalysisDto> Handle(GetBatchAnalysisQuery request, CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(request.requestParams, cancellationToken);

        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
        if (!_sortMappingProvider.ValidateMappings<PriceCandleDto, PriceCandle>(request.requestParams.sort))
        {
            throw new SortingException(request.requestParams.sort);
        }

        IEnumerable<string> symbols = request.requestParams.symbols.Split(',')
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s));

        SortMapping[] sortMappings = _sortMappingProvider.GetMappings<PriceCandleDto, PriceCandle>();

        var maxOffset = request.requestParams.condition.rules.Max(c => Math.Max(c.firstValueDayOffset, c.secondValueDayOffset));
        var minOffset = request.requestParams.condition.rules.Min(c => Math.Min(c.firstValueDayOffset, c.secondValueDayOffset));
        var minDate = request.requestParams.condition.fromDate.AddDays(minOffset);
        var maxDate = request.requestParams.condition.toDate.AddDays(maxOffset);

        var candles = await _context.PriceCandles
                .Where(c => symbols.Contains(c.Symbol))
                .Where(c => c.Timestamp >= minDate && c.Timestamp <= maxDate)
                .AsNoTracking()
                .ApplySort(request.requestParams.sort, sortMappings)
                .ToListAsync(cancellationToken);

        _logger.FoundCandles(candles.Count, minDate, maxDate);

        Dictionary<string, SymbolAnalysisDto> candlesFound = new();
        
        foreach (var symbol in symbols)
        {
            var filteredCandles = candles.Where(x => x.Symbol == symbol);

            (var analysisResult, var candlesInvolvedInCalc) = _analyser.Analyse(filteredCandles, _mapper.Map<BatchAnalysisCondition>(request.requestParams.condition));

            if (analysisResult != null)
            {
                candlesFound[symbol] = new SymbolAnalysisDto(
                    _mapper.Map<IEnumerable<PriceCandleDto>>(analysisResult),
                    _mapper.Map<IEnumerable<PriceCandleDto>>(candlesInvolvedInCalc)
                );
            }
        }

        return new BatchAnalysisDto(candlesFound);
    }
}