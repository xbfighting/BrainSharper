using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.DecisionTrees;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;
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
        public void DiscreteClassification_NumericFeatures_IrisData()
        {
            // Given
            var randomizer = new Random();
            var splitter = new StandardSplitter(randomizer);
            var testData = TestDataBuilder.ReadIrisData();
            var trainTestIndices = splitter.SplitData(testData, 0.8);

            var trainingData = testData.GetSubsetByRows(trainTestIndices[DataType.Training]);
            var testingData = testData.GetSubsetByRows(trainTestIndices[DataType.Testing]);
            var expectedResults = testingData.GetColumnVector<string>("iris_class").ToList();

            var predictor = new DecisionTreePredictor();

            // When
            var model = _binaryTreeBuilder.BuildModel(trainingData, "iris_class", null);
            var predictions = predictor.Predict(testingData, model, "iris_class");

            // Then
            var correct = 0.0;
            var incorrect = 0.0;
            for (int i = 0; i < predictions.Count; i++)
            {
                var predictedVal = predictions[i];
                var expectedVal = expectedResults[i];
                if (expectedVal.Equals(predictedVal))
                {
                    correct += 1;
                }
                else
                {
                    incorrect += 1;
                }
            }
            var accuracy = correct/(double) (correct + incorrect);
            Assert.IsTrue(accuracy > 0.9);
        }
    }
}
