using System;
using System.Linq;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Algorithms.Knn;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Algorithms.Knn
{
    public class BaseKnnModelBuilder : IKnnModelBuilder
    {
        public BaseKnnModelBuilder()
        {
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams)
        {
            if (!(additionalParams is IKnnAdditionalParams))
            {
                throw new ArgumentException("Invalid parameters type!");
            }
            var knnParams = additionalParams as IKnnAdditionalParams;
            var dataColumns = dataFrame.ColumnNames.Where(col => col != dependentFeatureName).ToList();
            var trainingData = dataFrame.GetSubsetByColumns(dataColumns).GetAsMatrix();
            var expectedOutcomes = dataFrame.GetNumericColumnVector(dependentFeatureName);
            return new KnnPredictionModel(trainingData, expectedOutcomes, knnParams.KNeighbors, knnParams.UseWeightedDistances);
        }

        public IPredictionModel BuildModel(IDataFrame dataFrame, int dependentFeatureIndex, IModelBuilderParams additionalParams)
        {
            return BuildModel(dataFrame, dataFrame.ColumnNames[dependentFeatureIndex], additionalParams);
        }
    }
}
