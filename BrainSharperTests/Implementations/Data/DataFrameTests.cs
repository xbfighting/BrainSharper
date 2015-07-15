using System;
using System.Data;
using System.Linq;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Data;
using BrainSharperTests.TestUtils;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Data
{
    [TestFixture]
    public class DataFrameTests
    {
        private IDataFrame _subject;

        [SetUp]
        public void SetUpSubject()
        {
            _subject = TestDataBuilder.BuildSmallDataFrameMixedDataTypes();
        }

        [Test]
        public void Test_GetRowVector_StronglyTypedVersion_UseRowIndex()
        {
            // Given
            var expectedVector = new DataVector<string>(
                new[] { "a1.1", "1", "b1.2", "2" },
                _subject.ColumnNames
                );

            // When
            var actualVector = _subject.GetRowVector<string>(0);

            // Then
            Assert.AreEqual(expectedVector, actualVector);
        }

        [Test]
        public void Test_DataFrameFrom_Matrix_NoColumnNames()
        {
            // Given
            var expectedMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
           {
                { 1, 2, 3, 4 },
                { 5, 6, 666, 8 },
                { 9, 10, 11, 12 },
           });

            // When
            var dataTableFromMatrix = new DataFrame(expectedMatrix);
            var actualMatrix = dataTableFromMatrix.GetAsMatrix();

            // Then
            Assert.IsTrue(expectedMatrix.Equals(actualMatrix));
        }

        [Test]
        public void Test_DataFrameFrom_Matrix_CustomColumnNames()
        {
            // Given
            var baseMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
           {
                { 1, 2, 3, 4 },
                { 5, 6, 666, 8 },
                { 9, 10, 11, 12 },
           });
            var columnNames = new[] { "Col1", "Col2", "Col3", "Col4" };
            var dataTable = new DataTable("some")
            {
                Columns = { "Col1", "Col2", "Col3", "Col4" },
                Rows =
                {
                    { 1, 2, 3, 4 },
                    { 5, 6, 666, 8 },
                    { 9, 10, 11, 12 },
                }
            };
            var expectedDataFrame = new DataFrame(dataTable);

            // When
            var actualDataFrame = new DataFrame(baseMatrix, columnNames);

            // Then
            Assert.IsTrue(expectedDataFrame.Equals(actualDataFrame));
        }

        [Test]
        public void Test_GetRowVector_StronglyTypedVersion_UseRowName()
        {
            // Given
            var expectedVector = new DataVector<string>(
                new[] { "a1.1", "1", "b1.2", "2" },
                _subject.ColumnNames
                );

            // When
            var actualVector = _subject.GetRowVector<string>(100, true);

            // Then
            Assert.AreEqual(expectedVector, actualVector);
        }

        [Test]
        public void Test_GetRowVector_ObjectBasedVersion_UseRowIndex()
        {
            // Given
            var expectedVector = new DataVector<object>(
                new object[] { "a1.1", 1, "b1.2", 2 },
                _subject.ColumnNames
                );

            // When
            var actualVector = _subject.GetRowVector(0);

            // Then
            Assert.AreEqual(expectedVector, actualVector);
        }

        [Test]
        public void Test_GetRowVector_ObjectBasedVersion_UseRowName()
        {
            // Given
            var expectedVector = new DataVector<object>(
                new object[] { "a1.1", 1, "b1.2", 2 },
                _subject.ColumnNames
                );

            // When
            var actualVector = _subject.GetRowVector(100, true);

            // Then
            Assert.AreEqual(expectedVector, actualVector);
        }

        [Test]
        public void Test_GetColumnVector_ByIndex_StronglyTypedVarsion()
        {
            // Given
            var expectedVector = new DataVector<string>(new[] { "a1.1", "a2.1", "a3.1" }, "Col1");

            // When
            var actualVector = _subject.GetColumnVector<string>(0);

            // Then
            Assert.IsTrue(expectedVector.Equals(actualVector));
        }

        [Test]
        public void Test_GetColumnVector_ByName_StronglyTypedVarsion()
        {
            // Given
            var expectedVector = new DataVector<string>(new[] { "a1.1", "a2.1", "a3.1" }, "Col1");

            // When
            var actualVector = _subject.GetColumnVector<string>("Col1");

            // Then
            Assert.IsTrue(expectedVector.Equals(actualVector));
        }

        [Test]
        public void Test_GetColumnVector_ByIndex_ObjectVarsion()
        {
            // Given
            var expectedVector = new DataVector<object>(new object[] { "a1.1", "a2.1", "a3.1" }, "C1");

            // When
            var actualVector = _subject.GetColumnVector<object>(0);

            // Then
            Assert.AreEqual(expectedVector, actualVector);
        }

        [Test]
        public void Test_GetColumnVector_ByName_ObjectVarsion()
        {
            // Given
            var expectedVector = new DataVector<object>(new object[] { "a1.1", "a2.1", "a3.1" }, "Col1");

            // When
            var actualVector = _subject.GetColumnVector<object>("Col1");

            // Then
            Assert.IsTrue(expectedVector.Equals(actualVector));
        }

        [Test]
        public void Test_GetNumericColumnVector_ByIndex()
        {
            // Given
            var expectedNumericColumnVector = Vector<double>.Build.Dense(new double[] { 1, 3, 5 });

            // When
            var actualNumericColumnVector = _subject.GetNumericColumnVector(1);

            // Then
            Assert.IsTrue(expectedNumericColumnVector.Equals(actualNumericColumnVector));
        }

        [Test]
        public void Test_GetNumericColumnVector_ByName()
        {
            // Given
            var expectedNumericColumnVector = Vector<double>.Build.Dense(new double[] { 1, 3, 5 });

            // When
            var actualNumericColumnVector = _subject.GetNumericColumnVector("Col2");

            // Then
            Assert.IsTrue(expectedNumericColumnVector.Equals(actualNumericColumnVector));
        }

        [Test]
        public void Test_GetNumericRowVector_ByRowIndex()
        {
            // Given
            var dataFrame = TestDataBuilder.BuildSmallDataFrameNumbersOnly();
            var expectedNumericColumnVector = Vector<double>.Build.Dense(new double[] { 1, 2, 3, 4 });

            // When
            var actualNumericColumnVector = dataFrame.GetNumericRowVector(0);

            // Then
            Assert.IsTrue(expectedNumericColumnVector.Equals(actualNumericColumnVector));
        }

        [Test]
        public void Test_GetNumericRowVector_ByRowName()
        {
            // Given
            var dataFrame = TestDataBuilder.BuildSmallDataFrameNumbersOnly();
            var expectedNumericColumnVector = Vector<double>.Build.Dense(new double[] { 1, 2, 3, 4 });

            // When
            var actualNumericColumnVector = dataFrame.GetNumericRowVector(100, true);

            // Then
            Assert.IsTrue(expectedNumericColumnVector.Equals(actualNumericColumnVector));
        }

        [Test]
        public void Test_GetSubset_ByColumns_UsingColumnIndices()
        {
            // Given
            var newDataTable = new DataTable()
            {
                Columns =
                {
                    new DataColumn("Col1", typeof (string)),
                    new DataColumn("Col3", typeof (string))
                },
                Rows =
                {
                    new object[] {"a1.1", "b1.2"},
                    new object[] {"a2.1", "b2.2"},
                    new object[] {"a3.1", "b3.2"},
                }
            };
            var expectedDataFrame = new DataFrame(newDataTable, new[] { 100, 101, 102, 103 });

            // When
            var actualDataFrame = _subject.GetSubsetByColumns(new[] { 0, 2 });

            // Then
            Assert.IsTrue(expectedDataFrame.Equals(actualDataFrame));
        }

        [Test]
        public void Test_GetSubset_ByColumns_UsingColumnNames()
        {
            var vec = new DataVector<object>(new object[] { "aa", 50, "bb" }, new[] { "f1", "f2", "f3" });
            vec.MemberwiseSet((string name, object val) => name == "f1" ? name.ToString().ToUpper() : name.ToUpper());

            // Given
            var newDataTable = new DataTable()
            {
                Columns =
                {
                    new DataColumn("Col1", typeof (string)),
                    new DataColumn("Col3", typeof (string))
                },
                Rows =
                {
                    new object[] {"a1.1", "b1.2"},
                    new object[] {"a2.1", "b2.2"},
                    new object[] {"a3.1", "b3.2"},
                }
            };
            var expectedDataFrame = new DataFrame(newDataTable, new[] { 100, 101, 102 });

            // When
            var actualDataFrame = _subject.GetSubsetByColumns(new[] { "Col1", "Col3" });

            // Then
            Assert.IsTrue(expectedDataFrame.Equals(actualDataFrame));
        }

        [Test]
        public void Test_GetSubset_ByRows_UsingRowIndices()
        {
            // Given
            var newDataTable = new DataTable()
            {
                Columns =
                {
                    new DataColumn("Col1", typeof (string)),
                    new DataColumn("Col2", typeof (int)),
                    new DataColumn("Col3", typeof (string)),
                    new DataColumn("Col4", typeof (int))
                },
                Rows =
                {
                    new object []{ "a1.1", 1, "b1.2", 2 },
                    new object []{ "a3.1", 5, "b3.2", 6 }
                }
            };
            var expectedDataFrame = new DataFrame(newDataTable, new[] { 100, 102 });

            // When
            var actualDataFrame = _subject.GetSubsetByRows(new[] { 0, 2 });

            // Then
            Assert.IsTrue(expectedDataFrame.Equals(actualDataFrame));
        }

        [Test]
        public void Test_GetAsMatrix()
        {
            // Given
            var expectedMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
            {
                { 1, 2, 3, 4 },
                { 5, 6, 7, 8 },
                { 9, 10, 11, 12 },
            });

            var basicDataFrame = TestDataBuilder.BuildSmallDataFrameNumbersOnly();

            // When
            var actualMatrix = basicDataFrame.GetAsMatrix();

            // Then
            Assert.IsTrue(expectedMatrix.Equals(actualMatrix));
        }

        [Test]
        public void Test_Set_UsingRowIndex_ColumnIndex()
        {
            // Given
            var expectedMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
            {
                { 1, 2, 3, 4 },
                { 5, 6, 666, 8 },
                { 9, 10, 11, 12 },
            });

            var basicDataFrame = TestDataBuilder.BuildSmallDataFrameNumbersOnly();

            // When
            var actualDataFrame = basicDataFrame.Set(666, 1, 2);
            var actualMatrix = actualDataFrame.GetAsMatrix();

            // Then
            Assert.IsTrue(expectedMatrix.Equals(actualMatrix));
        }

        [Test]
        public void Test_Set_UsingRowName_ColumnIndex()
        {
            // Given
            var expectedMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
            {
                { 1, 2, 3, 4 },
                { 5, 6, 666, 8 },
                { 9, 10, 11, 12 },
            });

            var basicDataFrame = TestDataBuilder.BuildSmallDataFrameNumbersOnly();

            // When
            var actualDataFrame = basicDataFrame.Set(666, 101, 2, true);
            var actualMatrix = actualDataFrame.GetAsMatrix();

            // Then
            Assert.IsTrue(expectedMatrix.Equals(actualMatrix));
        }

        [Test]
        public void Test_Set_UsingRowIndex_ColumnName()
        {
            // Given
            var expectedMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
            {
                { 1, 2, 3, 4 },
                { 5, 6, 666, 8 },
                { 9, 10, 11, 12 },
            });

            var basicDataFrame = TestDataBuilder.BuildSmallDataFrameNumbersOnly();

            // When
            var actualDataFrame = basicDataFrame.Set(666, 1, "Col3");
            var actualMatrix = actualDataFrame.GetAsMatrix();

            // Then
            Assert.IsTrue(expectedMatrix.Equals(actualMatrix));
        }

        [Test]
        public void Test_Set_UsingRowName_ColumnName()
        {
            // Given
            var expectedMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
            {
                { 1, 2, 3, 4 },
                { 5, 6, 666, 8 },
                { 9, 10, 11, 12 },
            });

            var basicDataFrame = TestDataBuilder.BuildSmallDataFrameNumbersOnly();

            // When
            var actualDataFrame = basicDataFrame.Set(666, 101, "Col3", true);
            var actualMatrix = actualDataFrame.GetAsMatrix();

            // Then
            Assert.IsTrue(expectedMatrix.Equals(actualMatrix));
        }

        [Test]
        public void Test_ProcessMultiple_WithRowIndex_ColumnName()
        {
            // Given
            DataFrameRowIndexColumnNameOperator<double> rowOperator = (rowIdx, colName, currentVal) =>
            {
                if (colName == "Col1")
                {
                    return Math.Pow(currentVal, 2);
                }
                return currentVal;
            };

            var expectedMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
               {
                    { 1, 2, 3, 4 },
                    { 25, 6, 7, 8 },
                    { 81, 10, 11, 12 },
               });

            var baseDataFrame = TestDataBuilder.BuildSmallDataFrameNumbersOnly();

            // When
            var actualMatrix = baseDataFrame.ProcessMultiple(rowOperator).GetAsMatrix();

            // Then
            Assert.IsTrue(expectedMatrix.Equals(actualMatrix));
        }

        [Test]
        public void Test_ProcessMultiple_WithRowIndex_ColumnIndex()
        {
            // Given
            DataFrameRowIndexColumnIndexOperator<double> rowOperator = (rowIdx, colIdx, currentVal) => colIdx == 0 ? Math.Pow(currentVal, 2) : currentVal;

            var expectedMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
               {
                    { 1, 2, 3, 4 },
                    { 25, 6, 7, 8 },
                    { 81, 10, 11, 12 },
               });

            var baseDataFrame = TestDataBuilder.BuildSmallDataFrameNumbersOnly();

            // When
            var actualMatrix = baseDataFrame.ProcessMultiple(rowOperator).GetAsMatrix();

            // Then
            Assert.IsTrue(expectedMatrix.Equals(actualMatrix));
        }

        [Test]
        public void Test_ProcessMultiple_WithRowName_ColumnName()
        {
            // Given
            DataFrameRowNameColumnNameOperator<double> rowOperator = (rowName, colName, currentVal) =>
            {
                if (colName == "Col1" && rowName != 101)
                {
                    return Math.Pow(currentVal, 2);
                }
                return currentVal;
            };

            var expectedMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
               {
                    { 1, 2, 3, 4 },
                    { 5, 6, 7, 8 },
                    { 81, 10, 11, 12 },
               });

            var baseDataFrame = TestDataBuilder.BuildSmallDataFrameNumbersOnly();

            // When
            var actualMatrix = baseDataFrame.ProcessMultiple(rowOperator).GetAsMatrix();

            // Then
            Assert.IsTrue(expectedMatrix.Equals(actualMatrix));
        }

        [Test]
        public void Test_ProcessMultiple_WithRowName_ColumnIndex()
        {
            // Given
            DataFrameRowNameColumnIndexOperator<double> rowOperator = (rowName, colIdx, currentVal) =>
            {
                if (colIdx == 0 && rowName != 101)
                {
                    return Math.Pow(currentVal, 2);
                }
                return currentVal;
            };

            var expectedMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
               {
                    { 1, 2, 3, 4 },
                    { 5, 6, 7, 8 },
                    { 81, 10, 11, 12 },
               });

            var baseDataFrame = TestDataBuilder.BuildSmallDataFrameNumbersOnly();

            // When
            var actualMatrix = baseDataFrame.ProcessMultiple(rowOperator).GetAsMatrix();

            // Then
            Assert.IsTrue(expectedMatrix.Equals(actualMatrix));
        }

        [Test]
        public void GetRowsWhere()
        {
            // Given
            var expectedMatrixMeetingCriteria = Matrix<double>.Build.DenseOfArray(new double[,]
              {
                  { 9, 10, 11, 12 },
              });

            var expectedMatrixNotMeetingCriteria = Matrix<double>.Build.DenseOfArray(new double[,]
              {
                    { 1, 2, 3, 4 },
                    { 5, 6, 7, 8 }
              });

            var baseDataFrame = TestDataBuilder.BuildSmallDataFrameNumbersOnly();
            Predicate<DataRow> rowFilter = row => Convert.ToDouble(row[0]) > 5;

            // When
            var filteredRows = baseDataFrame.GetRowsIndicesWhere(rowFilter);
            var matrixMeetingCriteria =
                baseDataFrame.GetSubsetByRows(filteredRows.IndicesOfRowsMeetingCriteria).GetAsMatrix();
            var matrixNotMeetingCriteria =
                baseDataFrame.GetSubsetByRows(filteredRows.IndicesOfRowsNotMeetingCriteria).GetAsMatrix();

            // Then
            Assert.IsTrue(expectedMatrixMeetingCriteria.Equals(matrixMeetingCriteria));
            Assert.IsTrue(expectedMatrixNotMeetingCriteria.Equals(matrixNotMeetingCriteria));
        }
    }
}
