using System.Data;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Data;
using BrainSharperTests.TestUtils;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Data
{
    /// <summary>
    /// Complex tests of DataFrame
    /// </summary>
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
                new[] {"a1.1", "1", "b1.2", "2"},
                _subject.ColumnNames
                );

            // When
            var actualVector = _subject.GetRowVector<string>(0, false);

            // Then
            Assert.AreEqual(expectedVector, actualVector);
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
            var expectedVector = new DataVector<string>(new[] {"a1.1", "a2.1", "a3.1"}, "Col1");

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
            var expectedDataFrame = new DataFrame(newDataTable, new[] {100, 101, 102, 103});

            // When
            var actualDataFrame = _subject.GetSubsetByColumns(new[] { 0, 2 });

            // Then
            Assert.IsTrue(expectedDataFrame.Equals(actualDataFrame));
        }

        [Test]
        public void Test_GetSubset_ByColumns_UsingColumnNames()
        {
            var vec = new DataVector<object>(new object[] {"aa", 50, "bb"}, new[] {"f1", "f2", "f3"});
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
            var actualDataFrame = _subject.GetSubsetByColumns(new[] { "Col1","Col3" });

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
            var actualDataFrame = _subject.GetSubsetByRows(new[] {0, 2});

            // Then
            Assert.IsTrue(expectedDataFrame.Equals(actualDataFrame));
        }
    }
}
