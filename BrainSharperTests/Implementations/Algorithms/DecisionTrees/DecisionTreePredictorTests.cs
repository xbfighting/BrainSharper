using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.DecisionTrees;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;
using BrainSharper.General.DataQuality;
using BrainSharper.General.DataUtils;
using BrainSharper.Implementations.Algorithms.DecisionTrees;
using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
using BrainSharper.Implementations.MathUtils.ImpurityMeasures;
using BrainSharperTests.TestUtils;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees
{
    [TestFixture]
    public class DecisionTreePredictorTests
    {
        private readonly IImpurityMeasure<string> _shannonEntropy = new ShannonEntropy<string>();
        private readonly IDecisionTreeModelBuilder _binaryTreeBuilder;

        public DecisionTreePredictorTests()
        {
            _binaryTreeBuilder =  new BinaryDecisionTreeModelBuilder<string>(
                new InformationGainRatioCalculator<string>(_shannonEntropy, _shannonEntropy as ICategoricalImpurityMeasure<string>),
                new BinarySplitSelector<string>(new BinaryDiscreteDataSplitter<string>(), new BinaryNumericDataSplitter()),
                new DiscreteDecisionTreeLeafBuilder());
        }

        [Test]
        public void DiscreteClassification_NumericFeatures_IrisData_CrossValidation()
        {
            // Given
            var randomizer = new Random();
            var splitter = new CrossValidator<object>();
            var testData = TestDataBuilder.ReadIrisData();
            var predictor = new DecisionTreePredictor();

            // When
            var accuracies = splitter.CrossValidate(
                _binaryTreeBuilder,
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
    }
}
