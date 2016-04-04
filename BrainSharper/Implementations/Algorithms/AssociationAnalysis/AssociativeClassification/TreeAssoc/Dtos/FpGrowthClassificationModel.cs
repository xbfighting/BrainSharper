using BrainSharper.Implementations.Algorithms.AssociationAnalysis.DataStructures.FPGrowth;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.TreeAssoc.Dtos
{
    public class FpGrowthClassificationModel<TValue, TClassLabel> : FpGrowthModel<TValue>
    {
        public FpGrowthClassificationModel(
            FpGrowthHeaderTable<TValue> headerTable, 
            ClassificationFpGrowthNode<TValue, TClassLabel> fpTree) : base(headerTable, fpTree)
        {
            ClassificationTree = fpTree;
        }

        ClassificationFpGrowthNode<TValue, TClassLabel> ClassificationTree { get; }
    }
}
