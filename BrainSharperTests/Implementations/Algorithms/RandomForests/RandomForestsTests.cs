namespace BrainSharperTests.Implementations.Algorithms.RandomForests
{
    using System;
    using System.Linq;

    using BrainSharper.Abstract.Algorithms.DecisionTrees;
    using BrainSharper.Abstract.MathUtils.ImpurityMeasures;
    using BrainSharper.General.DataQuality;
    using BrainSharper.General.DataUtils;
    using BrainSharper.Implementations.Algorithms.DecisionTrees;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
    using BrainSharper.Implementations.Algorithms.RandomForests;
    using BrainSharper.Implementations.MathUtils.ImpurityMeasures;

    using BrainSharperTests.TestUtils;

    using NUnit.Framework;

    [TestFixture]
    public class RandomForestsTests
    {
        private readonly IImpurityMeasure<string> shannonEntropy = new ShannonEntropy<string>();
        private readonly IDecisionTreeModelBuilder binaryTreeBuilder, multiValueTreeBuilder, multiValueTreeBuilderWithBetterNumercValsHandler;
        private readonly IDecisionTreeModelBuilderParams modelBuilderParams = new DecisionTreeModelBuilderParams(false);

        public RandomForestsTests()
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
        public void DiscreteClassification_NumericFeatures_MultiValuesSplits_AdultCensusData()
        {
            // Given
            var randomForestBuilder = new RandomForestModelBuilder<object>(
                multiValueTreeBuilderWithBetterNumercValsHandler,
                new DecisionTreePredictor<object>(),
                new ConfusionMatrixBuilder<object>(),
                i => (int)Math.Round(Math.Sqrt(i), MidpointRounding.AwayFromZero),
                () => new DecisionTreeModelBuilderParams(false));
            var randomForestPredictor = new RandomForestPredictor<object>(new DecisionTreePredictor<object>(), true);
            var testData = TestDataBuilder.ReadAdultCensusDataFrame();
            var crossValidator = new CrossValidator<object>();


            // When
            var accuracy = crossValidator.CrossValidate(
                randomForestBuilder,
                new RandomForestParams(10, 100),
                randomForestPredictor,
                new ConfusionMatrixBuilder<object>(),
                testData,
                "income",
                0.8,
                1).First();

            // Then
            Assert.IsTrue(accuracy.Accuracy >= 0.8);
        }

        [Test]
        public void DiscreteClassification_DiscreteFeatures_MultiValuesSplits_CongressVoting()
        {
            // Given
            var randomForestBuilder = new RandomForestModelBuilder<object>(
                multiValueTreeBuilderWithBetterNumercValsHandler,
                new DecisionTreePredictor<object>(),
                new ConfusionMatrixBuilder<object>(),
                i => (int)Math.Round(Math.Sqrt(i), MidpointRounding.AwayFromZero),
                () => new DecisionTreeModelBuilderParams(false));
            var randomForestPredictor = new RandomForestPredictor<object>(new DecisionTreePredictor<object>(), true);
            var testData = TestDataBuilder.ReadCongressData();
            var crossValidator = new CrossValidator<object>();


            // When
            var accuracy = crossValidator.CrossValidate(
                randomForestBuilder,
                new RandomForestParams(100, 100),
                randomForestPredictor,
                new ConfusionMatrixBuilder<object>(),
                testData,
                "party",
                0.7,
                1).First();

            // Then
            Assert.IsTrue(accuracy.Accuracy >= 0.9);
        }
    }
}
