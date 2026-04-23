using Application.DTOs.Analysis;

using Swashbuckle.AspNetCore.Filters;

namespace SORMAnalytics.Web.Swagger;

public class BatchAnalysisParamsDtoExample : IExamplesProvider<BatchAnalysisParamsDto>
{
    public BatchAnalysisParamsDto GetExamples()
    {
        return new BatchAnalysisParamsDto(
            sort: "timestamp",
            symbols: "AAPL,TSLA",
            condition: new BatchAnalysisConditionDto(
                fromDate: new DateOnly(2021, 1, 1),
                toDate: new DateOnly(2022, 10, 31),
                rules:
                [
                    new BatchAnalysisConditionBodyDto(
                        firstValueDayOffset: -1,
                        secondValueDayOffset: -1,
                        firstValue: AnalysableValuesDto.MA50,
                        secondValue: AnalysableValuesDto.MA200,
                        comparisonOperator: ComparisonOperatorDto.LessThanOrEqual),
                    new BatchAnalysisConditionBodyDto(
                        firstValueDayOffset: 0,
                        secondValueDayOffset: 0,
                        firstValue: AnalysableValuesDto.MA50,
                        secondValue: AnalysableValuesDto.MA200,
                        comparisonOperator: ComparisonOperatorDto.GreaterThan)
                ]
            )
        );
    }
}
