using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainSharper.Implementations.MathUtils.ImpurityMeasures
{
    public class GiniIndex<T> : BaseImpurityMeasure<T>
    {
        public override double ImpurityValue(IList<int> elementsInGroupsCount)
        {
            var totalCount = (double)elementsInGroupsCount.Sum();
            var probabilitiesSum = elementsInGroupsCount.Select(count => Math.Pow(count / totalCount, 2)).Sum();
            return 1 - probabilitiesSum;
        }
    }
}
