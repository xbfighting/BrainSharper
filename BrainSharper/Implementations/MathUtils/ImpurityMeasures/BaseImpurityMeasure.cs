using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;

namespace BrainSharper.Implementations.MathUtils.ImpurityMeasures
{
    public abstract class BaseImpurityMeasure<T> : ICategoricalImpurityMeasure<T>
    {
        public double ImpurityValue(IList<T> values)
        {
            var valuesCounts = new Dictionary<T, int>();
            foreach (var val in values)
            {
                if (valuesCounts.ContainsKey(val))
                {
                    valuesCounts[val] += 1;
                }
                else
                {
                    valuesCounts[val] = 1;
                }
            }
            return ImpurityValue(valuesCounts.Values.ToList());
        }

        public abstract double ImpurityValue(IList<int> elementsInGroupsCount);
    }
}