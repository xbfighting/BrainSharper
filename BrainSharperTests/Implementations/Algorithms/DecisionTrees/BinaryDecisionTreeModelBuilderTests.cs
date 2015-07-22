using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainSharper.Abstract.MathUtils.ImpurityMeasures;
using BrainSharper.Implementations.Algorithms.DecisionTrees;
using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
using BrainSharper.Implementations.MathUtils.ImpurityMeasures;
using BrainSharperTests.TestUtils;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees
{
    [TestFixture]
    public class BinaryDecisionTreeModelBuilderTests
    {
        private readonly IImpurityMeasure<string> _shannonEntropy = new ShannonEntropy<string>();

        [Test]
        public void BinaryTree_NumericAttributes_DiscreteDependentFeatures()
        {
            // Given
            var dataSet = TestDataBuilder.ReadIrisData();
            var binaryTreeBuilder = new BinaryDecisionTreeModelBuilder<string>(
                new InformationGainRatioCalculator<string>(_shannonEntropy, _shannonEntropy as ICategoricalImpurityMeasure<string>),
                new BinarySplitSelector<string>(new BinaryDiscreteDataSplitter<string>(), new BinaryNumericDataSplitter()),
                new DiscreteDecisionTreeLeafBuilder<string>());

            // When
            var decisionTree = binaryTreeBuilder.BuildModel(dataSet, "iris_class", null);

            // Then
            Assert.IsNotNull(decisionTree);
        }
    }
}
