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
            for (int elemIdx = 0; elemIdx < list.Count; elemIdx++)
            {
                var randomVal = randomizer.NextDouble();
                shuffledData.Add(elemIdx, randomVal);
            }
            return shuffledData.OrderBy(kvp => kvp.Value).Select(kvp => list[kvp.Key]).ToList();
        }
    }
}
