using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainSharper.Implementations.MathUtils.ImpurityMeasures
{
    public class ShannonEntropy<T> : BaseImpurityMeasure<T>
    {
        internal override double CalculateImpurity(IDictionary<T, int> valuesCounts, double elementsCount)
        {
            var probabilities = valuesCounts.Select(valCount => valCount.Value / elementsCount);
            return probabilities.Aggregate(0.0, (acc, current) => acc + (-current * Math.Log(current, 2)));
        }
    }
}
