﻿namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees
{
    using System;
    using System.Linq;

    using BrainSharper.Abstract.Algorithms.DecisionTrees;
    using BrainSharper.Abstract.MathUtils.ImpurityMeasures;
    using BrainSharper.General.DataQuality;
    using BrainSharper.General.DataUtils;
    using BrainSharper.Implementations.Algorithms.DecisionTrees;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
    using BrainSharper.Implementations.MathUtils.ImpurityMeasures;

    using NUnit.Framework;

    using TestUtils;

    [TestFixture]
    public class DecisionTreePredictorTests
    {
        private readonly IImpurityMeasure<string> shannonEntropy = new ShannonEntropy<string>();
        private readonly IDecisionTreeModelBuilder binaryTreeBuilder, multiValueTreeBuilder;

        public DecisionTreePredictorTests()
        {
            this.binaryTreeBuilder = new BinaryDecisionTreeModelBuilder(
                new InformationGainRatioCalculator<string>(shannonEntropy, shannonEntropy as ICategoricalImpurityMeasure<string>),
                new BinarySplitSelector<string>(new BinaryDiscreteDataSplitter<string>(), new BinaryNumericDataSplitter()),
                new CategoricalDecisionTreeLeafBuilder());
            this.multiValueTreeBuilder = new MultiSplitDecisionTreeModelBuilder(
                new InformationGainRatioCalculator<string>(shannonEntropy, shannonEntropy as ICategoricalImpurityMeasure<string>),
                new MultiValueSplitSelectorForCategoricalOutcome<string>(new MultiValueDiscreteDataSplitter<string>(), new BinaryNumericDataSplitter()),
                new CategoricalDecisionTreeLeafBuilder());
        }

        [Test]
        public void DiscreteClassificationNumericFeaturesBinarySplitsIrisDataCrossValidation()
        {
            // Given
            var randomizer = new Random();
            var splitter = new CrossValidator<object>();
            var testData = TestDataBuilder.ReadIrisData();
            var predictor = new DecisionTreePredictor<object>();

            // When
            var accuracies = splitter.CrossValidate(
                this.binaryTreeBuilder,
                null,
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
        public void DiscreteClassificationNumericFeaturesMultiValuesSplitsAdultCensusDataCrossValidation()
        {
            // TODO: add support for numeric attributes!!!
            // Given
            var splitter = new CrossValidator<string>();
            var testData = TestDataBuilder.ReadAdultCensusDataFrame();
            var predictor = new DecisionTreePredictor<string>();

            // When
            var accuracies = splitter.CrossValidate(
                multiValueTreeBuilder,
                null,
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
        public void DiscreteClassificationCategoricalFeaturesMultiValuesSplitsCongressVotingDataCrossValidation()
        {
            // TODO: add support for numeric attributes!!!
            // Given
            var randomizer = new Random();
            var splitter = new CrossValidator<string>();
            var testData = TestDataBuilder.ReadCongressData();
            var predictor = new DecisionTreePredictor<string>();

            // When
            var accuracies = splitter.CrossValidate(
                this.multiValueTreeBuilder,
                null,
                predictor,
                new ConfusionMatrixBuilder<string>(),
                testData,
                "party",
                0.7,
                10);

            // Then
            var averageAccuracy = accuracies.Select(report => report.Accuracy).Average();
            Assert.IsTrue(averageAccuracy >= 0.9);
        }
    }
}
