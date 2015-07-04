using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Algorithms.Knn;
using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.MathUtils.ErrorMeasures;
using BrainSharper.Abstract.MathUtils.Normalizers;
using BrainSharper.Implementations.Data;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Implementations.Algorithms.Knn
{
    public class BackwardsEliminationKnnModelBuilder : SimpleKnnModelBuilder
    {
        private readonly IQuantitativeDataNormalizer _dataNormalizer;
        private readonly IKnnPredictor _knnPredictor;
        private readonly IQuantitativeErrorMeasure _errorMeasure;

        public BackwardsEliminationKnnModelBuilder(IQuantitativeDataNormalizer dataNormalizer, IKnnPredictor knnPredictor, IQuantitativeErrorMeasure errorMeasure)
        {
            _dataNormalizer = dataNormalizer;
            _knnPredictor = knnPredictor;
            _errorMeasure = errorMeasure;
        }

        public override IPredictionModel BuildModel(IDataFrame dataFrame, string dependentFeatureName, IModelBuilderParams additionalParams)
        {
            ValidateAdditionalParams(additionalParams);
            return PerformBackwardsElimination(dataFrame, dependentFeatureName, additionalParams as IKnnAdditionalParams);
        }

        protected IBackwardsEliminationKnnModel PerformBackwardsElimination(IDataFrame dataFrame, string dependentFeatureName, IKnnAdditionalParams additionalParams)
        {
            Tuple<Matrix<double>, Vector<double>, IList<string>> preparedData = PrepareTrainingData(dataFrame, dependentFeatureName);
            var dataColumnsNames = preparedData.Item3;
            var trainingData = _dataNormalizer.NormalizeColumns(preparedData.Item1);
            var expectedValues = preparedData.Item2;
            _knnPredictor.NormalizeNumericValues = false;
            double baseErrorRate = ProcessDataAndQuantifyErrorRate(
                dependentFeatureName,
                trainingData,
                expectedValues,
                dataColumnsNames,
                additionalParams);

            var actualDataColumnNames = new List<string>(dataColumnsNames);
            var anyFeatureRemovedInThisIteration = true;
            var removedFeaturesInfo = new List<IBackwardsEliminationRemovedFeatureData>();
            while (anyFeatureRemovedInThisIteration)
            {
                anyFeatureRemovedInThisIteration = false;
                var candidateFeaturesToEliminate = new Dictionary<int, double>();
                foreach (var columnIdx in Enumerable.Range(0, actualDataColumnNames.Count))
                {
                    var newFeatureNames = new List<string>(actualDataColumnNames);
                    newFeatureNames.RemoveAt(columnIdx);

                    var trainingDataWithoutColumn = trainingData.RemoveColumn(columnIdx);
                    var newDataPredictionError = ProcessDataAndQuantifyErrorRate(
                        dependentFeatureName,
                        trainingDataWithoutColumn, 
                        expectedValues, 
                        newFeatureNames,
                        additionalParams);
                    if (newDataPredictionError < baseErrorRate)
                    {
                        var errorGain = baseErrorRate - newDataPredictionError;
                        candidateFeaturesToEliminate.Add(columnIdx, errorGain);
                    }
                }
                if (!candidateFeaturesToEliminate.Any())
                {
                    break;
                }
                var bestFeatureToRemove = candidateFeaturesToEliminate.OrderBy(kvp => kvp.Value).First();
                anyFeatureRemovedInThisIteration = true;
                removedFeaturesInfo.Add(new BackwardsEliminationRemovedFeatureData(bestFeatureToRemove.Value, actualDataColumnNames[bestFeatureToRemove.Key]));
                actualDataColumnNames.RemoveAt(bestFeatureToRemove.Key);
            }

            return new BackwardsEliminationKnnModel(
                preparedData.Item1,
                expectedValues,
                dataColumnsNames,
                additionalParams.KNeighbors,
                additionalParams.UseWeightedDistances,
                removedFeaturesInfo); // TODO: later add calculation of best known accuracy rate
        }

        private double ProcessDataAndQuantifyErrorRate(
            string dependentFeatureName, 
            Matrix<double> trainingData, 
            Vector<double> expectedValues,
            IList<string> dataColumnsNames,
            IKnnAdditionalParams knnAdditionalParams)
        {
            var partialResults = new ConcurrentDictionary<int, double>();
            ProcessData(dependentFeatureName, trainingData, expectedValues, dataColumnsNames, partialResults, knnAdditionalParams);
            var actualValues = partialResults.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value).ToList();
            return _errorMeasure.CalculateError(expectedValues, actualValues);
        }

        private void ProcessData(
            string dependentFeatureName, 
            Matrix<double> trainingData, 
            Vector<double> expectedValues,
            IList<string> dataColumnNames, 
            ConcurrentDictionary<int, double> partialResults,
            IKnnAdditionalParams knnAdditionalParams)
        {
            Parallel.For(0, trainingData.RowCount, rowIdx =>
            {
                var trainingDataExceptRow = trainingData.RemoveRow(rowIdx);
                var expectedValuesExceptRow = expectedValues.ToList();
                expectedValuesExceptRow.RemoveAt(rowIdx);
                var expectedValuesVector = Vector<double>.Build.DenseOfEnumerable(expectedValuesExceptRow);

                var queryMatrix = Matrix<double>.Build.DenseOfRowVectors(trainingData.Row(rowIdx));
                var queryDataFrame = new DataFrame(queryMatrix);
                var knnPredictionModel = new KnnPredictionModel(trainingDataExceptRow, expectedValuesVector,
                    dataColumnNames, 3, true);
                var results = _knnPredictor.Predict(
                    queryDataFrame, 
                    knnPredictionModel,
                    dependentFeatureName);
                double result = results.First();
                partialResults.AddOrUpdate(rowIdx, result, (i, d) => result);
            });
        }
    }
}
