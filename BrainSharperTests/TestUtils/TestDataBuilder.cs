using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
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

        public static IDataVector<object> BuildMixedObjectsVector()
        {
            return new DataVector<object>(new object[] { "A", 1, "B", 2 }, new []{ "F1", "F2", "F3", "F4" });
        }

        public static IDataVector<object> BuildNumericVector()
        {
            return new DataVector<object>(new object[] { 1.0, 2.0, 3.0, 4.0 }, new[] { "F1", "F2", "F3", "F4" });
        }

        /// <summary>
        /// Creates data frame with values calculated using the following linear equation:
        /// f6 = f1 + 2f2 - 3f3
        /// </summary>
        /// <returns>Data frame with numeric outcomes</returns>
        public static IDataFrame BuildRandomAbstractNumericDataFrame(int rowCount = 10000, Random randomizer = null, int min = 1, int max = 10)
        {
            randomizer = randomizer ?? new Random();
            var dataTable = new DataTable("some table")
            {
                Columns = {"F1", "F2", "F3", "F4", "F5", "F6"}
            };
            for (int i = 0; i < rowCount; i++)
            {
                var features = new double[6];
                for (int fIdx = 0; fIdx < 5; fIdx++)
                {
                    features[fIdx] = randomizer.Next(min, max);
                }
                features[5] = CalcualteLinearlyDependentFeatureValue(features);
                dataTable.Rows.Add(features.Select(val => val as object).ToArray());
            }
            return new DataFrame(dataTable);
        }

        public static double CalcualteLinearlyDependentFeatureValue(IList<double> features)
        {
            return features[0] + 2 * features[1] - 3 * features[2];
        }
    }
}
