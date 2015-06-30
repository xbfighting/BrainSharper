using BrainSharper.Implementations.MathUtils.Normalizers;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.MathUtils
{
    [TestFixture]
    public class MinMaxNormalizerTest
    {
        [Test]
        public void Test_MinMaxNormalization()
        {
            // Given
            var matrix = Matrix<double>.Build.DenseOfArray(new double[,]
            {
                    { 1.0, 2.0 },
                    { 5.0, 5.0 },
                    { 10.0, 8.0 }

            });

            var expectedMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
            {
                    { 0.0,   2.0 },
                    { 0.444, 5.0 },
                    { 1.0,   8.0 }
            });

            var subject = new MinMaxNormalizer();

            // When
            var actualMatrix = subject.NormalizeColumns(matrix, new []{ 0 });

            // Then
            Assert.IsTrue(expectedMatrix.AlmostEqual(actualMatrix, 0.009));
        }
    }
}
