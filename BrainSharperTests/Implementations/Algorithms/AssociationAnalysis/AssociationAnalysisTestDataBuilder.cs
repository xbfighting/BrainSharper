using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.Common;

namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis
{
    using System.Data;

    using BrainSharper.Abstract.Data;
    using BrainSharper.Implementations.Data;

    public static class AssociationAnalysisTestDataBuilder
    {
        public const string TranId = "TranId";
        public const string Product = "Product";
        public const string CocaCola = "coca-cola";
        public const string Beer = "beer";
        public const string Diapers = "diapers";
        public const string Nuts = "nuts";

        public static IDataFrame MarketBasketDataSet1 = new DataFrame(
            new DataTable()
            {
                Columns = { new DataColumn(TranId), new DataColumn(Product) },
                Rows =
                    {
                        new object[]{ 1, CocaCola },
                        new object[]{ 1, Nuts },

                        new object[]{ 2, Beer },
                        new object[]{ 2, Nuts },
                        new object[]{ 2, Diapers },

                        new object[]{ 3, CocaCola },

                        new object[]{ 4, CocaCola },
                        new object[]{ 4, Beer },
                        new object[]{ 4, Nuts },

                        new object[]{ 5, Beer },
                        new object[]{ 5, Nuts },
                        new object[]{ 5, Diapers }
                    }
            });

        public static ITransactionsSet<string> AbstractTransactionsSet = new TransactionsSet<string>(
            new List<ITransaction<string>>
            {
                new Transaction<string>(1, "A", "B", "D"),
                new Transaction<string>(2, "B", "C"),
                new Transaction<string>(3, "A", "D", "E"),
                new Transaction<string>(4, "B", "D", "E"),
                new Transaction<string>(5, "A", "B", "C")
            });

        public static ITransactionsSet<string> AbstractTransactionsSet2 = new TransactionsSet<string>(
            new List<ITransaction<string>>
            {
                new Transaction<string>(1, "25", "52", "274"),
                new Transaction<string>(2, "71"),
                new Transaction<string>(3, "71", "274"),
                new Transaction<string>(4, "52"),
                new Transaction<string>(5, "25", "52"),
                new Transaction<string>(6, "274", "71")
            });

        public static ITransactionsSet<string> AbstractTaTransactionsSet3 = new TransactionsSet<string>(
            new ITransaction<string>[]
            {
                new Transaction<string>(1, "a", "b"),
                new Transaction<string>(2, "b", "c", "d"),
                new Transaction<string>(3, "a", "c", "d", "e"),
                new Transaction<string>(4, "a", "d", "e"),
                new Transaction<string>(5, "a", "b", "c"),
                new Transaction<string>(6, "a", "b", "c", "d"),
                new Transaction<string>(7, "a"),
                new Transaction<string>(8, "a", "b", "c"),
                new Transaction<string>(9, "a", "b", "d"),
                new Transaction<string>(10, "b", "c", "e")
            });

        public static ITransactionsSet<IDataItem<string>> AbstractCMARDataSetOnlyFrequentItems = new TransactionsSet<IDataItem<string>>(
            new ITransaction<IDataItem<string>>[]
            {
                new Transaction<IDataItem<string>>(1, new DataItem<string>("A", "a1"), new DataItem<string>("C", "c1"), new DataItem<string>("label", "A")),
                new Transaction<IDataItem<string>>(2, new DataItem<string>("A", "a1"), new DataItem<string>("B", "b2"), new DataItem<string>("C", "c1"), new DataItem<string>("label", "B")),
                new Transaction<IDataItem<string>>(3, new DataItem<string>("D", "d3"), new DataItem<string>("label", "A")),
                new Transaction<IDataItem<string>>(4, new DataItem<string>("A", "a1"), new DataItem<string>("B", "b2"), new DataItem<string>("D", "d3"), new DataItem<string>("label", "C")),
                new Transaction<IDataItem<string>>(2, new DataItem<string>("A", "a1"), new DataItem<string>("B", "b2"), new DataItem<string>("C", "c1"), new DataItem<string>("D", "d3"), new DataItem<string>("label", "C"))
            });

    }
}
