namespace BrainSharperTests.Implementations.Algorithms.RuleBasedSystems.DataStructures
{
    using BrainSharper.Abstract.Algorithms.RuleBasedSystems.DataStructures;
    using BrainSharper.Implementations.Algorithms.RuleBasedSystems.DataStructures;

    using NUnit.Framework;

    [TestFixture]
    public class ComplexTests
    {
        private ISelector outlookSelector1 = new DisjunctiveSelector("outlook", "sunny", "overcast");
        private ISelector outlookSelector2 = new DisjunctiveSelector("outlook", "sunny");
        private ISelector temperatureSelector1 = new DisjunctiveSelector("temperature", "high", "moderate");
        private ISelector temperatureSelector2 = new DisjunctiveSelector("temperature", "high");

        [Test]
        public void IntersectComplexes_DifferentValuesAllowed()
        {
            // Given
            var complex1 = new Complex(new[] { outlookSelector1, temperatureSelector1 });
            var complex2 = new Complex(new[] { outlookSelector2, temperatureSelector2 });
            var expectedComplex = new Complex(new[] { outlookSelector2, temperatureSelector2 });

            // When
            var intersectedComplex = complex1.Intersect(complex2);

            // Then
            Assert.AreEqual(expectedComplex, intersectedComplex);
        }

        [Test]
        public void IntersectComplexes_DifferentValuesAllowed_OneSelectorIsUniversal()
        {
            // Given
            var complex1 = new Complex(new[] { outlookSelector1, temperatureSelector1 });
            var complex2 = new Complex(new[] { outlookSelector2 });
            var expectedComplex = new Complex(new[] { outlookSelector2, temperatureSelector1 });

            // When
            var intersectedComplex = complex1.Intersect(complex2);

            // Then
            Assert.AreEqual(expectedComplex, intersectedComplex);
        }

        [Test]
        public void IntersectComplexes_OneIsEmpty_ResultIsEmptyComplex()
        {
            // Given
            var complex1 = new Complex(new[] { outlookSelector1, temperatureSelector1 });
            var complex2 = new Complex();
            var expectedComplex = new Complex();

            // When
            var intersectedComplex = complex1.Intersect(complex2);

            // Then
            Assert.AreEqual(expectedComplex, intersectedComplex);
            Assert.IsTrue(intersectedComplex.IsEmpty);
        }
    }
}
