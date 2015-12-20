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

    }
}
