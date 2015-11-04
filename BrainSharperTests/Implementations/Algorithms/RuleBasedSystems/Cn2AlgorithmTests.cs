namespace BrainSharperTests.Implementations.Algorithms.RuleBasedSystems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using BrainSharper.Abstract.Algorithms.RuleInduction.Heuristics;
    using BrainSharper.Abstract.Algorithms.RuleInduction.Processors;
    using BrainSharper.General.DataQuality;
    using BrainSharper.General.DataUtils;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Helpers;
    using BrainSharper.Implementations.Algorithms.RuleInduction;
    using BrainSharper.Implementations.Algorithms.RuleInduction.Heuristics;
    using BrainSharper.Implementations.Algorithms.RuleInduction.Processors;
    using BrainSharper.Implementations.MathUtils.ImpurityMeasures;

    using BrainSharperTests.TestUtils;

    using NUnit.Framework;

    [TestFixture]
    public class Cn2AlgorithmTests
    {
        private readonly IComplexesIntersector<string> complexIntersector = new ComplexIntersectorWithSingleValue<string>();

        private readonly ImpurityBasedComplexQualityChecker<string> shannonEntropyQualityChecker =
            new ImpurityBasedComplexQualityChecker<string>(new ShannonEntropy<string>());
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
                shannonEntropyQualityChecker,
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

            // Then
            Assert.IsNotNull(accuracies);
        }
    }
}
