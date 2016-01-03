using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification;
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

        [Test]
        public void BuildAssociativeModelTest_HeuristicsComparison()
        {
            // Given
            var modelBuilder1 = new ClassificationApriori<string>(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet, _basicHeuristic);
            var modelBuilder2= new ClassificationApriori<string>(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet, _accHeuristic);
            var testData = TestDataBuilder.ReadIrisDiscretizedData();
            var miningParams = new ClassificationAssociationMiningParams(
                TestDataBuilder.DiscretizedIrisDependentFeatureName,
                0.05,
                null,
                0.8);

            // When
            var model1 = modelBuilder1.BuildModel(testData, "iris", miningParams) as IAssociativeClassificationModel<string>;
            var model2 = modelBuilder2.BuildModel(testData, "iris", miningParams) as IAssociativeClassificationModel<string>;

            // Then
            CollectionAssert.AreNotEquivalent(model1.ClassificationRules, model2.ClassificationRules);
        }


        [Test]
        public void BuildAssociativeModelTest_BasicCARHeuristic()
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
                10);

            // Then
            var avgAccuracy = accuracies.Select(report => report.Accuracy).First();
            Assert.GreaterOrEqual(avgAccuracy, 0.9);
        }

        [Test]
        public void BuildAssociativeModelTest_AccHeuristic()
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
                10);

            // Then
            var avgAccuracy = accuracies.Select(report => report.Accuracy).First();
            Assert.GreaterOrEqual(avgAccuracy, 0.9);
        }
    }
}
