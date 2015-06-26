using System.Data;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Data;

namespace BrainSharperTests.TestUtils
{
    public static class TestDataBuilder
    {
        public static IDataFrame BuildSmallDataFrameMixedDataTypes()
        {
            return new DataFrame(
                new DataTable("some")
                {
                    Columns =
                    {
                        new DataColumn("Col1", typeof(string)),
                        new DataColumn("Col2", typeof(int)),
                        new DataColumn("Col3", typeof(string)),
                        new DataColumn("Col4", typeof(int))
                    },
                    Rows =
                    {
                        new object []{ "a1.1", 1, "b1.2", 2 },
                        new object []{ "a2.1", 3, "b2.2", 4 },
                        new object []{ "a3.1", 5, "b3.2", 6 },
                    }
                }, new []{ 100, 101, 102 });
        }

        public static IDataFrame BuildSmallDataFrameNumbersOnly()
        {
            return new DataFrame(
               new DataTable("some")
               {
                   Columns =
                   {
                        new DataColumn("Col1", typeof(object)),
                        new DataColumn("Col2", typeof(object)),
                        new DataColumn("Col3", typeof(object)),
                        new DataColumn("Col4", typeof(object))
                   },
                   Rows =
                   {
                        new object []{ 1, 2, 3, 4 },
                        new object []{ 5, 6, 7, 8 },
                        new object []{ 9, 10, 11, 12 },
                   }
               }, new [] { 100, 101, 102 });
        }
    }
}
