using System.Collections.Generic;
using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.MathUtils.ErrorMeasures;
using BrainSharper.General.DataQuality;

namespace BrainSharper.Implementations.MathUtils.ErrorMeasures
{
    public class ClassificationAccuracyError<TPredictionResult> : IErrorMeasure<TPredictionResult>
    {
        public double CalculateError(IDataVector<TPredictionResult> vec1, IDataVector<TPredictionResult> vec2)
        {
            var confusionMatrix = new ConfusionMatrix<TPredictionResult>(vec1, vec2);
            return 1 - confusionMatrix.Accuracy;
        }

        public double CalculateError(IList<TPredictionResult> vec1, IList<TPredictionResult> vec2)
        {
            var confusionMatrix = new ConfusionMatrix<TPredictionResult>(vec1, vec2);
            return 1 - confusionMatrix.Accuracy;
        }
    }
}