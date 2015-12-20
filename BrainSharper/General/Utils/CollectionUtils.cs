using System;
using System.Collections.Generic;
using System.Linq;

namespace BrainSharper.General.Utils
{
    public static class CollectionUtils
    {
        public static IList<T> Shuffle<T>(this IList<T> list, Random randomizer = null)
        {
            randomizer = randomizer ?? new Random();
            var shuffledData = new Dictionary<int, double>();
            for (var elemIdx = 0; elemIdx < list.Count; elemIdx++)
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

        public static IEnumerable<IEnumerable<T>> GenerateAllCombinations<T>(this IEnumerable<T> originalCollection)
        {
            var source = originalCollection.ToList();
            var subsets = new List<IList<T>>();
            for (var i = 0; i < source.Count; i++)
            {
                var element = source[i];
                if (subsets.Any())
                {
                    var initialSubsetsCount = subsets.Count;
                    for (var subsetIdx = 0; subsetIdx < initialSubsetsCount; subsetIdx++)
                    {
                        var existingSubset = subsets[subsetIdx];
                        var newSubset = new List<T>(existingSubset) {element};
                        subsets.Add(newSubset);
                    }
                }
                subsets.Add(new[] {element});
            }
            return subsets;
        }

        public static IEnumerable<IEnumerable<T>> GenerateCombinationsOfSizeK<T>(
            this IEnumerable<T> originalCollection,
            int combinationSize)
        {
            var elements = originalCollection.ToList();
            return combinationSize == 0
                ? new[] {new T[0]}
                : elements.SelectMany((e, i) =>
                    elements.Skip(i + 1)
                        .GenerateCombinationsOfSizeK(combinationSize - 1)
                        .Select(c => (new[] {e}).Concat(c)));
        }
    }
}