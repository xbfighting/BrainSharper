namespace BrainSharper.Implementations.Algorithms.Knn.BackwardsElimination
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.Infrastructure;
    using Abstract.Algorithms.Knn;
    using Abstract.Algorithms.Knn.BackwardsElimination;
    using Abstract.Data;
    using Abstract.MathUtils.DistanceMeaseures;
    using Abstract.MathUtils.Normalizers;

    using MathNet.Numerics.LinearAlgebra;

    public abstract class BackwardsEliminationPredictor<TPredictionResult> : SimpleKnnPredictor<TPredictionResult>
    {
        protected BackwardsEliminationPredictor(
            IDistanceMeasure distanceMeasure, 
            IQuantitativeDataNormalizer dataNormalizer,
            KnnResultHandler<TPredictionResult> resultHandlingFunc, 
            Func<double, double> weightingFunc = null, 
            IDistanceMeasure similarityMeasure = null, 
            bool normalizeNumericValues = false) 
            : base(distanceMeasure, dataNormalizer, resultHandlingFunc, weightingFunc, similarityMeasure, normalizeNumericValues)
        {
        }

        protected override Tuple<Matrix<double>, Matrix<double>> NormalizeData(IDataFrame queryDataFrame, IKnnPredictionModel<TPredictionResult> knnModel, int dependentFeatureIdx)
        {
            var backwardsEliminationModel = knnModel as IBackwardsEliminationKnnModel<TPredictionResult>;
            var featureIndicesToRemove = backwardsEliminationModel.RemovedFeaturesData.Select(f => queryDataFrame.ColumnNames.IndexOf(f.FeatureName)).OrderBy(i => i).ToList();
            var relevantFeatures =
                queryDataFrame.ColumnNames.Where((colName, colIdx) => !featureIndicesToRemove.Contains(colIdx) && colIdx != dependentFeatureIdx).ToList();

            var modelMatrix = GetModelMatrixWithOnlyRelevantColumns(knnModel.TrainingData, featureIndicesToRemove);
            var queryDataFrameWithoutRedundantColumns = queryDataFrame.GetSubsetByColumns(relevantFeatures).GetAsMatrix();
            return base.PerformNormalization(modelMatrix, queryDataFrameWithoutRedundantColumns);
        }

        protected virtual Matrix<double> GetModelMatrixWithOnlyRelevantColumns(
            Matrix<double> modelMatrix,
            IList<int> feturesToRemoveIndices)
        {
            Matrix<double> newModelMatrix = modelMatrix;
            foreach (var featureToRemoveIdx in feturesToRemoveIndices.Reverse())
            {
                newModelMatrix = newModelMatrix.RemoveColumn(featureToRemoveIdx);
            }
            return newModelMatrix;
        }

        protected override void ValidateModel(IPredictionModel model)
        {
            if (!(model is IBackwardsEliminationKnnModel<TPredictionResult>))
            {
                throw new ArgumentException("Invalid model passed to Backwards Elimination KNN Predictor!");
            }
        }
    }
}
