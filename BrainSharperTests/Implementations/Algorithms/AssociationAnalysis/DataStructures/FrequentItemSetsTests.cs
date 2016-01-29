namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    using System;
    using System.Collections.Generic;

    using BrainSharper.Abstract.Data;
    using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;
    using BrainSharper.Implementations.Data;

    using NUnit.Framework;

    [TestFixture]
    public class FrequentItemSetsTests
    {
        [Test]
        public void TestForEquality_ShouldBeTrue()
        {
            // Given
            var frequentItemsSet1 = new FrequentItemsSet<string>(new object[]{ 1, 2, 3 }, new HashSet<string> { "b", "a", "c" }, 23, 0.23);
            var frequentItemsSet2 = new FrequentItemsSet<string>(new object[] { 1, 2, 3 }, new HashSet<string> { "b", "a", "c" }, 23, 0.23);

            // Then
            Assert.IsTrue(frequentItemsSet1.Equals(frequentItemsSet2));
            Assert.AreEqual(frequentItemsSet1.GetHashCode(), frequentItemsSet2.GetHashCode());
        }

        [Test]
        public void TestForEquality_ShouldBeFalse()
        {
            // Given
            var frequentItemsSet1 = new FrequentItemsSet<string>(new object[] { 1, 2, 3 }, new HashSet<string> { "a", "b", "c" }, 23, 0.23);
            var frequentItemsSet2 = new FrequentItemsSet<string>(new object[] { 1, 2, 3 }, new HashSet<string> { "a", "b", "b" }, 23, 0.23);

            // Then
            Assert.IsFalse(frequentItemsSet1.Equals(frequentItemsSet2));
            Assert.AreNotEqual(frequentItemsSet1.GetHashCode(), frequentItemsSet2.GetHashCode());
        }

        [Test]
        public void TestForEquality_ForKEqual2Prefix_ShouldBeEqual()
        {
            // Given
            var frequentItemsSet1 = new FrequentItemsSet<IDataItem<object>>( 
                new object[] { 1, 2, 3 }, 
                new HashSet<IDataItem<object>>
                    {
                        new DataItem<object>("col1", "val1"),
                        new DataItem<object>("col2", 2),
                        new DataItem<object>("col3", new DateTime(2013, 10, 10)),
                    },
                23,
                0.23);
            var frequentItemsSet2 = new FrequentItemsSet<IDataItem<object>>(
                new object[] { 1, 2, 3 },
                new HashSet<IDataItem<object>>
                    {
                        new DataItem<object>("col1", "val1"),
                        new DataItem<object>("col2", 2),
                        new DataItem<object>("col3", new DateTime(2013, 10, 11)),
                    },
                23,
                0.23);

            // When
            var isPrefixKEqual = frequentItemsSet1.KFirstElementsEqual(frequentItemsSet2, 2);

            // Then
            Assert.IsTrue(isPrefixKEqual);
        }

        [Test]
        public void TestForEquality_ForKEqual3Prefix_ShouldNotBeEqual()
        {
            // Given
            var frequentItemsSet1 = new FrequentItemsSet<IDataItem<object>>(
                new object[] { 1, 2, 3 },
                new HashSet<IDataItem<object>>
                    {
                        new DataItem<object>("col1", "val1"),
                        new DataItem<object>("col2", 2),
                        new DataItem<object>("col3", new DateTime(2013, 10, 10)),
                    },
                23,
                0.23);
            var frequentItemsSet2 = new FrequentItemsSet<IDataItem<object>>(
                new object[] { 1, 2, 3 },
                new HashSet<IDataItem<object>>
                    {
                        new DataItem<object>("col1", "val1"),
                        new DataItem<object>("col2", 2),
                        new DataItem<object>("col3", new DateTime(2013, 10, 11)),
                    },
                23,
                0.23);

            // When
            var isPrefixKEqual = frequentItemsSet1.KFirstElementsEqual(frequentItemsSet2, 3);

            // Then
            Assert.IsFalse(isPrefixKEqual);
        }

        [Test]
        public void TestForItemsOnlyEqual_ShouldBeEqual()
        {
            // Given
            var frequentItemsSet1 = new FrequentItemsSet<IDataItem<object>>(
                new object[] { 1, 2, 3 },
                new HashSet<IDataItem<object>>
                    {
                        new DataItem<object>("col1", "val1"),
                        new DataItem<object>("col2", 2),
                        new DataItem<object>("col3", new DateTime(2013, 10, 10)),
                    },
                23,
                0.23);
            var frequentItemsSet2 = new FrequentItemsSet<IDataItem<object>>(
                new object[] { 1, 2, 3 },
                new HashSet<IDataItem<object>>
                    {
                        new DataItem<object>("col1", "val1"),
                        new DataItem<object>("col2", 2),
                        new DataItem<object>("col3", new DateTime(2013, 10, 10)),
                    },
                23,
                0.23);

            // When
            var isPrefixKEqual = frequentItemsSet1.KFirstElementsEqual(frequentItemsSet2, 3);

            // Then
            Assert.IsTrue(frequentItemsSet1.ItemsOnlyEqual(frequentItemsSet2));
        }

        [Test]
        public void TestForItemsOnlyEqual_ShouldNotBeEqual()
        {
            // Given
            var frequentItemsSet1 = new FrequentItemsSet<IDataItem<object>>(
                new object[] { 1, 2, 3 },
                new HashSet<IDataItem<object>>
                    {
                        new DataItem<object>("col1", "val1"),
                        new DataItem<object>("col2", 2),
                        new DataItem<object>("col3", new DateTime(2013, 10, 10)),
                    },
                23,
                0.23);
            var frequentItemsSet2 = new FrequentItemsSet<IDataItem<object>>(
                new object[] { 1, 2, 3 },
                new HashSet<IDataItem<object>>
                    {
                        new DataItem<object>("col1", "val1"),
                        new DataItem<object>("col2", 2),
                        new DataItem<object>("col3", new DateTime(2013, 10, 11)),
                    },
                 23,
                0.23);

            // When
            var isPrefixKEqual = frequentItemsSet1.KFirstElementsEqual(frequentItemsSet2, 3);

            // Then
            Assert.IsFalse(frequentItemsSet1.ItemsOnlyEqual(frequentItemsSet2));
        }
    }
}
