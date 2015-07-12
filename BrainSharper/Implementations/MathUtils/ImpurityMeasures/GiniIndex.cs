using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainSharper.Implementations.MathUtils.ImpurityMeasures
{
    public class GiniIndex<T> : BaseImpurityMeasure<T>
    {
        internal override double CalculateImpurity(IDictionary<T, int> valuesCounts, double elementsCount)
        {
            var probabilitiesSum = valuesCounts.Select(valCount => Math.Pow(valCount.Value / elementsCount, 2)).Sum();
            return 1 - probabilitiesSum;
        }
    }
}
