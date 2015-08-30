namespace BrainSharper.Implementations.Algorithms.RandomForests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Abstract.Algorithms.Infrastructure;
    using Abstract.Algorithms.RandomForests;
    using Abstract.Data;

    public class RandomForestPredictor<TPredictionVal> : IPredictor<TPredictionVal>
    {
        private readonly IPredictor<TPredictionVal> decisionTreePredictor;
        private readonly bool useVotingWeightedByOob;

        public RandomForestPredictor(IPredictor<TPredictionVal> decisionTreePredictor, bool votingWeightedByOobError = false)
        {
            this.decisionTreePredictor = decisionTreePredictor;
            useVotingWeightedByOob = votingWeightedByOobError;
        }

        public IList<TPredictionVal> Predict(IDataFrame queryDataFrame, IPredictionModel model, string dependentFeatureName)
        {
            if (!(model is IRandomForestModel))
            {
                throw new ArgumentException("Invalid model passed to Random Forest Predictor");
            }

            var randomForestModel = (IRandomForestModel)model;

            var weightedPredictons = new Tuple<IList<TPredictionVal>, double>[randomForestModel.DecisionTrees.Count];
            Parallel.For(
                0,
                weightedPredictons.Length,
                i =>
                    {
                        var predictions = decisionTreePredictor.Predict(
                            queryDataFrame,
                            randomForestModel.DecisionTrees[i],
                            dependentFeatureName);
                        var weight = 1 - randomForestModel.OutOfBagErrors[i];
                        weightedPredictons[i] = new Tuple<IList<TPredictionVal>, double>(predictions, weight);
                    });

            var predictionVotes = new Dictionary<int, IDictionary<TPredictionVal, double>>();
            foreach (var weightedPrediction in weightedPredictons)
            {
                for (int rowIdx = 0; rowIdx < queryDataFrame.RowCount; rowIdx++)
                {
                    var predictedVal = weightedPrediction.Item1[rowIdx];
                    var weight = useVotingWeightedByOob ? weightedPrediction.Item2 : 1.0;
                    if (!predictionVotes.ContainsKey(rowIdx))
                    {
                        predictionVotes.Add(rowIdx, new Dictionary<TPredictionVal, double>());
                    }

                    if (!predictionVotes[rowIdx].ContainsKey(predictedVal))
                    {
                        predictionVotes[rowIdx].Add(predictedVal, 0.0);
                    }

                    predictionVotes[rowIdx][predictedVal] += weight;
                }
            }

            var results = predictionVotes.Select(
                rowVotes => rowVotes.Value.OrderByDescending(weightedPredictions => weightedPredictions.Value).First().Key)
                .ToList();

            return results;
        }

        public IList<TPredictionVal> Predict(IDataFrame queryDataFrame, IPredictionModel model, int dependentFeatureIndex)
        {
            return Predict(queryDataFrame, model, queryDataFrame.ColumnNames[dependentFeatureIndex]);
        }
    }
}
