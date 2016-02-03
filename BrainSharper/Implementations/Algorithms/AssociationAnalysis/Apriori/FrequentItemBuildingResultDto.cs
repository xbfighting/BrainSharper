using BrainSharper.Abstract.Algorithms.AssociationAnalysis.DataStructures;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.Apriori
{

    public struct FrequentItemBuildingResultDto<TValue>
    {
        public FrequentItemBuildingResultDto(IFrequentItemsSet<TValue> frequentItemsSet, bool meetsCriteria)
        {
            FrequentItemsSet = frequentItemsSet;
            MeetsCriteria = meetsCriteria;
        }

        public IFrequentItemsSet<TValue> FrequentItemsSet { get; }
        public bool MeetsCriteria { get; }
    }
}
