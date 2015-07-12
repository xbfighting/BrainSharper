using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;

namespace BrainSharper.Implementations.MathUtils.ImpurityMeasures
{
    public abstract class BaseImpurityMeasure<T> : IImpurityMeasure<T>
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
            return CalculateImpurity(valuesCounts, values.Count);
        }

        internal abstract double CalculateImpurity(IDictionary<T, int> valuesCounts, double elementsCount);
    }
}
