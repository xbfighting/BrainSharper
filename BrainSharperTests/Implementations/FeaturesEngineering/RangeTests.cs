namespace BrainSharperTests.Implementations.FeaturesEngineering
{
    using BrainSharper.Implementations.Data;
    using BrainSharper.Implementations.FeaturesEngineering;

    using NUnit.Framework;

    [TestFixture]
    public class RangeTests
    {
        [Test]
        public void CoversExample_RangeSelector_ValueInTheMiddle()
        {
            // Given
            var selector = new Range("B", 5, 10);
            var example = new DataVector<object>(
                new object[] { "qqq", 8 },
                new[] { "A", "B" });
            
            // When
            Assert.IsTrue(selector.Covers(example));
        }

        [Test]
        public void CoversExample_RangeSelector_ValueEqualsFrom()
        {
            // Given
            var selector = new Range("B", 5, 10);
            var example = new DataVector<object>(
                new object[] { "qqq", 5 },
                new[] { "A", "B" });

            // When
            Assert.IsTrue(selector.Covers(example));
        }

        [Test]
        public void CoversExample_RangeSelector_ValueEqualsTo()
        {
            // Given
            var selector = new Range("B", 5, 10);
            var example = new DataVector<object>(
                new object[] { "qqq", 10 },
                new[] { "A", "B" });

            // When
            Assert.IsFalse(selector.Covers(example));
        }

        [Test]
        public void NotCoversExample_RangeSelector_ValueOutsideRange()
        {
            // Given
            var selector = new Range("B", 5, 10);
            var example = new DataVector<object>(
                new object[] { "qqq", 20 },
                new[] { "A", "B" });

            // When
            Assert.IsFalse(selector.Covers(example));
        }

        [Test]
        public void IsMoreGeneralThan_IfIsSuperSet()
        {
            // Given
            var selector1 = new Range("B", 5, 10);
            var selector2 = new Range("B", 6, 10);

            // Then
            Assert.That(selector1.IsMoreGeneralThan(selector2));
        }

        [Test]
        public void IsMoreGeneralThan_IfAreTheSame()
        {
            // Given
            var selector1 = new Range("B", 5, 10);
            var selector2 = new Range("B", 5, 10);

            // Then
            Assert.IsFalse(selector1.IsMoreGeneralThan(selector2));
        }

        [Test]
        public void IsMoreGeneralThan_IfAreNotOverlapping()
        {
            // Given
            var selector1 = new Range("B", 5, 10);
            var selector2 = new Range("B", 1, 3);

            // Then
            Assert.IsFalse(selector1.IsMoreGeneralThan(selector2));
        }
    }
}