using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.MathUtils.ErrorMeasures;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Implementations.MathUtils.ErrorMeasures
{
    public class MeanSquareError : IQuantitativeErrorMeasure
    {
        public double CalculateError(Vector<double> vec1, Vector<double> vec2)
        {
            return Distance.MSE(vec1, vec2);
        }

        public double CalculateError(IDataVector<double> vec1, IDataVector<double> vec2)
        {
            return CalculateError(vec1.NumericVector, vec2.NumericVector);
        }
    }
}
