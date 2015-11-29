namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    using System.Collections.Generic;

    using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;

    using NUnit.Framework;

    [TestFixture]
    public class FrequentItemSetsTests
    {
        [Test]
        public void TestForEquality_ShouldBeTrue()
        {
            // Given
            var frequentItemsSet1 = new FrequentItemsSet<string>(23, 0.23, new object[]{ 1, 2, 3 }, new HashSet<string> { "b", "a", "c" });
            var frequentItemsSet2 = new FrequentItemsSet<string>(23, 0.23, new object[] { 1, 2, 3 }, new HashSet<string> { "b", "a", "c" });

            // Then
            Assert.IsTrue(frequentItemsSet1.Equals(frequentItemsSet2));
            Assert.AreEqual(frequentItemsSet1.GetHashCode(), frequentItemsSet2.GetHashCode());
        }

        [Test]
        public void TestForEquality_ShouldBeFalse()
        {
            // Given
            var frequentItemsSet1 = new FrequentItemsSet<string>(23, 0.23, new object[] { 1, 2, 3 }, new HashSet<string> { "a", "b", "c" });
            var frequentItemsSet2 = new FrequentItemsSet<string>(23, 0.23, new object[] { 1, 2, 3 }, new HashSet<string> { "a", "b", "b" });

            // Then
            Assert.IsFalse(frequentItemsSet1.Equals(frequentItemsSet2));
            Assert.AreNotEqual(frequentItemsSet1.GetHashCode(), frequentItemsSet2.GetHashCode());
        }
    }
}
