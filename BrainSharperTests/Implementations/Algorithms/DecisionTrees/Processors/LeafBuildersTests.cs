namespace BrainSharperTests.Implementations.Algorithms.DecisionTrees.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
    using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;
    using BrainSharper.Implementations.Data;

    using BrainSharperTests.TestUtils;

    using MathNet.Numerics.LinearAlgebra;

    using NUnit.Framework;
    using NUnit.Framework.Constraints;

    [TestFixture]
    public class LeafBuildersTests
    {
        [Test]
        public void TestRegressionAndModelLeafBuilder_RandomizedData()
        {
            // Given
            // Data generated with equation: 3 + (data["x1"] * 2) + (data["x2"] * 3) + (data["x3"] * 4)
            Func<IList<double>, double> resultCalc = list => 3 + (list[0] * 2) + (list[1] * 3) + (list[2] * 4);
            var testData = TestDataBuilder.BuildRandomAbstractNumericDataFrame(resultCalc, 100000, 3, min: 1, max: 100);
            var leafBuilder = new RegressionAndModelDecisionTreeLeafBuilder();

            // When
            var result = leafBuilder.BuildLeaf(testData, "result") as IDecisionTreeRegressionAndModelLeaf;

            //Then
            Assert.AreEqual(3.0, result.ModelWeights[0], 0.01);
            Assert.AreEqual(2.0, result.ModelWeights[1], 0.01);
            Assert.AreEqual(3.0, result.ModelWeights[2], 0.01);
            Assert.AreEqual(4.0, result.ModelWeights[3], 0.01);
        }
    }
}
