namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees
{
    using BrainSharper.Abstract.MathUtils.ImpurityMeasures;
    using BrainSharper.Implementations.Algorithms.DecisionTrees;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
    using BrainSharper.Implementations.MathUtils.ImpurityMeasures;

    using BrainSharperTests.TestUtils;

    using NUnit.Framework;

    [TestFixture]
    public class BinaryDecisionTreeModelBuilderTests
    {
        private readonly IImpurityMeasure<string> shannonEntropy = new ShannonEntropy<string>();

        [Test]
        public void BinaryTree_NumericAttributes_DiscreteDependentFeatures()
        {
            // Given
            var dataSet = TestDataBuilder.ReadIrisData();
            var binaryTreeBuilder = new BinaryDecisionTreeModelBuilder<string>(
                new InformationGainRatioCalculator<bool, string>(this.shannonEntropy, this.shannonEntropy as ICategoricalImpurityMeasure<string>),
                new BinarySplitSelector<string>(new BinaryDiscreteDataSplitter<string>(), new BinaryNumericDataSplitter()),
                new CategoricalDecisionTreeLeafBuilder());

            // When
            var decisionTree = binaryTreeBuilder.BuildModel(dataSet, "iris_class", null);

            // Then
            Assert.IsNotNull(decisionTree);
        }
    }
}
