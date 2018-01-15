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
        public IDistanceMeasure DistanceMeasure { get; }

        public bool NormalizeNumericValues { get; set; }

        public IDistanceMeasure SimilarityMeasure { get; }
        public IQuantitativeDataNormalizer DataNormalizer { get; }

        public static double FindBestRegressionValue(IEnumerable<RowIndexDistanceDto<double>> distances, Func<double, double> weightingFunction)
        {
            {
                var resultTotal = 0.0;
                var weights = 0.0;
                foreach (var dist in distances)
                {
                    var weight = weightingFunction(dist.Distance);
                    resultTotal += dist.DependentFeatureValue * weight;
                    weights += weight;
                }
                return resultTotal / weights;
            }
        }

        public static TPredictionResult VoteForBestCategoricalValue(IEnumerable<RowIndexDistanceDto<TPredictionResult>> distances, Func<double, double> weightingFunction)
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
            TPredictionResult winningResult = default(TPredictionResult);
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

        /// <summary>
        /// 预测
        /// </summary>
        /// <param name="queryDataFrame">被预测数据</param>
        /// <param name="model">预测模型</param>
        /// <param name="dependentFeatureName">依赖特征名称</param>
        /// <returns></returns>
        public IList<TPredictionResult> Predict(IDataFrame queryDataFrame, IPredictionModel model, string dependentFeatureName)
        {
            return Predict(queryDataFrame, model, queryDataFrame.ColumnNames.IndexOf(dependentFeatureName));
        }

        /// <summary>
        /// 预测
        /// </summary>
        /// <param name="queryDataFrame">被预测数据</param>
        /// <param name="model">预测模型</param>
        /// <param name="dependentFeatureIndex">依赖特征索引</param>
        /// <returns></returns>
        public IList<TPredictionResult> Predict(IDataFrame queryDataFrame, IPredictionModel model, int dependentFeatureIndex)
        {
            ValidateModel(model);
            var knnModel = model as IKnnPredictionModel<TPredictionResult>;
            var results = new ConcurrentBag<RowIndexDistanceDto<TPredictionResult>>(); // 线程安全的

            // 数据规范化
            var normalizedData =  NormalizeData(queryDataFrame, knnModel, dependentFeatureIndex);
            var normalizedTrainingData = normalizedData.Item1;
            var queryMatrix = normalizedData.Item2;

            // 并行
            Parallel.For(0, queryDataFrame.RowCount, queryRowIdx =>
            {
                var rowVector = queryMatrix.Row(queryRowIdx);
                var distances = new ConcurrentBag<RowIndexDistanceDto<TPredictionResult>>();
                for (int trainingRowIdx = 0; trainingRowIdx < normalizedTrainingData.RowCount; trainingRowIdx++)
                {
                    var trainingRow = normalizedTrainingData.Row(trainingRowIdx);
                    TPredictionResult dependentFeatureValue = knnModel.ExpectedTrainingOutcomes[trainingRowIdx];
                    double distance = DistanceMeasure.Distance(rowVector, trainingRow);
                    var distanceDto = new RowIndexDistanceDto<TPredictionResult>(trainingRowIdx, distance, dependentFeatureValue);
                    distances.Add(distanceDto);
                }

                // TODO:理解
                var sortedDistances = distances.OrderBy(distDto => distDto.Distance).Take(knnModel.KNeighbors);
                var result = new RowIndexDistanceDto<TPredictionResult>(queryRowIdx, 0, _resultHandler(sortedDistances, WeightingFunction));
                results.Add(result);
            });
            return results.OrderBy(res => res.RowIndex).Select(res => res.DependentFeatureValue).ToList();
        }

        /// <summary>
        /// 数据规范化
        /// </summary>
        /// <param name="queryDataFrame">被预测数据</param>
        /// <param name="knnModel">knn 预测模型</param>
        /// <param name="dependentFeatureIdx">依赖特征索引</param>
        /// <returns></returns>
        protected virtual Tuple<Matrix<double>, Matrix<double>> NormalizeData(IDataFrame queryDataFrame, IKnnPredictionModel<TPredictionResult> knnModel, int dependentFeatureIdx)
        {
            // 获取knn预测矩阵的训练数据
            var modelMatrix = knnModel.TrainingData;
            // 提取被预测数据的非特征矩阵
            var queryMatrix = ExtractQueryDataAsMatrix(queryDataFrame, knnModel, dependentFeatureIdx);

            return PerformNormalization(modelMatrix, queryMatrix);
        }

        /// <summary>
        /// 执行规范化
        /// </summary>
        /// <param name="modelMatrix">knn预测矩阵的训练数据</param>
        /// <param name="queryMatrix">提取被预测数据的非特征矩阵</param>
        /// <returns></returns>
        protected virtual Tuple<Matrix<double>, Matrix<double>> PerformNormalization(Matrix<double> modelMatrix, Matrix<double> queryMatrix)
        {
            if (!NormalizeNumericValues)
            {
                return new Tuple<Matrix<double>, Matrix<double>>(modelMatrix, queryMatrix);
            }

            // 矩阵堆叠
            var commonMatrix = modelMatrix.Stack(queryMatrix);
            var normalizedMatrix = DataNormalizer.NormalizeColumns(commonMatrix);
            var normalizedModelMatrix = normalizedMatrix.SubMatrix(0, modelMatrix.RowCount, 0, modelMatrix.ColumnCount);
            var normalizedQueryMatrix = normalizedMatrix.SubMatrix(modelMatrix.RowCount, queryMatrix.RowCount, 0,
                queryMatrix.ColumnCount);
            return new Tuple<Matrix<double>, Matrix<double>>(normalizedModelMatrix, normalizedQueryMatrix);
        }

        /// <summary>
        /// 提取被预测数据的非特征矩阵
        /// </summary>
        /// <param name="queryDataFrame">被预测数据</param>
        /// <param name="knnModel">knn 预测模型</param>
        /// <param name="dependentFeatureIdx">依赖特征索引</param>
        /// <returns></returns>
        protected virtual Matrix<double> ExtractQueryDataAsMatrix(IDataFrame queryDataFrame, IKnnPredictionModel<TPredictionResult> knnModel, int dependentFeatureIdx)
        {
            // 获取被预测模型的 特征列
            var dependentFetureName = (dependentFeatureIdx < queryDataFrame.ColumnsCount && dependentFeatureIdx >= 0)
                ? queryDataFrame.ColumnNames[dependentFeatureIdx]
                : string.Empty;
            // 获取被预测模型的子集（排除特征列）
            var queryMatrix = queryDataFrame.GetSubsetByColumns(
                queryDataFrame.ColumnNames.Where(colName => colName != dependentFetureName).ToList()).GetAsMatrix();
            return queryMatrix;
        }

        /// <summary>
        /// 验证是否是特定的预测模型
        /// </summary>
        /// <param name="model"></param>
        /// <exception cref="ArgumentException">Invalid prediction model passed for KNN predictor!</exception>
        protected virtual void ValidateModel(IPredictionModel model)
        {
            if (!(model is IKnnPredictionModel<TPredictionResult>))
            {
                throw new ArgumentException("Invalid prediction model passed for KNN predictor!");
            }
        }
    }
}
