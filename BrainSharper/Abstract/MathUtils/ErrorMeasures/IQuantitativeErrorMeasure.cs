using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Abstract.MathUtils.ErrorMeasures
{
    public interface IQuantitativeErrorMeasure : IErrorMeasure<double>
    {
        double CalculateError(Vector<double> vec1, Vector<double> vec2);
    }
}
