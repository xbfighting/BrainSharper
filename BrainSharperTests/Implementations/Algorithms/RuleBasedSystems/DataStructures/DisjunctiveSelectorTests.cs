namespace BrainSharperTests.Implementations.Algorithms.RuleBasedSystems.DataStructures
{
    using BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures;

    using NUnit.Framework;

    [TestFixture]
    public class DisjunctiveSelectorTests
    {
        [Test]
        public void IntersectSelectors_MoreSpecificSelectorReturned()
        {
            // Given
            var selector1 = new DisjunctiveSelector<string>("outlook", "sunny", "overcast");
            var selector2 = new DisjunctiveSelector<string>("outlook", "sunny", "rainy");

            var expectedSelector = new DisjunctiveSelector<string>("outlook", "sunny");

            // When
            var actualSelector = selector1.Intersect(selector2);

            // Then
            Assert.AreEqual(expectedSelector, actualSelector);
        }

        [Test]
        public void IntersectSelectors_EmptySelectorReturned()
        {
            // Given
            var selector1 = new DisjunctiveSelector<string>("outlook", "sunny", "overcast");
            var selector2 = new DisjunctiveSelector<string>("outlook", "rainy");

            var expectedSelector = new EmptySelector<string>("outlook");

            // When
            var actualSelector = selector1.Intersect(selector2);

            // Then
            Assert.AreEqual(expectedSelector, actualSelector);
        }

        [Test]
        public void IntersectSelectors_WithUniversalSelector_ReturnedTheSame()
        {
            // Given
            var selector1 = new DisjunctiveSelector<string>("outlook", "sunny", "overcast");
            var selector2 = new UniversalSelector<string>("outlook");

            var expectedSelector = selector1;

            // When
            var actualSelector = selector1.Intersect(selector2);

            // Then
            Assert.AreEqual(expectedSelector, actualSelector);
        }

        [Test]
        public void IntersectSelectors_WithEmptySelector_ReturnedEmpty()
        {
            // Given
            var selector1 = new DisjunctiveSelector<string>("outlook", "sunny", "overcast");
            var selector2 = new EmptySelector<string>("outlook");

            var expectedSelector = selector2;

            // When
            var actualSelector = selector1.Intersect(selector2);

            // Then
            Assert.AreEqual(expectedSelector, actualSelector);
        }

        [Test]
        public void IsMoreGeneralThan_ReturnsTrue()
        {
            // Given
            var selector1 = new DisjunctiveSelector<string>("outlook", "sunny", "overcast");
            var selector2 = new DisjunctiveSelector<string>("outlook", "sunny");

            // When
            Assert.IsFalse(selector2.IsMoreGeneralThan(selector1));
        }

        [Test]
        public void IsMoreGeneralThan_ReturnsFalse()
        {
            // Given
            var selector1 = new DisjunctiveSelector<string>("outlook", "sunny", "overcast");
            var selector2 = new DisjunctiveSelector<string>("outlook", "sunny");

            // When
            Assert.IsTrue(selector1.IsMoreGeneralThan(selector2));
        }

        [Test]
        public void IsMoreGeneralThan_ReturnsFalse_IfDifferentAttributes()
        {
            // Given
            var selector1 = new DisjunctiveSelector<string>("outlook", "sunny", "overcast");
            var selector2 = new DisjunctiveSelector<string>("temperature", "high");

            // When
            Assert.IsFalse(selector1.IsMoreGeneralThan(selector2));
        }

        [Test]
        public void ValuesRangeOverlaps_RetursTrue()
        {
            // Given
            var selector1 = new DisjunctiveSelector<string>("outlook", "sunny", "overcast");
            var selector2 = new DisjunctiveSelector<string>("outlook", "sunny");

            // When
            Assert.IsTrue(selector1.ValuesRangeOverlap(selector2));
            Assert.IsTrue(selector2.ValuesRangeOverlap(selector1));
        }

        [Test]
        public void ValuesRangeOverlaps_RetursFalse_SameAttributes()
        {
            // Given
            var selector1 = new DisjunctiveSelector<string>("outlook", "sunny", "overcast");
            var selector2 = new DisjunctiveSelector<string>("outlook", "rainy");

            // When
            Assert.IsFalse(selector1.ValuesRangeOverlap(selector2));
            Assert.IsFalse(selector2.ValuesRangeOverlap(selector1));
        }

        [Test]
        public void ValuesRangeOverlaps_RetursFalse_DifferentAttrs()
        {
            // Given
            var selector1 = new DisjunctiveSelector<string>("outlook_1", "sunny", "overcast");
            var selector2 = new DisjunctiveSelector<string>("outlook_2", "rainy");

            // When
            Assert.IsFalse(selector1.ValuesRangeOverlap(selector2));
            Assert.IsFalse(selector2.ValuesRangeOverlap(selector1));
        }

        [Test]
        public void ValuesRangeOverlaps_RetursFalse_EmptySelector()
        {
            // Given
            var selector1 = new DisjunctiveSelector<string>("outlook_1", "sunny", "overcast");
            var selector2 = new EmptySelector<string>("outlook_1");

            // When
            Assert.IsFalse(selector1.ValuesRangeOverlap(selector2));
            Assert.IsFalse(selector2.ValuesRangeOverlap(selector1));
        }

        [Test]
        public void ValuesRangeOverlaps_RetursTrue_UniversalSelector()
        {
            // Given
            var selector1 = new DisjunctiveSelector<string>("outlook_1", "sunny", "overcast");
            var selector2 = new UniversalSelector<string>("outlook_1");

            // When
            Assert.IsTrue(selector1.ValuesRangeOverlap(selector2));
            Assert.IsTrue(selector2.ValuesRangeOverlap(selector1));
        }

    }
}