namespace BrainSharperTests.Implementations.Algorithms.RuleBasedSystems.DataStructures
{
    using System.Collections.Generic;

    using BrainSharper.Abstract.Data;
    using BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures;
    using BrainSharper.Implementations.Data;

    using NUnit.Framework;

    [TestFixture]
    public class RangeSelectorTests
    {
        [Test]
        public void CoversExample_RangeSelector_ValueInTheMiddle()
        {
            // Given
            var selector = new RangeSelector<object>("B", 5, 10, true, true);
            var example = new DataVector<object>(
                new object[] { "qqq", 8 },
                new[] { "A", "B" });
            
            // When
            Assert.IsTrue(selector.Covers(example));
        }

        [Test]
        public void CoversExample_RangeSelector_FromInclusive_ValueIsFrom()
        {
            // Given
            var selector = new RangeSelector<object>("B", 5, 10, true, true);
            var example = new DataVector<object>(
                new object[] { "qqq", 5 },
                new[] { "A", "B" });

            // When
            Assert.IsTrue(selector.Covers(example));
        }

        [Test]
        public void CoversExample_RangeSelector_ToInclusive_ValueIsTo()
        {
            // Given
            var selector = new RangeSelector<object>("B", 5, 10, true, true);
            var example = new DataVector<object>(
                new object[] { "qqq", 10 },
                new[] { "A", "B" });

            // When
            Assert.IsTrue(selector.Covers(example));
        }

        [Test]
        public void NotCoversExample_RangeSelector_FromExclusive_ValueIsFrom()
        {
            // Given
            var selector = new RangeSelector<object>("B", 5, 10);
            var example = new DataVector<object>(
                new object[] { "qqq", 5 },
                new[] { "A", "B" });

            // When
            Assert.IsFalse(selector.Covers(example));
        }

        [Test]
        public void NotCoversExample_RangeSelector_ToExclusive_ValueIsTo()
        {
            // Given
            var selector = new RangeSelector<object>("B", 5, 10);
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
            var selector = new RangeSelector<object>("B", 5, 10);
            var example = new DataVector<object>(
                new object[] { "qqq", 20 },
                new[] { "A", "B" });

            // When
            Assert.IsFalse(selector.Covers(example));
        }
    }
}