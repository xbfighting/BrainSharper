using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis;
using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori
{
    public class DissociationRulesFinder<TValue>
    {
        private readonly IFrequentItemsFinder<TValue> _frequentItemsFinder;

        public DissociationRulesFinder(IFrequentItemsFinder<TValue> frequentItemsFinder)
        {
            _frequentItemsFinder = frequentItemsFinder;
        }

        //TODO: [Dissoc] later change return type to some fancy DTO
        //TODO: [Dissoc] make apriori result contain NEGATIVE BOUNDARY!!! Constructed during combination of items
        //TODO: [Dissoc] extend FrequetItems to carry information about negative boundary
        public IList<IDissociativeRule<TValue>> FindDissociativeRules(
            ITransactionsSet<TValue> transactionsSet,
            IDissociationRulesMiningParams miningParams)
        {
            var frequentItemsSearchResults = _frequentItemsFinder.FindFrequentItems(transactionsSet, miningParams);

            var negativeBoundary = new List<IFrequentItemsSet<TValue>>();
            foreach (var itemsSize in frequentItemsSearchResults.FrequentItemsSizes.Where(size => size > 1))
            {
                var itemsOfGivenSize = frequentItemsSearchResults[itemsSize];
                
            }

            return null;
        }

        private struct ItemsSetPair
        {
            public ItemsSetPair(IFrequentItemsSet<TValue> left, IFrequentItemsSet<TValue> right)
            {
                Left = left;
                Right = right;
            }

            public IFrequentItemsSet<TValue> Left { get; }
            public IFrequentItemsSet<TValue> Right { get; }
        }

    }
}
