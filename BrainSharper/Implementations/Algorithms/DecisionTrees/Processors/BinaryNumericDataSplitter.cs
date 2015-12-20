using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class BinaryNumericDataSplitter : BinaryDiscreteDataSplitter, IBinaryNumericDataSplitter
    {
        protected override Predicate<DataRow> BuildSplittingFunction(string splittingFeatureName,
            object splittingFeatureValue)
        {
            Predicate<DataRow> rowFilter = row =>
            {
                var rowValue = Convert.ToDouble(row[splittingFeatureName]);
                return rowValue >= (double) splittingFeatureValue;
            };
            return rowFilter;
        }

        protected override Dictionary<bool, string> BuildQueries(string splittingFeatureName,
            object splittingFeatureValue)
        {
            var sanitizedValues = Convert.ToDouble(splittingFeatureValue).ToString("F", CultureInfo.InvariantCulture);
            return new Dictionary<bool, string>
            {
                [true] = $"[{splittingFeatureName}] >= {sanitizedValues}",
                [false] = $"[{splittingFeatureName}] < {sanitizedValues}"
            };
        }
    }
}