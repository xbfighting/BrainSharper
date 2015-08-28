namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;

    using BrainSharper.Abstract.Algorithms.DecisionTrees;
    using BrainSharper.Abstract.Algorithms.DecisionTrees.Helpers;
    using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
    using BrainSharper.Abstract.MathUtils.ImpurityMeasures;
    using BrainSharper.General.DataQuality;
    using BrainSharper.General.DataUtils;
    using BrainSharper.Implementations.Algorithms.DecisionTrees;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Helpers;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
    using BrainSharper.Implementations.Algorithms.LinearRegression;
    using BrainSharper.Implementations.Data;
    using BrainSharper.Implementations.MathUtils.ImpurityMeasures;

    using MathNet.Numerics.LinearRegression;

    using NUnit.Framework;

    using TestUtils;

    [TestFixture]
    public class DecisionTreePredictorTests
    {
        private readonly IImpurityMeasure<string> shannonEntropy = new ShannonEntropy<string>();
        private readonly IDecisionTreeModelBuilder binaryTreeBuilder, multiValueTreeBuilder, multiValueTreeBuilderWithBetterNumercValsHandler;
        private readonly IDecisionTreeModelBuilderParams modelBuilderParams = new DecisionTreeModelBuilderParams(false);

        public DecisionTreePredictorTests()
        {
            binaryTreeBuilder = new BinaryDecisionTreeModelBuilder(
                new InformationGainRatioCalculator<string>(shannonEntropy, shannonEntropy as ICategoricalImpurityMeasure<string>),
                new BinarySplitSelectorForCategoricalOutcome(new BinaryDiscreteDataSplitter(), new BinaryNumericDataSplitter(), new ClassBreakpointsNumericSplitFinder()),
                new CategoricalDecisionTreeLeafBuilder());
            multiValueTreeBuilder = new MultiSplitDecisionTreeModelBuilder(
                new InformationGainRatioCalculator<string>(shannonEntropy, shannonEntropy as ICategoricalImpurityMeasure<string>),
                new MultiValueSplitSelectorForCategoricalOutcome(new MultiValueDiscreteDataSplitter(), new BinaryNumericDataSplitter(), new ClassBreakpointsNumericSplitFinder()),
                new CategoricalDecisionTreeLeafBuilder());
            multiValueTreeBuilderWithBetterNumercValsHandler = new MultiSplitDecisionTreeModelBuilder(
                new InformationGainRatioCalculator<string>(shannonEntropy, shannonEntropy as ICategoricalImpurityMeasure<string>),
                new MultiValueSplitSelectorForCategoricalOutcome(new MultiValueDiscreteDataSplitter(), new BinaryNumericDataSplitter(), new DynamicProgrammingNumericSplitFinder()),
                new CategoricalDecisionTreeLeafBuilder());
        }

        [Test]
        public void DiscreteClassification_NumericFeatures_BinarySplits_IrisData_CrossValidation()
        {
            // Given
            var randomizer = new Random();
            var splitter = new CrossValidator<object>();
            var testData = TestDataBuilder.ReadIrisData();
            var predictor = new DecisionTreePredictor<object>();

            // When
            var accuracies = splitter.CrossValidate(
                binaryTreeBuilder,
                modelBuilderParams,
                predictor,
                new ConfusionMatrixBuilder<object>(),
                testData,
                "iris_class",
                0.7,
                10);

            // Then
            var averageAccuracy = accuracies.Select(report => report.Accuracy).Average();
            Assert.IsTrue(averageAccuracy >= 0.9);
        }

        [Test]
        public void DiscreteClassification_NumericFeatures_MultiValuesSplits_AdultCensusData_CrossValidation()
        {
            // Given
            var splitter = new CrossValidator<object>();
            var testData = TestDataBuilder.ReadAdultCensusDataFrame();

            var predictor = new DecisionTreePredictor<object>();

            // When
            var accuracies = splitter.CrossValidate(
                multiValueTreeBuilderWithBetterNumercValsHandler,
                modelBuilderParams,
                predictor,
                new ConfusionMatrixBuilder<object>(),
                testData,
                "income",
                0.7,
                5);

            // Then
            var averageAccuracy = accuracies.Select(report => report.Accuracy).Average();
            Assert.IsTrue(averageAccuracy >= 0.8);
        }

        [Test]
        public void DiscreteClassification_CategoricalFeatures_MultiValuesSplits_CongressVotingData_CrossValidation()
        {
            // Given
            var randomizer = new Random();
            var splitter = new CrossValidator<string>(randomizer);
            var testData = TestDataBuilder.ReadCongressData() as DataFrame;

            var predictor = new DecisionTreePredictor<string>();

            // When
            var accuracies = splitter.CrossValidate(modelBuilder: this.multiValueTreeBuilder, modelBuilderParams: modelBuilderParams, predictor: predictor, qualityMeasure: new ConfusionMatrixBuilder<string>(), dataFrame: testData, dependentFeatureName: "party", percetnagOfTrainData: 0.7, folds: 10);

            // Then
            var averageAccuracy = accuracies.Select(report => report.Accuracy).Average();
            Assert.IsTrue(averageAccuracy >= 0.9);
        }

        [Test]
        public void DiscreteClassification_CategoricalFeatures_MultiValuesSplits_CongressVotingData_StatisticalSignificanceHeuristic_CrossValidation()
        {
            // Given
            var randomizer = new Random();
            var splitter = new CrossValidator<string>(randomizer);
            var testData = TestDataBuilder.ReadCongressData() as DataFrame;

            var predictor = new DecisionTreePredictor<string>();

            // When
            var accuracies = splitter.CrossValidate(
                modelBuilder: this.BuildCustomModelBuilder(true, statisticalSignificanceChecker: new ChiSquareStatisticalSignificanceChecker(0.05)), 
                modelBuilderParams: new DecisionTreeModelBuilderParams(false, true), 
                predictor: predictor, 
                qualityMeasure: new ConfusionMatrixBuilder<string>(),
                dataFrame: testData, 
                dependentFeatureName: "party", 
                percetnagOfTrainData: 0.7, 
                folds: 10);

            // Then
            var averageAccuracy = accuracies.Select(report => report.Accuracy).Average();
            Assert.IsTrue(averageAccuracy >= 0.9);
        }

        [Test]
        public void DiscreteClassification_CategoricalFeatures_BinarySplits_ConvressVotingData_CrossValidation()
        {
            // Given
            var randomizer = new Random(3);
            var splitter = new CrossValidator<string>(randomizer);
            var testData = TestDataBuilder.ReadCongressData() as DataFrame;

            var predictor = new DecisionTreePredictor<string>();

            // When
            var accuracies = splitter.CrossValidate(
                modelBuilder: binaryTreeBuilder, 
                modelBuilderParams: modelBuilderParams, 
                predictor: predictor, 
                qualityMeasure: new ConfusionMatrixBuilder<string>(), 
                dataFrame: testData, 
                dependentFeatureName: "party", 
                percetnagOfTrainData: 0.7, 
                folds: 10);

            // Then
            var averageAccuracy = accuracies.Select(report => report.Accuracy).Average();
            Assert.IsTrue(averageAccuracy >= 0.9);
        }

        [Test]
        public void DiscreteClassification_CategoricalFeatures_BinarySplits_ConvressVotingData_StatisticalSignificanceTest_CrossValidation()
        {
            // Given
            var randomizer = new Random(3);
            var splitter = new CrossValidator<string>(randomizer);
            var testData = TestDataBuilder.ReadCongressData() as DataFrame;

            var predictor = new DecisionTreePredictor<string>();

            // When
            var accuracies = splitter.CrossValidate(
                modelBuilder: this.BuildCustomModelBuilder(true, statisticalSignificanceChecker: new ChiSquareStatisticalSignificanceChecker()),
                modelBuilderParams: new DecisionTreeModelBuilderParams(false, true), 
                predictor: predictor,
                qualityMeasure: new ConfusionMatrixBuilder<string>(),
                dataFrame: testData,
                dependentFeatureName: "party",
                percetnagOfTrainData: 0.7,
                folds: 10);

            // Then
            var averageAccuracy = accuracies.Select(report => report.Accuracy).Average();
            Assert.IsTrue(averageAccuracy >= 0.9);
        }

        [Test]
        public void Mushroom_MultiSplit()
        {
            // Given
            var randomizer = new Random(3);
            var splitter = new CrossValidator<string>(randomizer);
            var testData = TestDataBuilder.ReadMushroomDataWithCategoricalAttributes();

            var predictor = new DecisionTreePredictor<string>();

            // When
            var accuracies = splitter.CrossValidate(
                modelBuilder: multiValueTreeBuilder, 
                modelBuilderParams: modelBuilderParams, 
                predictor: predictor, 
                qualityMeasure: new ConfusionMatrixBuilder<string>(), 
                dataFrame: testData, 
                dependentFeatureName: "type", 
                percetnagOfTrainData: 0.7, 
                folds: 2);

            // Then
            var averageAccuracy = accuracies.Select(report => report.Accuracy).Average();
            Assert.IsTrue(averageAccuracy >= 0.99);
        }

        [Test]
        public void Mushroom_MultiSplit_StatisticalSignificanceHeuristic()
        {
            // Given
            var randomizer = new Random(3);
            var splitter = new CrossValidator<string>(randomizer);
            var testData = TestDataBuilder.ReadMushroomDataWithCategoricalAttributes();

            var predictor = new DecisionTreePredictor<string>();

            // When
            var accuracies = splitter.CrossValidate(
                modelBuilder: this.BuildCustomModelBuilder(statisticalSignificanceChecker: new ChiSquareStatisticalSignificanceChecker()),
                modelBuilderParams: new DecisionTreeModelBuilderParams(false, true), 
                predictor: predictor,
                qualityMeasure: new ConfusionMatrixBuilder<string>(),
                dataFrame: testData,
                dependentFeatureName: "type",
                percetnagOfTrainData: 0.7,
                folds: 2);

            // Then
            var averageAccuracy = accuracies.Select(report => report.Accuracy).Average();
            Assert.IsTrue(averageAccuracy >= 0.99);
        }

        [Test]
        public void Mushroom_BinarySplit()
        {
            // Given
            var randomizer = new Random(3);
            var splitter = new CrossValidator<string>(randomizer);
            var testData = TestDataBuilder.ReadMushroomDataWithCategoricalAttributes();

            var predictor = new DecisionTreePredictor<string>();

            // When
            var accuracies = splitter.CrossValidate(
                modelBuilder: binaryTreeBuilder,
                modelBuilderParams: modelBuilderParams,
                predictor: predictor,
                qualityMeasure: new ConfusionMatrixBuilder<string>(),
                dataFrame: testData,
                dependentFeatureName: "type",
                percetnagOfTrainData: 0.7,
                folds: 2);

            // Then
            var averageAccuracy = accuracies.Select(report => report.Accuracy).Average();
            Assert.IsTrue(averageAccuracy >= 0.99);
        }

        [Test]
        public void Regression_NumericAttrsAndOutcomesOnly_RegularizedGradientDescent()
        {
            // Given
            var randomizer = new Random(3);
            var splitter = new CrossValidator<double>(randomizer);
            var testData = TestDataBuilder.ReadHousingDataNormalizedAttrs();

            var predictor = new DecisionTreePredictor<double>();

            var numericTreeBuilder = new BinaryDecisionTreeModelBuilder(
                new VarianceBasedSplitQualityChecker(),
                new BestSplitSelectorForNumericValues(new BinaryNumericDataSplitter()),
                new RegressionAndModelDecisionTreeLeafBuilder(new RegularizedGradientDescentModelBuilder(0, 1, iterCount: 50, regularizationVal: 0.005), 0.05));
            // When
            var accuracies = splitter.CrossValidate(
                modelBuilder: numericTreeBuilder,
                modelBuilderParams: modelBuilderParams,
                predictor: predictor,
                qualityMeasure: new GoodnessOfFitQualityMeasure(),
                dataFrame: testData,
                dependentFeatureName: "MEDV",
                percetnagOfTrainData: 0.8,
                folds: 15);

            // Then
            var averageAccuracy = accuracies.Select(report => report.Accuracy).Average();
            var averageError =
                accuracies.Select(report => Math.Abs((report as IRegressionQualityMeasure).ErrorRate)).Sum();
            Assert.AreEqual(0.9, averageAccuracy);
        }

        [Test]
        public void Regression_NumericAttrsAndOutcomesOnly_RegularizedRegression()
        {
            // Given
            var randomizer = new Random(3);
            var splitter = new CrossValidator<double>(randomizer);
            var testData = TestDataBuilder.ReadHousingDataNormalizedAttrs();

            var predictor = new DecisionTreePredictor<double>();

            var numericTreeBuilder = new BinaryDecisionTreeModelBuilder(
                new VarianceBasedSplitQualityChecker(),
                new BestSplitSelectorForNumericValues(new BinaryNumericDataSplitter()),
                new RegressionAndModelDecisionTreeLeafBuilder(new RegularizedLinearRegressionModelBuilder(0.005)));

            // When
            var accuracies = splitter.CrossValidate(
                modelBuilder: numericTreeBuilder,
                modelBuilderParams: modelBuilderParams,
                predictor: predictor,
                qualityMeasure: new GoodnessOfFitQualityMeasure(),
                dataFrame: testData,
                dependentFeatureName: "MEDV",
                percetnagOfTrainData: 0.7,
                folds: 15);

            // Then
            var averegeRsquared = accuracies.Select(report => report.Accuracy).Average();
            Assert.IsTrue(averegeRsquared >= 0.6);
        }

        private IDecisionTreeModelBuilder BuildCustomModelBuilder(
            bool binary = false, 
            ISplitQualityChecker splitQualityChecker = null,
            IBestSplitSelector bestSplitSelector = null,
            ILeafBuilder leafBuilder = null,
            IStatisticalSignificanceChecker statisticalSignificanceChecker = null)
        {
            if (binary)
            {
                return new BinaryDecisionTreeModelBuilder(
                    splitQualityChecker ?? new InformationGainRatioCalculator<string>(shannonEntropy, shannonEntropy as ICategoricalImpurityMeasure<string>),
                    bestSplitSelector as IBinaryBestSplitSelector ?? new BinarySplitSelectorForCategoricalOutcome(new BinaryDiscreteDataSplitter(), new BinaryNumericDataSplitter(), new ClassBreakpointsNumericSplitFinder()),
                    leafBuilder ?? new CategoricalDecisionTreeLeafBuilder(),
                    statisticalSignificanceChecker);
            }

            return new MultiSplitDecisionTreeModelBuilder(
               splitQualityChecker ?? new InformationGainRatioCalculator<string>(shannonEntropy, shannonEntropy as ICategoricalImpurityMeasure<string>),
               bestSplitSelector ?? new MultiValueSplitSelectorForCategoricalOutcome(new MultiValueDiscreteDataSplitter(), new BinaryNumericDataSplitter(), new DynamicProgrammingNumericSplitFinder()),
               leafBuilder ?? new CategoricalDecisionTreeLeafBuilder(),
               statisticalSignificanceChecker);

        }
    }
}