using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Algorithms.Knn;
using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.MathUtils.DistanceMeaseures;
using BrainSharper.Abstract.MathUtils.Normalizers;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Implementations.Algorithms.Knn
{
    public class SimpleKnnPredictor : IKnnPredictor
    {
        public SimpleKnnPredictor(
            IDistanceMeasure distanceMeasure, 
            IQuantitativeDataNormalizer dataNormalizer, 
            Func<double, double> weightingFunc = null,
            IDistanceMeasure similarityMeasure = null,
            bool normalizeNumericValues = false)
        {
            DistanceMeasure = distanceMeasure;
            SimilarityMeasure = similarityMeasure ?? distanceMeasure;
            DataNormalizer = dataNormalizer;
            WeightingFunction = weightingFunc;
            NormalizeNumericValues = normalizeNumericValues;
        }

        public Func<double, double> WeightingFunction { get; } 
        public IDistanceMeasure DistanceMeasure { get; }

        public bool NormalizeNumericValues { get; set; }

        public IDistanceMeasure SimilarityMeasure { get; }
        public IQuantitativeDataNormalizer DataNormalizer { get; }


        public IList<double> Predict(IDataFrame queryDataFrame, IPredictionModel model, string dependentFeatureName)
        {
            return Predict(queryDataFrame, model, queryDataFrame.ColumnNames.IndexOf(dependentFeatureName));
        }

        public IList<double> Predict(IDataFrame queryDataFrame, IPredictionModel model, int dependentFeatureIndex)
        {
            ValidateModel(model);
            var knnModel = model as IKnnPredictionModel;
            var results = new ConcurrentBag<RowIndexDistanceDto>();
            var normalizedData =  NormalizeData(queryDataFrame, knnModel, dependentFeatureIndex);
            var normalizedTrainingData = normalizedData.Item1;
            var queryMatrix = normalizedData.Item2;
            Parallel.For(0, queryDataFrame.RowCount, queryRowIdx =>
            {
                var rowVector = queryMatrix.Row(queryRowIdx);
                var distances = new ConcurrentBag<RowIndexDistanceDto>();
                for (int trainingRowIdx = 0; trainingRowIdx < normalizedTrainingData.RowCount; trainingRowIdx++)
                {
                    var trainingRow = normalizedTrainingData.Row(trainingRowIdx);
                    double dependentFeatureValue = knnModel.ExpectedTrainingOutcomes[trainingRowIdx];
                    double distance = DistanceMeasure.Distance(rowVector, trainingRow);
                    var distanceDto = new RowIndexDistanceDto(trainingRowIdx, distance, dependentFeatureValue);
                    distances.Add(distanceDto);
                }
                var sortedDistances = distances.OrderBy(distDto => distDto.Distance).Take(knnModel.KNeighbors);
                var result = new RowIndexDistanceDto(queryRowIdx, 0, FindResult(sortedDistances));
                results.Add(result);
            });
            return results.OrderBy(res => res.RowIndex).Select(res => res.DependentFeatureValue).ToList();
        }

        protected virtual Tuple<Matrix<double>, Matrix<double>> NormalizeData(IDataFrame queryDataFrame, IKnnPredictionModel knnModel, int dependentFeatureIdx)
        {
            var modelMatrix = knnModel.TrainingData;
            var queryMatrix = ExtractQueryDataAsMatrix(queryDataFrame, knnModel, dependentFeatureIdx);

            return PerformNormalization(modelMatrix, queryMatrix);
        }

        protected virtual Tuple<Matrix<double>, Matrix<double>> PerformNormalization(Matrix<double> modelMatrix, Matrix<double> queryMatrix)
        {
            if (!NormalizeNumericValues)
            {
                return new Tuple<Matrix<double>, Matrix<double>>(modelMatrix, queryMatrix);
            }

            var commonMatrix = modelMatrix.Stack(queryMatrix);
            var normalizedMatrix = DataNormalizer.NormalizeColumns(commonMatrix);
            var normalizedModelMatrix = normalizedMatrix.SubMatrix(0, modelMatrix.RowCount, 0, modelMatrix.ColumnCount);
            var normalizedQueryMatrix = normalizedMatrix.SubMatrix(modelMatrix.RowCount, queryMatrix.RowCount, 0,
                queryMatrix.ColumnCount);
            return new Tuple<Matrix<double>, Matrix<double>>(normalizedModelMatrix, normalizedQueryMatrix);
        }

        protected virtual Matrix<double> ExtractQueryDataAsMatrix(
            IDataFrame queryDataFrame, 
            IKnnPredictionModel knnModel,
            int dependentFeatureIdx)
        {
            var dependentFetureName = (dependentFeatureIdx < queryDataFrame.ColumnsCount && dependentFeatureIdx >= 0)
                ? queryDataFrame.ColumnNames[dependentFeatureIdx]
                : string.Empty;
            var queryMatrix = queryDataFrame.GetSubsetByColumns(
                queryDataFrame.ColumnNames.Where(colName => colName != dependentFetureName).ToList()).GetAsMatrix();
            return queryMatrix;
        }

        protected virtual double FindResult(IEnumerable<RowIndexDistanceDto> distances)
        {
            var resultTotal = 0.0;
            var weights = 0.0;
            foreach (var dist in distances)
            {
                var weight = WeightingFunction(dist.Distance);
                resultTotal += dist.DependentFeatureValue*weight;
                weights += weight;
            }
            return resultTotal/weights;
        }

        protected virtual void ValidateModel(IPredictionModel model)
        {
            if (!(model is IKnnPredictionModel))
            {
                throw new ArgumentException("Invalid prediction model passed for KNN predictor!");
            }
        }
    }
}
