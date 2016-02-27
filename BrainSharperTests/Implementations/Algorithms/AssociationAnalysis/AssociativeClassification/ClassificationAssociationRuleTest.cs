using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common;
using BrainSharper.Implementations.Data;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification
{
    [TestFixture]
    public class ClassificationAssociationRuleTest
    {
        [Test]
        public void TestCoversExample_ResultTrue()
        {
            // Given
            var dataVector = new DataVector<object>(new object[] { "aaa", 2 }, new [] { "col1", "col2" });
            var assocRule = new ClassificationAssociationRule<object>(
                new FrequentItemsSet<IDataItem<object>>(
                    0, 0,
                    new DataItem<object>("col1", "aaa"),
                    new DataItem<object>("col2", 2)
                    ),
                new FrequentItemsSet<IDataItem<object>>(0, 0, new DataItem<object>("result", "super")), 
                10,
                0.1,
                0.9
                );

            // Then
           Assert.IsTrue(assocRule.Covers(dataVector));
        }

        [Test]
        public void TestCoversExample_ValuesAreDifferent_ResultFalse()
        {
            // Given
            var dataVector = new DataVector<object>(new object[] { "aaa", 3 }, new[] { "col1", "col2" });
            var assocRule = new ClassificationAssociationRule<object>(
                new FrequentItemsSet<IDataItem<object>>(
                    0, 0,
                    new DataItem<object>("col1", "aaa"),
                    new DataItem<object>("col2", 2)
                    ),
                new FrequentItemsSet<IDataItem<object>>(0, 0, new DataItem<object>("result", "super")),
                10,
                0.1,
                0.9
                );

            // Then
            Assert.IsFalse(assocRule.Covers(dataVector));
        }

        [Test]
        public void TestCoversExample_FeatureIsMissing_ResultFalse()
        {
            // Given
            var dataVector = new DataVector<object>(new object[] { "aaa" }, new[] { "col1" });
            var assocRule = new ClassificationAssociationRule<object>(
                new FrequentItemsSet<IDataItem<object>>(
                    0, 0,
                    new DataItem<object>("col1", "aaa"),
                    new DataItem<object>("col2", 2)
                    ),
                new FrequentItemsSet<IDataItem<object>>(0, 0, new DataItem<object>("result", "super")),
                10,
                0.1,
                0.9
                );

            // Then
            Assert.IsFalse(assocRule.Covers(dataVector));
        }
    }
}
