namespace SORMAnalytics.Domain.Entities;
public enum ComparisonOperator
{
    LessThan = 0,
    GreaterThan = 1,
    LessThanOrEqual = 2,
    GreaterThanOrEqual = 3,
    Equal = 4
}
public enum AnalysableValues
{
    MA50 = 0,
    MA200 = 1,
    Open = 2,
    High = 3,
    Low = 4,
    Close = 5,
    Volume = 6,
}
public record BatchAnalysisCondition
(
    DateOnly fromDate,
    DateOnly toDate,
    IEnumerable<BatchAnalysisConditionBody> rules
);
public record BatchAnalysisConditionBody
(
    int firstValueDayOffset,
    int secondValueDayOffset,
    AnalysableValues firstValue,
    AnalysableValues secondValue,
    ComparisonOperator comparisonOperator
);