using System;
using System.Collections.Generic;
using System.Linq;
using BrainSharper.Abstract.Data;
using BrainSharper.General.Utils;

namespace BrainSharper.General.DataUtils
{
    public enum DataType
    {
        Training,
        Testing
    }

    /// <summary>
    ///     Splits data into training and testing set, keeping specified proportions
    /// </summary>
    public interface ITrainTestSplitter
    {
        IDictionary<DataType, IList<int>> SplitData(IDataFrame dataFrame, double trainingProportion);
    }

    public class StandardSplitter : ITrainTestSplitter
    {
        private readonly Random _randomier;

        public StandardSplitter(Random randomier)
        {
            _randomier = randomier;
        }

        public IDictionary<DataType, IList<int>> SplitData(IDataFrame dataFrame, double trainingProportion)
        {
            var trainingDataCount = (int) (dataFrame.RowCount*trainingProportion);
            var shuffledIndices = Enumerable.Range(0, dataFrame.RowCount).ToList().Shuffle(_randomier);
            var trainingIndices = shuffledIndices.Take(trainingDataCount).ToList();
            var testingIndices = shuffledIndices.Skip(trainingDataCount).ToList();
            return new Dictionary<DataType, IList<int>>
            {
                [DataType.Training] = trainingIndices,
                [DataType.Testing] = testingIndices
            };
        }
    }
}