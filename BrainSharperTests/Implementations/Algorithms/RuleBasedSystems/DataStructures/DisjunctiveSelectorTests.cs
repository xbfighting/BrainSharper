namespace BrainSharperTests.Implementations.Algorithms.RuleBasedSystems.DataStructures
{
    using BrainSharper.Implementations.Algorithms.RuleBasedSystems.DataStructures;
    using NUnit.Framework;

    [TestFixture]
    public class DisjunctiveSelectorTests
    {
        [Test]
        public void IntersectSelectors_MoreSpecificSelectorReturned()
        {
            // Given
            var selector1 = new DisjunctiveSelector("outlook", "sunny", "overcast");
            var selector2 = new DisjunctiveSelector("outlook", "sunny", "rainy");

            var expectedSelector = new DisjunctiveSelector("outlook", "sunny");

            // When
            var actualSelector = selector1.Intersect(selector2);

            // Then
            Assert.AreEqual(expectedSelector, actualSelector);
        }

        [Test]
        public void IntersectSelectors_EmptySelectorReturned()
        {
            // Given
            var selector1 = new DisjunctiveSelector("outlook", "sunny", "overcast");
            var selector2 = new DisjunctiveSelector("outlook", "rainy");

            var expectedSelector = new EmptySelector("outlook");

            // When
            var actualSelector = selector1.Intersect(selector2);

            // Then
            Assert.AreEqual(expectedSelector, actualSelector);
        }

        [Test]
        public void IntersectSelectors_WithUniversalSelector_ReturnedTheSame()
        {
            // Given
            var selector1 = new DisjunctiveSelector("outlook", "sunny", "overcast");
            var selector2 = new UniversalSelector("outlook");

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
            var selector1 = new DisjunctiveSelector("outlook", "sunny", "overcast");
            var selector2 = new EmptySelector("outlook");

            var expectedSelector = selector2;

            // When
            var actualSelector = selector1.Intersect(selector2);

            // Then
            Assert.AreEqual(expectedSelector, actualSelector);
        }

        [Test]
        public void IsMoreDetailedThan_ReturnsTrue()
        {
            // Given
            var selector1 = new DisjunctiveSelector("outlook", "sunny", "overcast");
            var selector2 = new DisjunctiveSelector("outlook", "sunny");

            // When
            Assert.IsTrue(selector2.IsMoreDetailedThan(selector1));
        }

        [Test]
        public void IsMoreDetailedThan_ReturnsFalse()
        {
            // Given
            var selector1 = new DisjunctiveSelector("outlook", "sunny", "overcast");
            var selector2 = new DisjunctiveSelector("outlook", "sunny");

            // When
            Assert.IsFalse(selector1.IsMoreDetailedThan(selector2));
        }

        [Test]
        public void IsMoreDetailedThan_ReturnsFalse_IfDifferentAttributes()
        {
            // Given
            var selector1 = new DisjunctiveSelector("outlook", "sunny", "overcast");
            var selector2 = new DisjunctiveSelector("temperature", "high");

            // When
            Assert.IsFalse(selector1.IsMoreDetailedThan(selector2));
        }
    }
}
