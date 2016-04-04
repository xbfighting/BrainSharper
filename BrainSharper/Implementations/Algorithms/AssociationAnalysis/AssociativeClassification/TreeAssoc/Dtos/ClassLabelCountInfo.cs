namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.TreeAssoc.Dtos
{
    public class ClassLabelCountInfo<TValue>
    {
        public ClassLabelCountInfo(TValue classLabel, int count)
        {
            ClassLabel = classLabel;
            Count = count;
        }

        public TValue ClassLabel { get; }
        public int Count { get; private set; }

        public void IncrementCount(int by)
        {
            Count += by;
        }

    }
}
