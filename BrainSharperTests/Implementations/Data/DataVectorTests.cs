using System;
using BrainSharper.Abstract.Data;
using BrainSharperTests.TestUtils;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data.Common;
using BrainSharper.Implementations.Data;

namespace BrainSharperTests.Implementations.Data
{
    [TestFixture]
    public class DataVectorTests
    {
        [Test]
        public void Test_MemberwiseSet_UsingIndex()
        {
            // Given
            VectorMemberwiseIndexOpertor<object> vectorOperator =
                (featureIndex, value) => featureIndex == 1 ? Math.Pow(Convert.ToDouble(value), 2) : value;

            var expectedVector = Vector<double>.Build.Dense(new[] {1.0, 4.0, 3.0, 4.0});

            // When
            var actualVector = TestDataBuilder.BuildNumericVector().MemberwiseSet(vectorOperator).NumericVector;

            // Then
            Assert.True(expectedVector.Equals(actualVector));
        }

        [Test]
        public void Test_MemberwiseSet_UsingFeatureNames()
        {
            // Given
            VectorMemberwiseNameOpertor<object> vectorOperator =
                (featureName, value) => featureName == "F2" ? Math.Pow(Convert.ToDouble(value), 2) : value;

            var expectedVector = Vector<double>.Build.Dense(new[] {1.0, 4.0, 3.0, 4.0});

            // When
            var actualVector = TestDataBuilder.BuildNumericVector().MemberwiseSet(vectorOperator).NumericVector;

            // Then
            Assert.True(expectedVector.Equals(actualVector));
        }

        [Test]
        public void Test_DataItems_Vector()
        {
            // Given
            var expectedDataItems = new List<IDataItem<object>>
            {
                new DataItem<object>("F1", "A"),
                new DataItem<object>("F2", 1),
                new DataItem<object>("F3", "B"),
                new DataItem<object>("F4", 2)
            };

            // When

            var actualDataItems = TestDataBuilder.BuildMixedObjectsVector().DataItems;

            // Then
            CollectionAssert.AreEquivalent(expectedDataItems, actualDataItems);
        }
    }
}
