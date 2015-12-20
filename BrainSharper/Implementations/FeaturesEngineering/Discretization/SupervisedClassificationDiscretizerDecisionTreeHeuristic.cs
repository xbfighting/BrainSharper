using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.FeaturesEngineering;
using BrainSharper.Abstract.FeaturesEngineering.Discretization;

namespace BrainSharper.Implementations.FeaturesEngineering.Discretization
{
    public class SupervisedClassificationDiscretizerDecisionTreeHeuristic : ISupervisedClassificationDiscretizer
    {
        private readonly IBinaryNumericSplitPointSelectorCategoricalOutcome
            binaryNumericSplitPointSelectorCategoricalOutcome;

        private readonly ICategoricalSplitQualityChecker categoricalSplitQualityChecker;
        private readonly ILeafBuilder leafBuilder;
        private readonly IBinaryNumericDataSplitter numericDataSplitter;

        public SupervisedClassificationDiscretizerDecisionTreeHeuristic(
            IBinaryNumericDataSplitter numericDataSplitter,
            IBinaryNumericSplitPointSelectorCategoricalOutcome binaryNumericSplitPointSelectorCategoricalOutcome,
            ICategoricalSplitQualityChecker categoricalSplitQualityChecker,
            ILeafBuilder leafBuilder)
        {
            this.numericDataSplitter = numericDataSplitter;
            this.binaryNumericSplitPointSelectorCategoricalOutcome = binaryNumericSplitPointSelectorCategoricalOutcome;
            this.categoricalSplitQualityChecker = categoricalSplitQualityChecker;
            this.leafBuilder = leafBuilder;
        }

        public ISupervisedDiscretizationResult Discretize(
            IDataFrame dataFrame,
            string dependentFeatureName,
            string numericFeatureName,
            string newFeatureName)
        {
            var initialEntropy = categoricalSplitQualityChecker.GetInitialEntropy(dataFrame, dependentFeatureName);
            var ranges = DivideAndConquer(dataFrame, dependentFeatureName, numericFeatureName, initialEntropy)
                .OrderBy(rng => rng.RangeFrom);
            return new SupervisedClassificationResult(
                newFeatureName,
                ranges
                    .Select((rng, idx) => new {Index = idx, Range = rng})
                    .ToDictionary(elem => elem.Range as IRange, elem => $"{newFeatureName}_{elem.Index}"));
        }

        private IList<Range> DivideAndConquer(
            IDataFrame dataFrame,
            string dependentFeatureName,
            string numericFeatureName,
            double initialEntropy)
        {
            if (initialEntropy == 0)
            {
                return BuildRange(dataFrame, numericFeatureName);
            }
            var splitResults = binaryNumericSplitPointSelectorCategoricalOutcome.FindBestSplitPoint(
                dataFrame,
                dependentFeatureName,
                numericFeatureName,
                categoricalSplitQualityChecker,
                numericDataSplitter,
                initialEntropy);
            var binarySplitResult = splitResults.Item1 as IBinarySplittingResult;
            var splittedData = binarySplitResult.SplittedDataSets;
            if (SplitIsEmpty(binarySplitResult))
            {
                return BuildRange(dataFrame, numericFeatureName);
            }

            var results = new List<Range>();
            Range lastElem = null;
            foreach (var splittedFrame in splittedData)
            {
                var subset = splittedFrame.SplittedDataFrame;
                var subsetInitialEntropy = categoricalSplitQualityChecker.GetInitialEntropy(subset, dependentFeatureName);
                var ranges = DivideAndConquer(subset, dependentFeatureName, numericFeatureName, subsetInitialEntropy);
                if (lastElem != null)
                {
                    var firstFromCurrent = ranges.First();

                    var startOfLast = lastElem.RangeFrom;
                    var endOfCurrent = firstFromCurrent.RangeTo;
                    var middlePoint = (startOfLast + endOfCurrent)/2.0;
                    var newLastElem = new Range(lastElem.AttributeName, middlePoint, lastElem.RangeTo);
                    results.Remove(lastElem);
                    results.Add(newLastElem);

                    ranges.RemoveAt(0);
                    var newFirstCurrent = new Range(firstFromCurrent.AttributeName, firstFromCurrent.RangeFrom,
                        middlePoint);
                    ranges.Insert(0, newFirstCurrent);
                }
                lastElem = ranges.Last();
                results.AddRange(ranges);
            }
            return results;
        }

        private static IList<Range> BuildRange(IDataFrame dataFrame, string numericFeatureName)
        {
            var allValues = dataFrame.GetColumnVector<double>(numericFeatureName).Values;
            var min = allValues.Min();
            var max = allValues.Max();
            var range = new Range(numericFeatureName, min, max);
            return new List<Range> {range};
        }

        private static bool SplitIsEmpty(ISplittingResult splitResult)
        {
            return splitResult == null
                   || splitResult.SplittedDataSets.Any(
                       splitSet => splitSet?.SplittedDataFrame == null || splitSet.SplittedDataFrame.RowCount == 0);
        }
    }
}