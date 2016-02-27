using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common;

namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.DataStructures
{
    using System.Collections.Generic;

    using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
    using BrainSharper.Abstract.Data;
    using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;
    using BrainSharper.Implementations.Data;

    using static AssociationAnalysisTestDataBuilder;

    using NUnit.Framework;

    [TestFixture]
    public class DataUtilsTests
    {
        [Test]
        public void TransformDataFrameToTransactions_GenericMethod_TransactionIdSpecified()
        {
            // Given
            var dataFrame = MarketBasketDataSet1;

            var expectedTransactionsSet = new TransactionsSet<IDataItem<string>>(
                new List<ITransaction<IDataItem<string>>>
                    {
                        new Transaction<IDataItem<string>>("1", new DataItem<string>(Product, CocaCola), new DataItem<string>(Product, Nuts)),
                        new Transaction<IDataItem<string>>("2", new DataItem<string>(Product, Beer), new DataItem<string>(Product, Nuts), new DataItem<string>(Product, Diapers)),
                        new Transaction<IDataItem<string>>("3", new DataItem<string>(Product, CocaCola)),
                        new Transaction<IDataItem<string>>("4", new DataItem<string>(Product, CocaCola), new DataItem<string>(Product, Beer), new DataItem<string>(Product, Nuts)),
                        new Transaction<IDataItem<string>>("5", new DataItem<string>(Product, Beer), new DataItem<string>(Product, Nuts), new DataItem<string>(Product, Diapers))
                    }
                );

            // When
            var actualDataSet = dataFrame.ToAssociativeTransactionsSet<string>(TranId);

            // Then
            CollectionAssert.AreEquivalent(expectedTransactionsSet.TransactionsList, actualDataSet.TransactionsList);
        }

        [Test]
        public void TransformDataFrameToTransactions_GenericMethod_NoTransactionIdSpecified()
        {
            // Given
            var dataFrame = MarketBasketDataSet1;

            var expectedTransactionsSet = new TransactionsSet<IDataItem<string>>(
                new List<ITransaction<IDataItem<string>>>
                    {
                        new Transaction<IDataItem<string>>(0, new DataItem<string>(Product, CocaCola), new DataItem<string>(Product, Nuts)),
                        new Transaction<IDataItem<string>>(1, new DataItem<string>(Product, Beer), new DataItem<string>(Product, Nuts), new DataItem<string>(Product, Diapers)),
                        new Transaction<IDataItem<string>>(2, new DataItem<string>(Product, CocaCola)),
                        new Transaction<IDataItem<string>>(3, new DataItem<string>(Product, CocaCola), new DataItem<string>(Product, Beer), new DataItem<string>(Product, Nuts)),
                        new Transaction<IDataItem<string>>(4, new DataItem<string>(Product, Beer), new DataItem<string>(Product, Nuts), new DataItem<string>(Product, Diapers))
                    }
                );

            // When
            var actualDataSet = dataFrame.ToAssociativeTransactionsSet<string>();

            // Then
            CollectionAssert.AreEquivalent(expectedTransactionsSet.TransactionsList, actualDataSet.TransactionsList);
        }

        [Test]
        public void TransformDataFrameToTransactions_NoneGenericMethod_TransactionIdSpecified()
        {
            // Given
            var dataFrame = MarketBasketDataSet1;

            var expectedTransactionsSet = new TransactionsSet<IDataItem<object>>(
                new List<ITransaction<IDataItem<object>>>
                    {
                        new Transaction<IDataItem<object>>("1", new DataItem<object>(Product, CocaCola), new DataItem<object>(Product, Nuts)),
                        new Transaction<IDataItem<object>>("2", new DataItem<object>(Product, Beer), new DataItem<object>(Product, Nuts), new DataItem<object>(Product, Diapers)),
                        new Transaction<IDataItem<object>>("3", new DataItem<object>(Product, CocaCola)),
                        new Transaction<IDataItem<object>>("4", new DataItem<object>(Product, CocaCola), new DataItem<object>(Product, Beer), new DataItem<object>(Product, Nuts)),
                        new Transaction<IDataItem<object>>("5", new DataItem<object>(Product, Beer), new DataItem<object>(Product, Nuts), new DataItem<object>(Product, Diapers))
                    }
                );

            // When
            var actualDataSet = dataFrame.ToAssociativeTransactionsSet(TranId);

            // Then
            CollectionAssert.AreEquivalent(expectedTransactionsSet.TransactionsList, actualDataSet.TransactionsList);
        }
    }
}
