namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.FPGrowth
{
    public class FpGrowthModel<TValue>
    {
        public FpGrowthModel(FpGrowthHeaderTable<TValue> headerTable, FpGrowthNode<TValue> fpTree)
        {
            HeaderTable = headerTable;
            FpTree = fpTree;
        }

        public FpGrowthHeaderTable<TValue> HeaderTable { get; }
        public FpGrowthNode<TValue> FpTree { get; }
    }
}
