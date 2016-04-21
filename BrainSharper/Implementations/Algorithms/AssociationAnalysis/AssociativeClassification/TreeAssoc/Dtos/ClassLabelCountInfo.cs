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

        public static ClassLabelCountInfo<TValue> FromOther(ClassLabelCountInfo<TValue> other)
        {
            return new ClassLabelCountInfo<TValue>(other.ClassLabel, other.Count);
        } 

        public void IncrementCount(int by)
        {
            Count += by;
        }

    }
}
