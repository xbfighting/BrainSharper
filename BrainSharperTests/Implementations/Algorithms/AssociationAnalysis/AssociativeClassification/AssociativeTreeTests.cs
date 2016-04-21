using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.Common;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.TreeAssoc;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.TreeAssoc.Dtos;
using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.FPGrowth;
using NUnit.Framework;

namespace BrainSharperTests.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification
{
    [TestFixture]
    public class AssociativeTreeTests
    {
        AssociationTreeBuilder<string> Subject = new AssociationTreeBuilder<string>();
            
        [Test]
        public void TestBuildingClassDistributionForInitialItems()
        {
            // Given
            var data = AssociationAnalysisTestDataBuilder.AbstractCMARDataSetOnlyFrequentItems;
            var miningParams = new ClassificationAssociationMiningParams("label", 0.4, null, 0.5);

            var expectedNodes = new[]
            {
                null,
                "Feature: A Value: a1",
                "Feature: D Value: d3",
                "Feature: C Value: c1",
                "Feature: B Value: b2",
                "Feature: C Value: c1",
                "Feature: D Value: d3",
                "Feature: D Value: d3"
            };

            var expectedClassificationNodes = new[]
            {
                null,
                "Feature: D Value: d3;A=>1",
                "Feature: C Value: c1;A=>1",
                "Feature: C Value: c1;B=>1",
                "Feature: D Value: d3;C=>1",
                "Feature: D Value: d3;C=>1"
            };

            // When
            var result = Subject.InsertTransactionsIntoTree(data, miningParams,
                new Dictionary<IDataItem<string>, IList<FpGrowthNode<IDataItem<string>>>>());

            var gatheredNodes = new List<string>();
            var classificationNodes = new List<string>();

            var childrenBreadthFirst = new Queue<FpGrowthNode<IDataItem<string>>>();
            childrenBreadthFirst.Enqueue(result);
            var currentNode = result;
            while (childrenBreadthFirst.Any())
            {
                var head = childrenBreadthFirst.Dequeue();
                if (head is ClassificationFpGrowthNode<IDataItem<string>, string>)
                {
                    var classificationHead = head as ClassificationFpGrowthNode<IDataItem<string>, string>;
                    classificationNodes.Add(
                        $"{head.Value};{string.Join("", string.Join(",", classificationHead.ClassLabelDistributions.Select(kvp => $"{kvp.Key}=>{kvp.Value.Count}")))}");

                }
                gatheredNodes.Add(head.Value?.ToString());
                if(head.HasChildren) head.Children.ToList().ForEach(childrenBreadthFirst.Enqueue);
            }

            // Then
            CollectionAssert.AreEquivalent(expectedNodes, gatheredNodes);
            CollectionAssert.AreEquivalent(classificationNodes, classificationNodes);
        }

        [Test]
        public void FindingFrequentItems()
        {
            var data = AssociationAnalysisTestDataBuilder.AbstractCMARDataSetOnlyFrequentItems;
            var miningParams = new ClassificationAssociationMiningParams("label", 0.4, null, 0.5);

            var result = Subject.FindFrequentItems(data, miningParams);
        }
    }
}
