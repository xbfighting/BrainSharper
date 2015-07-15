using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainSharper.Implementations.MathUtils.ImpurityMeasures
{
    public class ShannonEntropy<T> : BaseImpurityMeasure<T>
    {
        public override double ImpurityValue(IList<int> elementsInGroupsCount)
        {
            var totalCount = (double)elementsInGroupsCount.Sum();
            var probabilities = elementsInGroupsCount.Select(count => count / totalCount);
            return probabilities.Aggregate(0.0, (acc, current) => acc + (-current * Math.Log(current, 2)));
        }
    }
}
