namespace BrainSharperTests.Implementations.Algorithms.RuleBasedSystems.DataStructures
{
    using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Implementations.Data;

    using NUnit.Framework;

    [TestFixture]
    public class ComplexTests
    {
        private readonly ISelector<string> outlookSelector1 = new DisjunctiveSelector<string>("outlook", "sunny", "overcast");
        private readonly ISelector<string> outlookSelector2 = new DisjunctiveSelector<string>("outlook", "sunny");
        private readonly ISelector<string> temperatureSelector1 = new DisjunctiveSelector<string>("temperature", "high", "moderate");
        private readonly ISelector<string> temperatureSelector2 = new DisjunctiveSelector<string>("temperature", "high");

        [Test]
        public void IntersectComplexes_DifferentValuesAllowed1()
        {
            // Given
            var complex1 = new Complex<string>(new[] { outlookSelector1, temperatureSelector2 });
            var complex2 = new Complex<string>(new[] { outlookSelector2, temperatureSelector1 });
            var expectedComplex = new Complex<string>(new[] { outlookSelector2, temperatureSelector2 });

            // When
            var intersectedComplex = complex1.Intersect(complex2);

            // Then
            Assert.AreEqual(expectedComplex, intersectedComplex);
        }

        [Test]
        public void IntersectComplexes_DifferentValuesAllowed_OneComplexHasMoreAttributesCovered()
        {
            // Given
            var complex1 = new Complex<string>(new[] { outlookSelector1, temperatureSelector1 });
            var complex2 = new Complex<string>(new[] { outlookSelector2 });
            var expectedComplex = new Complex<string>(new[] { outlookSelector2, temperatureSelector1 });

            // When
            var intersectedComplex = complex1.Intersect(complex2);

            // Then
            Assert.AreEqual(expectedComplex, intersectedComplex);
        }

        [Test]
        public void IntersectComplexes_OneIsUniversal_ResultIsMoreSpecific()
        {
            // Given
            var complex1 = new Complex<string>(new[] { outlookSelector1, temperatureSelector1 });
            var complex2 = new Complex<string>();
            var expectedComplex = new Complex<string>(new[] { outlookSelector1, temperatureSelector1 });

            // When
            var intersectedComplex = complex1.Intersect(complex2);

            // Then
            Assert.AreEqual(expectedComplex, intersectedComplex);
        }

        [Test]
        public void IntersectComplexes_OneIsEmpty_ResultIsEmpty()
        {
            // Given
            var emptyTemperatureSelector = new EmptySelector<string>("temperature");
            var complex1 = new Complex<string>(new[] { outlookSelector1, temperatureSelector1 });
            var complex2 = new Complex<string>(new ISelector<string>[] { emptyTemperatureSelector });
            var expectedComplex = new Complex<string>(new[] { outlookSelector1, emptyTemperatureSelector });

            // When
            var intersectedComplex = complex1.Intersect(complex2);

            // Then
            Assert.AreEqual(expectedComplex, intersectedComplex);
            Assert.IsTrue(intersectedComplex.IsEmpty);
        }

        [Test]
        public void IsMoreGeneralThan_ReturnsTrue()
        {
            // Given
            var complex1 = new Complex<string>(new[] { outlookSelector1, temperatureSelector1 });
            var complex2 = new Complex<string>(new[] { outlookSelector2, temperatureSelector2 });

            // Then
            Assert.IsTrue(complex2.IsMoreGeneralThan(complex1));
        }

        [Test]
        public void IsMoreGeneralThan_ReturnsFalse()
        {
            // Given
            var complex1 = new Complex<string>(new[] { outlookSelector1, temperatureSelector1 });
            var complex2 = new Complex<string>(new[] { outlookSelector2, temperatureSelector2 });

            // Then
            Assert.IsFalse(complex1.IsMoreGeneralThan(complex2));
        }

        [Test]
        public void CoversExample_AllExplicitAttributesMatch_ReturnsTrue()
        {
            //Given
            var complex1 = new Complex<string>(new[] { outlookSelector1, temperatureSelector1 });
            var example = new DataVector<string>(new[] { "sunny", "high", "qqq" }, new[] { "outlook", "temperature", "other_attr" });

            // Then
            Assert.IsTrue(complex1.Covers(example));
        }

        [Test]
        public void CoversExample_NotAllExplicitAttributesMatch_ReturnsFalse()
        {
            //Given
            var complex1 = new Complex<string>(new[] { outlookSelector1, temperatureSelector1 });
            var example = new DataVector<string>(new[] { "sunny", "xxx" }, new[] { "outlook", "temperature" });

            // Then
            Assert.IsFalse(complex1.Covers(example));
        }

        //TODO: !!! AAA Add more tests for: 1. intersecting with more attributes, 2.More general than when more attributes are covered
    }
}
