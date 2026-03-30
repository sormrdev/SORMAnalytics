using System;
using System.Collections.Generic;
using System.Text;

using SORMAnalytics.Domain.Entities;

namespace SORMAnalytics.Domain.Interfaces;
public interface IBatchAnalyser
{
    public (IEnumerable<PriceCandle>?, IEnumerable<PriceCandle>?) Analyse(IEnumerable<PriceCandle> filteredPriceCandles, BatchAnalysisCondition condition);
}
