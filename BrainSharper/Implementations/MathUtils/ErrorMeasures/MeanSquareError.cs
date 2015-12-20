using System.Collections.Generic;
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

        public double CalculateError(IList<double> vec1, IList<double> vec2)
        {
            return CalculateError(Vector<double>.Build.DenseOfEnumerable(vec1),
                Vector<double>.Build.DenseOfEnumerable(vec2));
        }
    }
}