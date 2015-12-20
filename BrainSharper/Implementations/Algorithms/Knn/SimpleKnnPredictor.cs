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
    public delegate TPredictionResult KnnResultHandler<TPredictionResult>(
        IEnumerable<RowIndexDistanceDto<TPredictionResult>> distances,
        Func<double, double> weightingFunction);

    public class SimpleKnnPredictor<TPredictionResult> : IKnnPredictor<TPredictionResult>
    {
        private readonly KnnResultHandler<TPredictionResult> _resultHandler;

        public SimpleKnnPredictor(
            IDistanceMeasure distanceMeasure,
            IQuantitativeDataNormalizer dataNormalizer,
            KnnResultHandler<TPredictionResult> resultHandlingFunc,
            Func<double, double> weightingFunc = null,
            IDistanceMeasure similarityMeasure = null,
            bool normalizeNumericValues = false)
        {
            _resultHandler = resultHandlingFunc;
            DistanceMeasure = distanceMeasure;
            SimilarityMeasure = similarityMeasure ?? distanceMeasure;
            DataNormalizer = dataNormalizer;
            WeightingFunction = weightingFunc;
            NormalizeNumericValues = normalizeNumericValues;
        }

        public Func<double, double> WeightingFunction { get; }
        public IDistanceMeasure SimilarityMeasure { get; }
        public IQuantitativeDataNormalizer DataNormalizer { get; }
        public IDistanceMeasure DistanceMeasure { get; }
        public bool NormalizeNumericValues { get; set; }

        public IList<TPredictionResult> Predict(IDataFrame queryDataFrame, IPredictionModel model,
            string dependentFeatureName)
        {
            return Predict(queryDataFrame, model, queryDataFrame.ColumnNames.IndexOf(dependentFeatureName));
        }

        public IList<TPredictionResult> Predict(IDataFrame queryDataFrame, IPredictionModel model,
            int dependentFeatureIndex)
        {
            ValidateModel(model);
            var knnModel = model as IKnnPredictionModel<TPredictionResult>;
            var results = new ConcurrentBag<RowIndexDistanceDto<TPredictionResult>>();
            var normalizedData = NormalizeData(queryDataFrame, knnModel, dependentFeatureIndex);
            var normalizedTrainingData = normalizedData.Item1;
            var queryMatrix = normalizedData.Item2;
            Parallel.For(0, queryDataFrame.RowCount, queryRowIdx =>
            {
                var rowVector = queryMatrix.Row(queryRowIdx);
                var distances = new ConcurrentBag<RowIndexDistanceDto<TPredictionResult>>();
                for (var trainingRowIdx = 0; trainingRowIdx < normalizedTrainingData.RowCount; trainingRowIdx++)
                {
                    var trainingRow = normalizedTrainingData.Row(trainingRowIdx);
                    var dependentFeatureValue = knnModel.ExpectedTrainingOutcomes[trainingRowIdx];
                    var distance = DistanceMeasure.Distance(rowVector, trainingRow);
                    var distanceDto = new RowIndexDistanceDto<TPredictionResult>(trainingRowIdx, distance,
                        dependentFeatureValue);
                    distances.Add(distanceDto);
                }
                var sortedDistances = distances.OrderBy(distDto => distDto.Distance).Take(knnModel.KNeighbors);
                var result = new RowIndexDistanceDto<TPredictionResult>(queryRowIdx, 0,
                    _resultHandler(sortedDistances, WeightingFunction));
                results.Add(result);
            });
            return results.OrderBy(res => res.RowIndex).Select(res => res.DependentFeatureValue).ToList();
        }

        public static double FindBestRegressionValue(IEnumerable<RowIndexDistanceDto<double>> distances,
            Func<double, double> weightingFunction)
        {
            {
                var resultTotal = 0.0;
                var weights = 0.0;
                foreach (var dist in distances)
                {
                    var weight = weightingFunction(dist.Distance);
                    resultTotal += dist.DependentFeatureValue*weight;
                    weights += weight;
                }
                return resultTotal/weights;
            }
        }

        public static TPredictionResult VoteForBestCategoricalValue(
            IEnumerable<RowIndexDistanceDto<TPredictionResult>> distances, Func<double, double> weightingFunction)
        {
            var predictionsWithWeights = new Dictionary<TPredictionResult, double>();
            var weights = 0.0;
            foreach (var dist in distances)
            {
                var weight = weightingFunction(dist.Distance);
                if (!predictionsWithWeights.ContainsKey(dist.DependentFeatureValue))
                {
                    predictionsWithWeights.Add(dist.DependentFeatureValue, 0);
                }
                predictionsWithWeights[dist.DependentFeatureValue] += weight;
                weights += weight;
            }

            double highestVotesCount = 0;
            var winningResult = default(TPredictionResult);
            foreach (var result in predictionsWithWeights)
            {
                if (result.Value > highestVotesCount)
                {
                    highestVotesCount = result.Value;
                    winningResult = result.Key;
                }
            }

            return winningResult;
        }

        protected virtual Tuple<Matrix<double>, Matrix<double>> NormalizeData(IDataFrame queryDataFrame,
            IKnnPredictionModel<TPredictionResult> knnModel, int dependentFeatureIdx)
        {
            var modelMatrix = knnModel.TrainingData;
            var queryMatrix = ExtractQueryDataAsMatrix(queryDataFrame, knnModel, dependentFeatureIdx);

            return PerformNormalization(modelMatrix, queryMatrix);
        }

        protected virtual Tuple<Matrix<double>, Matrix<double>> PerformNormalization(Matrix<double> modelMatrix,
            Matrix<double> queryMatrix)
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
            IKnnPredictionModel<TPredictionResult> knnModel,
            int dependentFeatureIdx)
        {
            var dependentFetureName = (dependentFeatureIdx < queryDataFrame.ColumnsCount && dependentFeatureIdx >= 0)
                ? queryDataFrame.ColumnNames[dependentFeatureIdx]
                : string.Empty;
            var queryMatrix = queryDataFrame.GetSubsetByColumns(
                queryDataFrame.ColumnNames.Where(colName => colName != dependentFetureName).ToList()).GetAsMatrix();
            return queryMatrix;
        }

        protected virtual void ValidateModel(IPredictionModel model)
        {
            if (!(model is IKnnPredictionModel<TPredictionResult>))
            {
                throw new ArgumentException("Invalid prediction model passed for KNN predictor!");
            }
        }
    }
}