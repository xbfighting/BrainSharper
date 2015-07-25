using System;
using BrainSharper.Abstract.Algorithms.DecisionTrees;
using BrainSharper.Abstract.Algorithms.DecisionTrees.Processors;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Data;
using BrainSharper.Implementations.Algorithms.DecisionTrees.Processors;

namespace BrainSharper.Implementations.Algorithms.DecisionTrees
{
    public abstract class MultiSplitDecisionTreeModelBuilder<T> : IDecisionTreeModelBuilder
    {
        /*
        private readonly ISplitQualityChecker<T> _splitQualityChecker;
        private readonly IBinaryBestSplitSelector _binaryBestSplitSelector;
        private readonly ILeafBuilder _leafBuilder;
            */
        public IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams)
        {
            throw new NotImplementedException();
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, int dependentFeatureIndex, IModelBuilderParams additionalParams)
        {
            throw new NotImplementedException();
        }
    
    }
}
