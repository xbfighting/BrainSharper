namespace BrainSharperTests.Implementations.Algorithms.RuleBasedSystems
{
    using System;
    using System.Linq;

    using BrainSharper.Abstract.Algorithms.RuleInduction.Heuristics;
    using BrainSharper.Abstract.Algorithms.RuleInduction.Processors;
    using BrainSharper.General.DataQuality;
    using BrainSharper.General.DataUtils;
    using BrainSharper.Implementations.Algorithms.RuleInduction;
    using BrainSharper.Implementations.Algorithms.RuleInduction.Heuristics;
    using BrainSharper.Implementations.Algorithms.RuleInduction.Processors;
    using BrainSharper.Implementations.MathUtils.ImpurityMeasures;

    using TestUtils;

    using NUnit.Framework;

    [TestFixture]
    public class Cn2AlgorithmTests
    {
        private readonly IComplexesIntersector<string> complexIntersector = new ComplexIntersectorWithSingleValue<string>();

        private readonly ImpurityBasedComplexQualityChecker<string> shannonEntropyQualityChecker =
            new ImpurityBasedComplexQualityChecker<string>(new ShannonEntropy<string>());
        private readonly ImpurityBasedComplexQualityChecker<string> giniImpurityQualityChecker =
            new ImpurityBasedComplexQualityChecker<string>(new GiniIndex<string>());
        private readonly IComplexQualityChecker laplacianSmoothingQualityChecker =
            new LaplacianSmoothingQualityChecker(3);
        private readonly IComplexStatisticalImportanceChecker chisquareImportanceChecker = new ChiSquareComplexStatisticalImportanceChecker<string>(0.01);

        [Test]
        public void TestAlgorithmPerformance_DiscretizedIrisDataSet()
        {
            // Given
            var randomizer = new Random();
            var splitter = new CrossValidator<string>();
            var testData = TestDataBuilder.ReadIrisDiscretizedData();
            var featureDomains = complexIntersector.PrepareFeatureDomains(
                testData,
                TestDataBuilder.DiscretizedIrisDependentFeatureName);

            //TODO: 111 !!! AAA use always indices WITHOUT NAMES - only causes troubles!!!
            //TODO: 111 !!! AAA add checking max value in complex quality heuristic- will save time!!!

            var subject = new Cn2Algorithm<string>(
                chisquareImportanceChecker,
                laplacianSmoothingQualityChecker,
                complexIntersector,
                5);

            // When
            var accuracies = splitter.CrossValidate(
               subject,
               new RuleInductionParams<string>(featureDomains), 
               new RulesPredictor<string>(), 
               new ConfusionMatrixBuilder<string>(),
               testData,
               TestDataBuilder.DiscretizedIrisDependentFeatureName,
               0.75,
               10);
            var averageAccuracies = accuracies.Select(acc => acc.Accuracy).Average();
            
            // Then
            Assert.GreaterOrEqual(averageAccuracies, 0.9);

        }

        [Test]
        public void TestAlgorithmPerformance_CongressVotingDataSet()
        {
            // Given
            var randomizer = new Random();
            var splitter = new CrossValidator<string>();
            var testData = TestDataBuilder.ReadCongressData();
            var featureDomains = complexIntersector.PrepareFeatureDomains(
                testData,
                TestDataBuilder.CongressDataDependentFeatureName);

            var subject = new Cn2Algorithm<string>(
                chisquareImportanceChecker,
                giniImpurityQualityChecker,
                complexIntersector,
                5);

            // When
            var accuracies = splitter.CrossValidate(
               subject,
               new RuleInductionParams<string>(featureDomains),
               new RulesPredictor<string>(),
               new ConfusionMatrixBuilder<string>(),
               testData,
               TestDataBuilder.CongressDataDependentFeatureName,
               0.75,
               10);
            var averageAccuracies = accuracies.Select(acc => acc.Accuracy).Average();

            // Then
            Assert.GreaterOrEqual(averageAccuracies, 0.85);
        }
    }
}
