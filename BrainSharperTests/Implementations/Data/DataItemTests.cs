namespace BrainSharperTests.Implementations.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using BrainSharper.Abstract.Data;
    using BrainSharper.Implementations.Data;

    using NUnit.Framework;

    [TestFixture]
    public class DataItemTests
    {
        [Test]
        public void SortDataItemsOnFeatureNameAndFeatureValues_OperatesOnStrings()
        {
            // Given
            var dataItems = new List<IDataItem<string>>
                                {
                                    new DataItem<string>("c", "b"), new DataItem<string>("c", "a"),
                                    new DataItem<string>("a", "b"), new DataItem<string>("a", "a"),
                                    new DataItem<string>("b", "b"), new DataItem<string>("b", "a")
                                };
            var expectedItems = new List<IDataItem<string>>
                                {
                                    new DataItem<string>("a", "a"), new DataItem<string>("a", "b"),
                                    new DataItem<string>("b", "a"), new DataItem<string>("b", "b"),
                                    new DataItem<string>("c", "a"), new DataItem<string>("c", "b")
                                };

            // When
            dataItems.Sort();

            // Then
            CollectionAssert.AreEqual(expectedItems, dataItems);
        }

        [Test]
        public void SortDataItemsOnFeatureNameAndFeatureValues_OperatesOnObjects()
        {
            // Given
            var dataItems = new List<IDataItem<object>>
                                {
                                    new DataItem<object>("c", 2), new DataItem<object>("c", 1),
                                    new DataItem<object>("a", 2), new DataItem<object>("a", 1),
                                    new DataItem<object>("b", 2), new DataItem<object>("b", 1)
                                };
            var expectedItems = new List<IDataItem<object>>
                                {
                                    new DataItem<object>("a", 1), new DataItem<object>("a", 2),
                                    new DataItem<object>("b", 1), new DataItem<object>("b", 2),
                                    new DataItem<object>("c", 1), new DataItem<object>("c", 2)
                                };

            // When
            dataItems.Sort();

            // Then
            CollectionAssert.AreEqual(expectedItems, dataItems);
        }
    }
}
