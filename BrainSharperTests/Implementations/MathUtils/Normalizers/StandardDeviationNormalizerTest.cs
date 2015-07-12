using BrainSharper.Implementations.MathUtils.Normalizers;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.MathUtils.Normalizers
{
    [TestFixture]
    public class StandardDeviationNormalizerTest
    {
        [Test]
        public void Test_StandardDeviationNormalization()
        {
            // Given
            var matrix = Matrix<double>.Build.DenseOfArray(new double[,]
            {
                    { 1.0, 2.0, 3.0 },
                    { 4.0, 5.0, 6.0 },
                    { 7.0, 8.0, 9.0 }

            });

            var expectedMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
            {
                    { -1.0, -1.0, -1.0 },
                    { 0.0, 0.0, 0.0 },
                    { 1.0, 1.0, 1.0 }

            });

            var subject = new StandardDeviationNormalizer();

            // When
            var actualMatrix = subject.NormalizeColumns(matrix);

            // Then
            Assert.IsTrue(expectedMatrix.Equals(actualMatrix));
        }
    }
}
