namespace BrainSharper.Implementations.Algorithms.RandomForests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Abstract.Algorithms.Infrastructure;
    using Abstract.Data;

    using BrainSharper.Abstract.Algorithms.RandomForests;

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
                        var weight = randomForestModel.OutOfBagErrors[i];
                        weightedPredictons[i] = new Tuple<IList<TPredictionVal>, double>(predictions, weight);
                    });

            throw new System.NotImplementedException();
        }

        public IList<TPredictionVal> Predict(IDataFrame queryDataFrame, IPredictionModel model, int dependentFeatureIndex)
        {
            return Predict(queryDataFrame, model, queryDataFrame.ColumnNames[dependentFeatureIndex]);
        }
    }
}
