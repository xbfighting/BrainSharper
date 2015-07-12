using System.Collections.Generic;
using System.Linq;
using BrainSharper.Implementations.MathUtils.ImpurityMeasures;
using BrainSharperTests.TestUtils;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.MathUtils.ImpurityMeasures
{
    [TestFixture]
    public class ImpurityMeasuresTests
    {
        private readonly ShannonEntropy<string> _shannonEntropy = new ShannonEntropy<string>();
        private readonly GiniIndex<string> _giniIndexCalculator = new GiniIndex<string>();

        [Test]
        public void Test_ShannonEntropy()
        {
            // Given
            var vector = new[] { "a", "a", "b", "b", "b" };
            var expectedEntropy = 0.9709;

            // When
            var actualEntropy = _shannonEntropy.ImpurityValue(vector);

            // Then
            Assert.AreEqual(
                expectedEntropy,
                actualEntropy,
                0.0009
                );
        }

        [Test]
        public void Test_GiniIndex()
        {
            // Given
            var vector = new[] { "a", "a", "a", "a", "b", "b", "b", "c", "c", "c" };
            var expectedGiniIndex = 0.66;

            // When
            var actualGiniIndex = _giniIndexCalculator.ImpurityValue(vector);

            // Then
            Assert.AreEqual(
                expectedGiniIndex,
                actualGiniIndex,
                0.0009
                );
        }

    }
}
