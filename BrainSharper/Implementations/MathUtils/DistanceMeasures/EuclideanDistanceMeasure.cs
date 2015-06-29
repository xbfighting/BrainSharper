using BrainSharper.Abstract.Data;
using BrainSharper.Abstract.MathUtils.DistanceMeaseures;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Implementations.MathUtils.DistanceMeasures
{
    public class EuclideanDistanceMeasure : IDistanceMeasure
    {
        public double Distance(Vector<double> vec1, Vector<double> vec2)
        {
            return MathNet.Numerics.Distance.Euclidean(vec1, vec2);
        }

        public double Distance(IDataVector<double> vec1, IDataVector<double> vec2)
        {
            return Distance(vec1.NumericVector, vec2.NumericVector);
        }
    }
}
