using System;
using System.Linq;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Data;
using BrainSharper.General.Utils;
using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class MultiValueSplitSelectorForCategoricalOutcome<TValue> : IBestSplitSelector<TValue>
    {
        private readonly IDataSplitter<TValue> _categoricalDataSplitter;
        //TODO: implement numeric multi-value splitter
        //private readonly IDataSplitter<TValue> _binaryNumericDataSplitter;

        public MultiValueSplitSelectorForCategoricalOutcome(IDataSplitter<TValue> categoricalDataSplitter)
        {
            _categoricalDataSplitter = categoricalDataSplitter;
        }

        public ISplittingResult<TValue> SelectBestSplit(
            IDataFrame baseData, 
            string dependentFeatureName,
            ISplitQualityChecker<TValue> splitQualityChecker)
        {
            ISplittingResult<TValue> bestSplit = null;
            double bestSplitQuality = float.NegativeInfinity;
            double initialEntropy = splitQualityChecker.GetInitialEntropy(baseData, dependentFeatureName);
            int totalRowsCount = baseData.RowCount;

            foreach (var attributeToSplit in baseData.ColumnNames.Except(new[] {dependentFeatureName}))
            {
                if (baseData.GetColumnType(attributeToSplit).TypeIsNumeric())
                {
                    // TODO: handle numeric data in multi-splits
                }
                else
                {
                    var allPossibleValues = baseData.GetColumnVector<TValue>(attributeToSplit).Distinct();
                    foreach (var possibleValue in allPossibleValues)
                    {
                        var splitParam = new SplittingParams<TValue>(attributeToSplit);
                        var splittedData = _categoricalDataSplitter.SplitData(baseData, splitParam);
                        var splitQuality = splitQualityChecker.CalculateSplitQuality(initialEntropy, totalRowsCount,
                            splittedData, dependentFeatureName);
                        if (splitQuality > bestSplitQuality)
                        {
                            bestSplitQuality = splitQuality;
                            bestSplit = new SplittingResult<TValue>(false, attributeToSplit, splittedData);
                        }
                    }
                }
            }
            return bestSplit;
        }
    }
}
