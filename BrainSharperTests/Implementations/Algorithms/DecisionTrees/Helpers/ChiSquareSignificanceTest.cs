using System.Collections.Generic;

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
        private static readonly string DependentFeatureName = "Play";
        private static readonly string SplittingFeatureName = "Gender";
        private static readonly string FemaleField = "F";
        private static readonly string MaleField = "M";

        [Test]
        public void TestSignificanceOfSplitUsingDataFrames()
        {
            // Given
            var subject = new ChiSquareStatisticalSignificanceChecker(0.1);
            var testTable = new DataTable()
                                    {
                                        Columns =
                                            {
                                                new DataColumn(SplittingFeatureName, typeof(string)),
                                                new DataColumn(DependentFeatureName, typeof(bool))
                                            },
                                        Rows =
                                            {
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, true },
                                                { FemaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, false },
                                                { MaleField, false },
                                                { MaleField, false },
                                                { MaleField, false },
                                                { MaleField, false },
                                                { MaleField, false },
                                                { MaleField, false },
                                            }
                                    };
            var dataFrame = new DataFrame(testTable);

            var males = dataFrame.GetSubsetByQuery($"{SplittingFeatureName} = '{MaleField}'");
            var females = dataFrame.GetSubsetByQuery($"{SplittingFeatureName} = '{FemaleField}'");

            var splittedData = new BinarySplittingResult(
                false,
                SplittingFeatureName,
                new[]
                    {
                        new SplittedData(new BinaryDecisionTreeLink(0.333, 10, true), females) as ISplittedData, 
                        new SplittedData(new BinaryDecisionTreeLink(0.666, 10, false), males) as ISplittedData,
                    },
                FemaleField);

            // When
            var isSplitSignificant = subject.IsSplitStatisticallySignificant(dataFrame, splittedData, DependentFeatureName);

            // Then
            Assert.IsTrue(isSplitSignificant);
        }

        [Test]
        public void TestSignificanceOfSplitUsingExpectedActualValues()
        {
            // Given
            var subject = new ChiSquareStatisticalSignificanceChecker(0.1);
            var testTable = new DataTable()
            {
                Columns =
                                            {
                                                new DataColumn(SplittingFeatureName, typeof(string)),
                                                new DataColumn(DependentFeatureName, typeof(bool))
                                            },
                Rows =
                                            {
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, false },
                                                { FemaleField, true },
                                                { FemaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, true },
                                                { MaleField, false },
                                                { MaleField, false },
                                                { MaleField, false },
                                                { MaleField, false },
                                                { MaleField, false },
                                                { MaleField, false },
                                                { MaleField, false },
                                            }
            };
            var dataFrame = new DataFrame(testTable);

            var males = dataFrame.GetSubsetByQuery($"{SplittingFeatureName} = '{MaleField}'");
            var females = dataFrame.GetSubsetByQuery($"{SplittingFeatureName} = '{FemaleField}'");

            // When
            var isSplitSignificant = subject.IsSplitStatisticallySignificant(
                dataFrame.GetColumnVector<bool>(DependentFeatureName),
                new List<IList<bool>> { males.GetColumnVector<bool>(DependentFeatureName), females.GetColumnVector<bool>(DependentFeatureName) }
                );

            // Then
            Assert.IsTrue(isSplitSignificant);
        }
    }
}