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
        public void BinaryTreeNumericAttributesDiscreteDependentFeatures()
        {
            // Given
            var dataSet = TestDataBuilder.ReadIrisData();
            var binaryTreeBuilder = new BinaryDecisionTreeModelBuilder(
                new InformationGainRatioCalculator<string>(this.shannonEntropy, this.shannonEntropy as ICategoricalImpurityMeasure<string>),
                new BinarySplitSelectorForCategoricalOutcome(new BinaryDiscreteDataSplitter(), new BinaryNumericDataSplitter(), new ClassBreakpointsNumericSplitFinder()),
                new CategoricalDecisionTreeLeafBuilder());

            // When
            var decisionTree = binaryTreeBuilder.BuildModel(dataSet, "iris_class", null);

            // Then
            Assert.IsNotNull(decisionTree);
        }
    }
}
