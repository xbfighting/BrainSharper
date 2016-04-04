using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.FPGrowth;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.FpGrowth
{
    [TestFixture]
    public class FpGrowthNodeTests
    {
        [Test]
        public void TestGettingPathToSelfIncludingSelfIncludingRoot()
        {
            // Given
            var tree = new FpGrowthNode<string>();
            var firstChild = new FpGrowthNode<string>("A", count: 1);
            var firstChildOfFirstChild = new FpGrowthNode<string>("Aa", count: 2);
            var secondChildOfFirstChild = new FpGrowthNode<string>("Ab", count: 2);

            firstChild.AddChild(firstChildOfFirstChild);
            firstChild.AddChild(secondChildOfFirstChild);

            var secondChild = new FpGrowthNode<string>("B", count: 1);
            var firstChildOfSecondChild = new FpGrowthNode<string>("Ba", count: 2);
            var secondChildOfSecondChild = new FpGrowthNode<string>("Bb", count: 2);

            secondChild.AddChild(firstChildOfSecondChild);
            secondChild.AddChild(secondChildOfSecondChild);

            tree.AddChild(firstChild);
            tree.AddChild(secondChild);

            var expectedPath = new List<FpGrowthNode<string>> { secondChildOfSecondChild, secondChild, tree };

            // When
            var pathToBb = secondChildOfSecondChild.GetPathToSelf(true, true);

            // Then
            CollectionAssert.AreEqual(pathToBb, expectedPath);
        }

        [Test]
        public void TestGettingPathToSelfWithoutSelfIncludingRoot()
        {
            // Given
            var tree = new FpGrowthNode<string>();
            var firstChild = new FpGrowthNode<string>("A", count: 1);
            var firstChildOfFirstChild = new FpGrowthNode<string>("Aa", count: 2);
            var secondChildOfFirstChild = new FpGrowthNode<string>("Ab", count: 2);

            firstChild.AddChild(firstChildOfFirstChild);
            firstChild.AddChild(secondChildOfFirstChild);

            var secondChild = new FpGrowthNode<string>("B", count: 1);
            var firstChildOfSecondChild = new FpGrowthNode<string>("Ba", count: 2);
            var secondChildOfSecondChild = new FpGrowthNode<string>("Bb", count: 2);

            secondChild.AddChild(firstChildOfSecondChild);
            secondChild.AddChild(secondChildOfSecondChild);

            tree.AddChild(firstChild);
            tree.AddChild(secondChild);

            var expectedPath = new List<FpGrowthNode<string>> { secondChild, tree };

            // When
            var pathToBb = secondChildOfSecondChild.GetPathToSelf(false, true);

            // Then
            CollectionAssert.AreEqual(pathToBb, expectedPath);
        }

        [Test]
        public void TestGettingPathToSelfWithoutSelfWithoutRoot()
        {
            // Given
            var tree = new FpGrowthNode<string>();
            var firstChild = new FpGrowthNode<string>("A", count: 1);
            var firstChildOfFirstChild = new FpGrowthNode<string>("Aa", count: 2);
            var secondChildOfFirstChild = new FpGrowthNode<string>("Ab", count: 2);

            firstChild.AddChild(firstChildOfFirstChild);
            firstChild.AddChild(secondChildOfFirstChild);

            var secondChild = new FpGrowthNode<string>("B", count: 1);
            var firstChildOfSecondChild = new FpGrowthNode<string>("Ba", count: 2);
            var secondChildOfSecondChild = new FpGrowthNode<string>("Bb", count: 2);

            secondChild.AddChild(firstChildOfSecondChild);
            secondChild.AddChild(secondChildOfSecondChild);

            tree.AddChild(firstChild);
            tree.AddChild(secondChild);

            var expectedPath = new List<FpGrowthNode<string>> { secondChild };

            // When
            var pathToBb = secondChildOfSecondChild.GetPathToSelf();

            // Then
            CollectionAssert.AreEqual(pathToBb, expectedPath);
        }

        [Test]
        public void TestReplaceChildWithKeepingSubtree()
        {
            // Given
            var replacement = new FpGrowthNode<string>("XXX", count: 1);
            var tree = new FpGrowthNode<string>();
            var firstChild = new FpGrowthNode<string>("A", count: 1);
            var firstChildOfFirstChild = new FpGrowthNode<string>("Aa", count: 2);
            var secondChildOfFirstChild = new FpGrowthNode<string>("Ab", count: 2);

            firstChild.AddChild(firstChildOfFirstChild);
            firstChild.AddChild(secondChildOfFirstChild);

            var secondChild = new FpGrowthNode<string>("B", count: 1);
            var firstChildOfSecondChild = new FpGrowthNode<string>("Ba", count: 2);
            var secondChildOfSecondChild = new FpGrowthNode<string>("Bb", count: 2);

            secondChild.AddChild(firstChildOfSecondChild);
            secondChild.AddChild(secondChildOfSecondChild);

            tree.AddChild(firstChild);
            tree.AddChild(secondChild);

            tree.ReplaceChild(firstChild, replacement);

            var expectedPath = new List<FpGrowthNode<string>> { replacement, tree };

            // When
            var actualPath = secondChildOfFirstChild.GetPathToSelf(false, true);

            // Then
            CollectionAssert.AreEqual(expectedPath, actualPath);
        }
    }
}
