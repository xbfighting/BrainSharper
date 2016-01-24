using System.Collections.Generic;
using System.Linq;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification;
using BrainSharper.Implementations.Algorithms.DecisionTrees.Helpers;
using BrainSharperTests.TestUtils;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification
{
    using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
    using BrainSharper.Abstract.Algorithms.Infrastructure;
    using BrainSharper.General.DataQuality;
    using BrainSharper.General.DataUtils;
    using BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.Heuristics;

    [TestFixture]
    public class ClassificationAprioriTest
    {
        private readonly IPredictor<string> _classifier = new AssociativeClassificationPredictor<string>();
        private readonly ICrossValidator<string> _splitter = new CrossValidator<string>();

        private readonly ClassificationAprioriRulesSelector<string> _basicHeuristic = new BasicCARRulesSelector<string>();
        private readonly ClassificationAprioriRulesSelector<string> _accHeuristic = new AccuracyBasedRulesSelector<string>();
        private readonly ClassificationAprioriRulesSelector<string> _statisticalSignificanceHeuristic = new StatisticalSignificanceRulesSelector<string>(new ChiSquareStatisticalSignificanceChecker());
        
        [Test]
        public void BuildAssociativeModelTest_HeuristicsComparison_IrisData()
        {
            // Given
            var modelBuilder1 = new ClassificationApriori<string>(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet, _basicHeuristic);
            var modelBuilder2 = new ClassificationApriori<string>(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet, _accHeuristic);
            var modelBuilder3 = new ClassificationApriori<string>(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet, _statisticalSignificanceHeuristic);
            var testData = TestDataBuilder.ReadIrisDiscretizedData();
            var miningParams = new ClassificationAssociationMiningParams(
                TestDataBuilder.DiscretizedIrisDependentFeatureName,
                0.05,
                null,
                0.8);

            // When
            var model1 = modelBuilder1.BuildModel(testData, "iris", miningParams) as IAssociativeClassificationModel<string>;
            var model2 = modelBuilder2.BuildModel(testData, "iris", miningParams) as IAssociativeClassificationModel<string>;
            var model3 =
                modelBuilder3.BuildModel(testData, "iris", miningParams) as IAssociativeClassificationModel<string>;

            // Then
            CollectionAssert.AreNotEquivalent(model1.ClassificationRules, model2.ClassificationRules);
            CollectionAssert.AreNotEquivalent(model3.ClassificationRules, model1.ClassificationRules);
            CollectionAssert.AreNotEquivalent(model3.ClassificationRules, model2.ClassificationRules);
        }


        [Test]
        public void BuildAssociativeModelTest_BasicCARHeuristic_IrisData()
        {
            // Given
            var modelBuilder = new ClassificationApriori<string>(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet, _basicHeuristic);
            var testData = TestDataBuilder.ReadIrisDiscretizedData();
            var miningParams = new ClassificationAssociationMiningParams(
                TestDataBuilder.DiscretizedIrisDependentFeatureName,
                0.05,
                null,
                0.8);

            // When
            var accuracies = _splitter.CrossValidate(
                modelBuilder,
                miningParams,
                _classifier,
                new ConfusionMatrixBuilder<string>(),
                testData,
                "iris",
                0.7,
                20);

            // Then
            var avgAccuracy = accuracies.Select(report => report.Accuracy).First();
            Assert.GreaterOrEqual(avgAccuracy, 0.9);
        }

        [Test]
        public void BuildAssociativeModelTest_BasicCARHeuristic_CongressData()
        {
            // Given
            var modelBuilder = new ClassificationApriori<string>(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet, _basicHeuristic);
            var testData = TestDataBuilder.ReadCongressData();
            var miningParams = new ClassificationAssociationMiningParams(
                TestDataBuilder.CongressDataDependentFeatureName,
                0.13,
                null,
                0.8);

            // When
            var accuracies = _splitter.CrossValidate(
                modelBuilder,
                miningParams,
                _classifier,
                new ConfusionMatrixBuilder<string>(),
                testData,
                TestDataBuilder.CongressDataDependentFeatureName,
                0.7,
                20);

            // Then
            var avgAccuracy = accuracies.Select(report => report.Accuracy).First();
            Assert.GreaterOrEqual(avgAccuracy, 0.9);
        }


        [Test]
        public void BuildAssociativeModelTest_AccHeuristic_IrisData()
        {
            // Given
            var modelBuilder = new ClassificationApriori<string>(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet, _accHeuristic);
            var testData = TestDataBuilder.ReadIrisDiscretizedData();
            var miningParams = new ClassificationAssociationMiningParams(
                TestDataBuilder.DiscretizedIrisDependentFeatureName,
                0.05,
                null,
                0.8);

            // When
            var accuracies = _splitter.CrossValidate(
                modelBuilder,
                miningParams,
                _classifier,
                new ConfusionMatrixBuilder<string>(),
                testData,
                "iris",
                0.7,
                20);

            // Then
            var avgAccuracy = accuracies.Select(report => report.Accuracy).First();
            Assert.GreaterOrEqual(avgAccuracy, 0.9);
        }

        [Test]
        public void BuildAssociativeModelTest_StatisticalSignificanceHeuristic_IrisData()
        {
            // Given
            var modelBuilder = new ClassificationApriori<string>(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet, _statisticalSignificanceHeuristic);
            var testData = TestDataBuilder.ReadIrisDiscretizedData();
            var miningParams = new ClassificationAssociationMiningParams(
                TestDataBuilder.DiscretizedIrisDependentFeatureName,
                0.05,
                null,
                0.8);

            // When
            var accuracies = _splitter.CrossValidate(
                modelBuilder,
                miningParams,
                _classifier,
                new ConfusionMatrixBuilder<string>(),
                testData,
                "iris",
                0.7,
                20);

            // Then
            var avgAccuracy = accuracies.Select(report => report.Accuracy).First();
            Assert.GreaterOrEqual(avgAccuracy, 0.9);
        }
    }
}
