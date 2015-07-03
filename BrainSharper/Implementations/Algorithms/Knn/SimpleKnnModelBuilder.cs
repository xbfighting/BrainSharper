using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Algorithms.Knn;
using BrainSharper.Abstract.Data;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Implementations.Algorithms.Knn
{
    public class SimpleKnnModelBuilder : IKnnModelBuilder
    {
        public virtual IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams)
        {
            ValidateAdditionalParams(additionalParams);
            var knnParams = (IKnnAdditionalParams) additionalParams;
            Tuple<Matrix<double>, Vector<double>, IList<String>> preparedData = PrepareTrainingData(dataFrame, dependentFeatureName);
            return new KnnPredictionModel(preparedData.Item1, preparedData.Item2, preparedData.Item3, knnParams.KNeighbors, knnParams.UseWeightedDistances);
        }

        public virtual IPredictionModel BuildModel(IDataFrame dataFrame, int dependentFeatureIndex, IModelBuilderParams additionalParams)
        {
            return BuildModel(dataFrame, dataFrame.ColumnNames[dependentFeatureIndex], additionalParams);
        }

        protected virtual void ValidateAdditionalParams(IModelBuilderParams additionalParams)
        {
            if (!(additionalParams is IKnnAdditionalParams))
            {
                throw new ArgumentException("Invalid parameters type!");
            }
        }

        protected virtual Tuple<Matrix<double>, Vector<double>, IList<string>>  PrepareTrainingData(
            IDataFrame dataFrame,
            string dependentFeatureName)
        {
            var dataColumns = dataFrame.ColumnNames.Where(col => col != dependentFeatureName).ToList();
            var trainingData = dataFrame.GetSubsetByColumns(dataColumns).GetAsMatrix();
            var expectedOutcomes = dataFrame.GetNumericColumnVector(dependentFeatureName);
            return new Tuple<Matrix<double>, Vector<double>, IList<string>>(trainingData, expectedOutcomes, dataColumns);
        }
    }
}
