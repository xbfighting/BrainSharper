using System;
using System.Data;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees.Processors
{
    public class BinaryNumericDataSplitter : BinaryDiscreteDataSplitter<double>, IBinaryNumericDataSplitter
    {
        protected override Predicate<DataRow> BuildSplittingFunction(string splittingFeatureName, double splittingFeatureValue)
        {
            Predicate<DataRow> rowFilter =
                row => {
                           double rowValue = Convert.ToDouble(row[splittingFeatureName]);
                           return rowValue >= splittingFeatureValue;
                };
            return rowFilter;
        }
    }
}
