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
             this.categoricalSubject = new MultiSplitDecisionTreeModelBuilder<string>(
            new InformationGainRatioCalculator<string, string>(this.shannonEntropy, this.shannonEntropy as ICategoricalImpurityMeasure<string>),
            new MultiValueSplitSelectorForCategoricalOutcome<string>(new MultiValueDiscreteDataSplitter<string>()),
            new CategoricalDecisionTreeLeafBuilder());
        }

        [Test]
        public void BuildMultiValueDecisionTree_CategoricalVariablesOnly()
        {
            // Given
            var testData = TestDataBuilder.ReadWeatherDataWithCategoricalAttributes();
            
            // When
            var model = this.categoricalSubject.BuildModel(testData, "Play", null) as IDecisionTreeParentNode<string>;

            // Then
            Assert.IsNotNull(model);
            Assert.AreEqual(3, model.Children.Count);
            Assert.AreEqual("Outlook", model.DecisionFeatureName);

            // First child of the root
            var sunnyChild = model.GetChildForTestResult("Sunny") as IDecisionTreeParentNode<string>;
            Assert.IsNotNull(sunnyChild);
            Assert.AreEqual("Humidity", sunnyChild.DecisionFeatureName);
            Assert.AreEqual(2, sunnyChild.Children.Count);
            Assert.IsTrue(sunnyChild.Children.All(chd => chd is IDecisionTreeLeaf));

            // Second child of the root
            var overcastChild = model.GetChildForTestResult("Overcast") as IDecisionTreeLeaf;
            Assert.AreEqual("Yes", overcastChild.LeafValue);

            // Third child of the root
            var rainyChild = model.GetChildForTestResult("Rainy") as IDecisionTreeParentNode<string>;
            Assert.IsNotNull(rainyChild);
            Assert.AreEqual("Windy", rainyChild.DecisionFeatureName);
            Assert.AreEqual(2, rainyChild.Children.Count);
            Assert.IsTrue(rainyChild.Children.All(chd => chd is IDecisionTreeLeaf));
        }
    }
}
