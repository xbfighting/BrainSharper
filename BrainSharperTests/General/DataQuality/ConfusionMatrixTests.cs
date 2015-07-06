using System.Linq;
using BrainSharper.General.DataQuality;
using BrainSharper.Implementations.MathUtils.Normalizers;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using NUnit.Framework;

namespace BrainSharperTests.General.DataQuality
{
    [TestFixture]
    public class ConfusionMatrixTests
    {
        [Test]
        public void Test_AccuracyMultiClass()
        {
            
            // Given
            var expectedValues = new[] { "a", "a", "b", "b", "c", "c" };
            var actualValues = new[] { "a", "b", "b", "c", "c", "a" };
            var confusionMatrix = new ConfusionMatrix<string>(expectedValues, actualValues);
            
            // Then
            Assert.AreEqual(0.5, confusionMatrix.Accuracy);
        }
    }
}
