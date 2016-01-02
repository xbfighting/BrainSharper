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

    [TestFixture]
    public class ClassificationAprioriTest
    {
        private readonly IPredictor<string> _classifier = new AssociativeClassificationPredictor<string>();
        private readonly ICrossValidator<string> _splitter = new CrossValidator<string>();
        private readonly IPredictionModelBuilder _modelBuilder = new ClassificationApriori<string>(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet);



        [Test]
        public void BuildAssociativeModelTest()
        {
            // Given
            var testData = TestDataBuilder.ReadIrisDiscretizedData();
            var miningParams = new ClassificationAssociationMiningParams(
                TestDataBuilder.DiscretizedIrisDependentFeatureName,
                0.05,
                null,
                0.8);

            // When
            var accuracies = _splitter.CrossValidate(
                _modelBuilder,
                miningParams,
                _classifier,
                new ConfusionMatrixBuilder<string>(),
                testData,
                "iris",
                0.7,
                5);

            // Then
            var avgAccuracy = accuracies.Select(report => report.Accuracy).First();
            Assert.GreaterOrEqual(avgAccuracy, 0.9);
        }
    }
}
