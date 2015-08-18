namespace BrainSharperTests.General.CollectionUtils
{
    using BrainSharper.General.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class CumulativeSumTest
    {
        [Test]
        public void TestCumulativeSum()
        {
            // Given
            var sequence = new[] { 1.0, 2.0, 3.0, 4.0  };
            var expectedCumSum = new[] { 1.0, 3.0, 6.0, 10.0 };

            // When
            var actualCumSum = sequence.CumulativeSum();

            // Then
            CollectionAssert.AreEqual(expectedCumSum, actualCumSum);
        }
    }
}
