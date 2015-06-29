using BrainSharper.Abstract.Data;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Abstract.MathUtils.ErrorMeasures
{
    public interface IQuantitativeErrorMeasure
    {
        double CalculateError(Vector<double> vec1, Vector<double> vec2);
        double CalculateError(IDataVector<double> vec1, IDataVector<double> vec2);
    }
}
