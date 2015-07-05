using System.Collections.Generic;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.MathUtils.ErrorMeasures
{
    public interface IErrorMeasure<TPredictionResult>
    {
        double CalculateError(IDataVector<TPredictionResult> vec1, IDataVector<TPredictionResult> vec2);
        double CalculateError(IList<TPredictionResult> vec1, IList<TPredictionResult> vec2);
    }
}
