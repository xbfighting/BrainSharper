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
    [TestFixture]
    public class ClassificationAprioriTest
    {
        [Test]
        public void BuildAssociativeModelTest()
        {
            // Given
            var testData = TestDataBuilder.ReadIrisDiscretizedData();
            var model = new ClassificationApriori<string>(AssociationMiningParamsInterpreter.AreMinimalRequirementsMet);
            var miningParams = new ClassificationAssociationMiningParams(
                TestDataBuilder.DiscretizedIrisDependentFeatureName,
                0.15,
                null,
                0.8);

            // When
            var classifAssocModel = model.BuildModel(
                testData,
                TestDataBuilder.DiscretizedIrisDependentFeatureName,
                miningParams);

            // Then
            Assert.IsNotNull(classifAssocModel);
        }
    }
}
