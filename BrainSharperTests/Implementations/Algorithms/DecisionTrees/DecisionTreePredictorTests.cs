namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees
{
    using System;
    using System.Linq;

    using BrainSharper.Abstract.Algorithms.DecisionTrees;
    using BrainSharper.Abstract.MathUtils.ImpurityMeasures;
    using BrainSharper.General.DataQuality;
    using BrainSharper.General.DataUtils;
    using BrainSharper.Implementations.Algorithms.DecisionTrees;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
    using BrainSharper.Implementations.Data;
    using BrainSharper.Implementations.MathUtils.ImpurityMeasures;

    using NUnit.Framework;

    using TestUtils;

    [TestFixture]
    public class DecisionTreePredictorTests
    {
        private readonly IImpurityMeasure<string> shannonEntropy = new ShannonEntropy<string>();
        private readonly IDecisionTreeModelBuilder binaryTreeBuilder, multiValueTreeBuilder;
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
            // TODO: add support for numeric attributes!!!
            // Given
            var splitter = new CrossValidator<string>();
            var testData = TestDataBuilder.ReadAdultCensusDataFrame();

            var predictor = new DecisionTreePredictor<string>();

            // When
            var accuracies = splitter.CrossValidate(
                multiValueTreeBuilder,
                modelBuilderParams,
                predictor,
                new ConfusionMatrixBuilder<string>(),
                testData,
                "income",
                0.7,
                10);

            // Then
            var averageAccuracy = accuracies.Select(report => report.Accuracy).Average();
            Assert.IsTrue(averageAccuracy >= 0.9);
        }

        [Test]
        public void DiscreteClassification_CategoricalFeatures_MultiValuesSplits_CongressVotingData_CrossValidation()
        {
            // Given
            var randomizer = new Random(3);
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
                percetnagOfTrainData: 0.7, folds: 10);

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
    }
}
