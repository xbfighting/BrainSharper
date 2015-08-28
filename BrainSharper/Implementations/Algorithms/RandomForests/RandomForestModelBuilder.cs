namespace BrainSharper.Implementations.Algorithms.RandomForests
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;

    using Abstract.Algorithms.DecisionTrees;
    using Abstract.Algorithms.DecisionTrees.DataStructures;
    using Abstract.Algorithms.Infrastructure;
    using Abstract.Algorithms.RandomForests;
    using Abstract.Data;

    using BrainSharper.General.DataQuality;

    using MathNet.Numerics.Statistics;

    public class RandomForestModelBuilder<TPredictionVal> : IRandomForestModelBuilder
    {
        private readonly IDecisionTreeModelBuilder decisionTreeModelBuilder;
        private readonly Random randomizer;
        private readonly IDataQualityMeasure<TPredictionVal> dataQualityMeasure;
        private readonly IPredictor<TPredictionVal> decisionTreePredictor;
        private readonly Func<int, int> featuresToUseCountCalculator;
        private readonly Func<IDecisionTreeModelBuilderParams> decisionTreeModelBuilderParamsFactory; 

        public RandomForestModelBuilder(
            IDecisionTreeModelBuilder decisionTreeModelBuilder,
            IPredictor<TPredictionVal> treePredictor,
            IDataQualityMeasure<TPredictionVal> dataQualityMeasurer,
            Func<int, int> featuresToUseCountSelector,
            Func<IDecisionTreeModelBuilderParams> decisionTreePramsFactory,
            Random random = null)
        {
            this.decisionTreeModelBuilder = decisionTreeModelBuilder;
            decisionTreePredictor = treePredictor;
            dataQualityMeasure = dataQualityMeasurer;
            randomizer = random ?? new Random();
            featuresToUseCountCalculator = featuresToUseCountSelector;
            decisionTreeModelBuilderParamsFactory = decisionTreePramsFactory;
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams)
        {
            if (!(additionalParams is IRandomForestModelBuilderParams))
            {
                throw new ArgumentException("Invalid parameters passed to Random Forest Model builder!");
            }

            var randomForestParams = additionalParams as IRandomForestModelBuilderParams;

            var trees = new IDecisionTreeNode[randomForestParams.TreesCount];
            var oobErrors = new double[randomForestParams.TreesCount];

            var columnsCountToTake = featuresToUseCountCalculator(dataFrame.ColumnsCount - 1);
            var featureColumns = dataFrame.ColumnNames.Except(new[] { dependentFeatureName }).ToList();

            Parallel.For(
                0,
                randomForestParams.TreesCount,
                i =>
                    {
                        var randomlySelectedIndices =
                            Enumerable.Range(0, dataFrame.RowCount)
                                .Select(_ => randomizer.Next(0, dataFrame.RowCount))
                                .ToList();
                        var outOfBagIndices =
                            Enumerable.Range(0, dataFrame.RowCount).Except(randomlySelectedIndices).ToList();
                        var columnsToTake = featureColumns.OrderBy(_ => randomizer.Next()).Take(columnsCountToTake).ToList();
                        columnsToTake.Add(dependentFeatureName);

                        var baggedTestData = dataFrame.Slice(randomlySelectedIndices, columnsToTake);
                        var oobTestData = dataFrame.Slice(outOfBagIndices, columnsToTake);
                        var oobExpected = oobTestData.GetColumnVector<TPredictionVal>(dependentFeatureName).Values;

                        var decisionTree = decisionTreeModelBuilder.BuildModel(
                            baggedTestData,
                            dependentFeatureName,
                            decisionTreeModelBuilderParamsFactory());
                        var prediction = decisionTreePredictor.Predict(oobTestData, decisionTree, dependentFeatureName);

                        //TODO: AAA !!! Better calculate errors!!!
                        //TODO: AAA !!! Later on add support for calculating variable importance!!!
                        var dataQuality = dataQualityMeasure.CalculateError(oobExpected, prediction);
                        trees[i] = decisionTree as IDecisionTreeNode;
                        oobErrors[i] = dataQuality;
                    });

            return new RandomForestModel(trees, oobErrors);
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, int dependentFeatureIndex, IModelBuilderParams additionalParams)
        {
            if (!(additionalParams is IRandomForestModelBuilderParams))
            {
                throw new ArgumentException("Invalid parameters passed to Random Forest Model builder!");
            }

            return BuildModel(dataFrame, dataFrame.ColumnNames[dependentFeatureIndex], additionalParams);
        }
    }
}
