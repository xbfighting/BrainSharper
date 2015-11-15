namespace BrainSharper.General.DataUtils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Abstract.Algorithms.Infrastructure;
    using Abstract.Data;
    using DataQuality;

    using MathNet.Numerics.LinearRegression;

    using Utils;

    public interface ICrossValidator<TPredictionResult>
    {
        IList<IDataQualityReport<TPredictionResult>> CrossValidate(
            IPredictionModelBuilder modelBuilder,
            IModelBuilderParams modelBuilderParams,
            IPredictor<TPredictionResult> predictor,
            IDataQualityMeasure<TPredictionResult> qualityMeasure,
            IDataFrame dataFrame,
            string dependentFeatureName,
            double percetnagOfTrainData,
            int folds);
    }

    public class CrossValidator<TPredictionResult> : ICrossValidator<TPredictionResult>
    {
        private readonly Random _randomizer;

        public CrossValidator(Random randomizer = null)
        {
            _randomizer = randomizer;
        }

        public IList<IDataQualityReport<TPredictionResult>> CrossValidate(
            IPredictionModelBuilder modelBuilder, 
            IModelBuilderParams modelBuilderParams, 
            IPredictor<TPredictionResult> predictor,
            IDataQualityMeasure<TPredictionResult> qualityMeasure,
            IDataFrame dataFrame,
            string dependentFeatureName,
            double percetnagOfTrainData,
            int folds)
        {
            var trainingDataCount = (int)Math.Round(percetnagOfTrainData * dataFrame.RowCount);
            var testDataCount = dataFrame.RowCount - trainingDataCount;
            var shuffledAllIndices = dataFrame.RowIndices.Shuffle(_randomizer);
            var maxWindowsCount = dataFrame.RowCount / testDataCount;

            var iterationAccuracies = new List<IDataQualityReport<TPredictionResult>>();
            var currentWindowNo = 0;
            for (var i = 0; i < folds; i++)
            {
                if (currentWindowNo == maxWindowsCount)
                {
                    currentWindowNo = 0;
                    shuffledAllIndices = shuffledAllIndices.Shuffle();
                }
                var offset = currentWindowNo * testDataCount;
                var trainingIndices = shuffledAllIndices.Skip(offset).Take(trainingDataCount).ToList();
                var trainingData = dataFrame.GetSubsetByRows(trainingIndices);

                var testIndices = shuffledAllIndices.Except(trainingIndices).ToList();
                var testData = dataFrame.GetSubsetByRows(testIndices);
                IPredictionModel model = modelBuilder.BuildModel(trainingData, dependentFeatureName, modelBuilderParams);
                IList<TPredictionResult> predictions = predictor.Predict(testData, model, dependentFeatureName);
                IList<TPredictionResult> expected = testData.GetColumnVector<TPredictionResult>(dependentFeatureName);
                IDataQualityReport<TPredictionResult> qualityReport = qualityMeasure.GetReport(expected, predictions);
                iterationAccuracies.Add(qualityReport);
                currentWindowNo++;
            }
           return iterationAccuracies;
        }
    }
}
