namespace BrainSharper.General.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MathNet.Numerics.Statistics;

    public static class CollectionUtils
    {
        public static IList<T> Shuffle<T>(this IList<T> list, Random randomizer = null)
        {
            randomizer = randomizer ?? new Random();
            var shuffledData = new Dictionary<int, double>();
            for (int elemIdx = 0; elemIdx < list.Count; elemIdx++)
            {
                var randomVal = randomizer.NextDouble();
                shuffledData.Add(elemIdx, randomVal);
            }
            return shuffledData.OrderBy(kvp => kvp.Value).Select(kvp => list[kvp.Key]).ToList();
        }

        public static IList<double> CumulativeSum(this IEnumerable<double> numericSequence)
        {
            var sum = 0.0;
            return numericSequence.Select(elem => (sum += elem)).ToList();
        }

        public static bool IsEquivalentTo<TValue>(this IEnumerable<TValue> col1, IEnumerable<TValue> col2)
        {
            if (col2 == null)
            {
                return false;
            }

            var lst1 = col1.ToList();
            var lst2 = col2.ToList();

            return lst1.Count() == lst2.Count() && !lst1.Except(lst2).Any();
        }
    }
}
