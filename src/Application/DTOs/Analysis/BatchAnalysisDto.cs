using Application.Common.Services.Sorting;
using Application.DTOs.PriceCandles;

using AutoMapper;

using FluentValidation;

using SORMAnalytics.Domain.Entities;

namespace Application.DTOs.Analysis;
public record BatchAnalysisDto(
    Dictionary<string, SymbolAnalysisDto> Data
);
public record SymbolAnalysisDto(
    IEnumerable<PriceCandleDto> ResultCandles,
    IEnumerable<PriceCandleDto> InvolvedCandles
);
public enum ComparisonOperatorDto
{
    LessThan = 0,
    GreaterThan = 1,
    LessThanOrEqual = 2,
    GreaterThanOrEqual = 3,
    Equal = 4
}
public enum AnalysableValuesDto
{
    MA50 = 0,
    MA200 = 1,
    Open = 2,
    High = 3,
    Low = 4,
    Close = 5,
    Volume = 6,
}
public record BatchAnalysisParamsDto(
    string sort,
    string symbols,
    BatchAnalysisConditionDto condition
);

public record BatchAnalysisConditionDto
(
    DateOnly fromDate,
    DateOnly toDate,
    IEnumerable<BatchAnalysisConditionBodyDto> rules
);
public record BatchAnalysisConditionBodyDto
(
    int firstValueDayOffset,
    int secondValueDayOffset,
    AnalysableValuesDto firstValue,
    AnalysableValuesDto secondValue,
    ComparisonOperatorDto comparisonOperator
);

public class BatchAnalysisMappingProfile : Profile
{
    public BatchAnalysisMappingProfile()
    {
        CreateMap<BatchAnalysisConditionDto, BatchAnalysisCondition>();

        CreateMap<BatchAnalysisConditionBodyDto, BatchAnalysisConditionBody>();

        CreateMap<AnalysableValuesDto, AnalysableValues>();
        CreateMap<ComparisonOperatorDto, ComparisonOperator>();
    }
}
public static class BatchAnalysisMapping
{
    public static readonly SortMappingDefinition<PriceCandleDto, PriceCandle> SortMapping = new()
    {
        Mappings =
        [
            new SortMapping(nameof(PriceCandleDto.Timestamp), nameof(PriceCandle.Timestamp)),
            new SortMapping(nameof(PriceCandleDto.Open), nameof(PriceCandle.Open)),
            new SortMapping(nameof(PriceCandleDto.Close), nameof(PriceCandle.Close)),
            new SortMapping(nameof(PriceCandleDto.High), nameof(PriceCandle.High)),
            new SortMapping(nameof(PriceCandleDto.Low), nameof(PriceCandle.Low)),
            new SortMapping(nameof(PriceCandleDto.Volume), nameof(PriceCandle.Volume)),
            new SortMapping(nameof(PriceCandleDto.MA50), nameof(PriceCandle.MA50)),
            new SortMapping(nameof(PriceCandleDto.MA200), nameof(PriceCandle.MA200)),
        ]
    };
}
public class BatchAnalysisParamsDtoValidator : AbstractValidator<BatchAnalysisParamsDto>
{
    public BatchAnalysisParamsDtoValidator()
    {
        RuleFor(x => x.symbols).NotEmpty().WithMessage("Symbols must be provided");
        RuleFor(x => x.condition).NotNull().WithMessage("Condition must be provided");
        RuleFor(x => x.condition.fromDate).LessThanOrEqualTo(x => x.condition.toDate).WithMessage("FromDate must be less than or equal to ToDate");
        RuleForEach(x => x.condition.rules).SetValidator(new BatchAnalysisConditionBodyDtoValidator());
    }
}

public class BatchAnalysisConditionBodyDtoValidator : AbstractValidator<BatchAnalysisConditionBodyDto>
{
    public BatchAnalysisConditionBodyDtoValidator()
    {
        RuleFor(x => x.firstValueDayOffset).LessThan(30).WithMessage("FirstValueDayOffset cannot be higher than 30");
        RuleFor(x => x.secondValueDayOffset).LessThan(30).WithMessage("SecondValueDayOffset cannot be higher than 30");
        RuleFor(x => x.comparisonOperator).IsInEnum().WithMessage("Invalid comparison operator");
        RuleFor(x => x.firstValue).IsInEnum().WithMessage("Invalid first value type");
        RuleFor(x => x.secondValue).IsInEnum().WithMessage("Invalid second value type");
    }
}

