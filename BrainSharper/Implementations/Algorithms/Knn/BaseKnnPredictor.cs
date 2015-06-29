using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Algorithms.Knn;
using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.MathUtils.DistanceMeaseures;

namespace BrainSharper.Implementations.Algorithms.Knn
{
    public class BaseKnnPredictor : IKnnPredictor
    {
        public BaseKnnPredictor(IDistanceMeasure distanceMeasure)
        {
            DistanceMeasure = distanceMeasure;
        }

        public IDistanceMeasure DistanceMeasure { get; }

        public IList<double> Predict(IDataFrame queryDataFrame, IPredictionModel model, string dependentFeatureName)
        {
            return Predict(queryDataFrame, model, queryDataFrame.ColumnNames.IndexOf(dependentFeatureName));
        }

        public IList<double> Predict(IDataFrame queryDataFrame, IPredictionModel model, int dependentFeatureIndex)
        {
            ValidateModel(model);
            var knnModel = model as IKnnPredictionModel;
            var results = new ConcurrentBag<RowIndexDistanceDto>();
            var queryData = queryDataFrame;
            // TODO: add checking query frame length - if it has empty value column
            //Parallel.For(0, queryDataFrame.RowCount, queryRowIdx =>
            for(int queryRowIdx = 0; queryRowIdx < queryDataFrame.RowCount; queryRowIdx++)
            {
                var rowVector = queryDataFrame.GetNumericRowVector(queryRowIdx);
                var distances = new ConcurrentBag<RowIndexDistanceDto>();
                // Parallel.For(0, knnModel.TrainingData.RowCount, trainingRowIdx =>
                for (int trainingRowIdx = 0; trainingRowIdx < knnModel.TrainingData.RowCount; trainingRowIdx++)

                {
                    var trainingRow = knnModel.TrainingData.Row(trainingRowIdx);
                    double dependentFeatureValue = knnModel.ExpectedTrainingOutcomes[trainingRowIdx];
                    double distance = DistanceMeasure.Distance(rowVector, trainingRow);
                    var distanceDto = new RowIndexDistanceDto(trainingRowIdx, distance, dependentFeatureValue);
                    distances.Add(distanceDto);
                }//);
                var sortedDistances = distances.OrderBy(distDto => distDto.Distance).Take(knnModel.KNeighbors);
                var result = new RowIndexDistanceDto(queryRowIdx, 0, FindResult(sortedDistances));
                results.Add(result);
            }//);
            return results.OrderBy(res => res.RowIndex).Select(res => res.DependentFeatureValue).ToList();
        }

        protected virtual double FindResult(IEnumerable<RowIndexDistanceDto> distances)
        {
            var resultTotal = 0.0;
            var weights = 0.0;
            foreach (var dist in distances)
            {
                resultTotal += dist.DependentFeatureValue * dist.Distance;
                weights += dist.Distance;
            }
            return resultTotal/weights;
        }

        private static void ValidateModel(IPredictionModel model)
        {
            if (!(model is IKnnPredictionModel))
            {
                throw new ArgumentException("Invalid prediction model passed for KNN predictor!");
            }
        }
    }
}
