namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees.Helpers
{
    using System.Data;

    using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.DataStructures.BinaryDecisionTrees;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Helpers;
    using BrainSharper.Implementations.Data;

    using MathNet.Numerics.Distributions;

    using NUnit.Framework;

    [TestFixture]
    public class ChiSquareSignificanceTest
    {
        [Test]
        public void TestSignificanceOfSplit()
        {
            // Given
            var subject = new ChiSquareStatisticalSignificanceChecker(0.1);
            var testTable = new DataTable()
                                    {
                                        Columns =
                                            {
                                                new DataColumn("Gender", typeof(string)),
                                                new DataColumn("Play", typeof(bool))
                                            },
                                        Rows =
                                            {
                                                { "F", false },
                                                { "F", false },
                                                { "F", false },
                                                { "F", false },
                                                { "F", false },
                                                { "F", false },
                                                { "F", false },
                                                { "F", false },
                                                { "F", true },
                                                { "F", true },
                                                { "M", true },
                                                { "M", true },
                                                { "M", true },
                                                { "M", true },
                                                { "M", true },
                                                { "M", true },
                                                { "M", true },
                                                { "M", true },
                                                { "M", true },
                                                { "M", true },
                                                { "M", true },
                                                { "M", true },
                                                { "M", true },
                                                { "M", false },
                                                { "M", false },
                                                { "M", false },
                                                { "M", false },
                                                { "M", false },
                                                { "M", false },
                                                { "M", false },
                                            }
                                    };
            var dataFrame = new DataFrame(testTable);

            var males = dataFrame.GetSubsetByQuery("Gender = 'M'");
            var females = dataFrame.GetSubsetByQuery("Gender = 'F'");

            var splittedData = new BinarySplittingResult(
                false,
                "Gender",
                new[]
                    {
                        new SplittedData(new BinaryDecisionTreeLink(0.333, 10, true), females) as ISplittedData, 
                        new SplittedData(new BinaryDecisionTreeLink(0.666, 10, false), males) as ISplittedData,
                    },
                "F");

            // When
            var isSplitSignificant = subject.IsSplitStatisticallySignificant(dataFrame, splittedData, "Play");

            // Then
            Assert.IsTrue(isSplitSignificant);

        }
    }
}