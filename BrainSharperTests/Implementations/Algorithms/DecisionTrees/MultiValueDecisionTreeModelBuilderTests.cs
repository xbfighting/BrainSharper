namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees
{
    using System.Linq;

    using BrainSharper.Abstract.Algorithms.DecisionTrees;
    using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
    using BrainSharper.Abstract.MathUtils.ImpurityMeasures;
    using BrainSharper.Implementations.Algorithms.DecisionTrees;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
    using BrainSharper.Implementations.MathUtils.ImpurityMeasures;

    using BrainSharperTests.TestUtils;

    using NUnit.Framework;

    [TestFixture]
    public class MultiValueDecisionTreeModelBuilderTests
    {
        private readonly IImpurityMeasure<string> shannonEntropy = new ShannonEntropy<string>();
        private readonly IDecisionTreeModelBuilder categoricalSubject;

        public MultiValueDecisionTreeModelBuilderTests()
        {
             this.categoricalSubject = new MultiSplitDecisionTreeModelBuilder(
                new InformationGainRatioCalculator<string>(this.shannonEntropy, this.shannonEntropy as ICategoricalImpurityMeasure<string>),
                new MultiValueSplitSelectorForCategoricalOutcome(
                    new MultiValueDiscreteDataSplitter(), 
                    new BinaryNumericDataSplitter(),
                    new ClassBreakpointsNumericSplitFinder()),
                new CategoricalDecisionTreeLeafBuilder());
        }

        [Test]
        public void BuildMultiValueDecisionTreeCategoricalVariablesOnly()
        {
            // Given
            var testData = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();
            
            // When
            var model = categoricalSubject.BuildModel(testData, "Play", null) as IDecisionTreeParentNode;

            // Then
            Assert.IsNotNull(model);
            Assert.AreEqual(3, model.Children.Count);
            Assert.AreEqual("Outlook", model.DecisionFeatureName);

            // First child of the root
            var sunnyChild = model.GetChildForTestResult("Sunny") as IDecisionTreeParentNode;
            Assert.IsNotNull(sunnyChild);
            Assert.AreEqual("Humidity", sunnyChild.DecisionFeatureName);
            Assert.AreEqual(2, sunnyChild.Children.Count);
            Assert.IsTrue(sunnyChild.Children.All(chd => chd is IDecisionTreeLeaf));

            // Second child of the root
            var overcastChild = model.GetChildForTestResult("Overcast") as IDecisionTreeLeaf;
            Assert.AreEqual("Yes", overcastChild.LeafValue);

            // Third child of the root
            var rainyChild = model.GetChildForTestResult("Rainy") as IDecisionTreeParentNode;
            Assert.IsNotNull(rainyChild);
            Assert.AreEqual("Windy", rainyChild.DecisionFeatureName);
            Assert.AreEqual(2, rainyChild.Children.Count);
            Assert.IsTrue(rainyChild.Children.All(chd => chd is IDecisionTreeLeaf));
        }
    }
}
