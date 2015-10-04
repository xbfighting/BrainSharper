namespace BrainSharper.Implementations.MathUtils.ImpurityMeasures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ShannonEntropy<T> : BaseImpurityMeasure<T>
    {
        public override double ImpurityValue(IList<int> elementsInGroupsCount)
        {
            var totalCount = (double)elementsInGroupsCount.Sum();
            var probabilities = elementsInGroupsCount.Where(count => count != 0).Select(count => count / totalCount);
            return probabilities.Aggregate(0.0, (acc, current) => acc + (-current * Math.Log(current, 2)));
        }
    }
}
