using BrainSharper.Abstract.Data;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Abstract.MathUtils.DistanceMeaseures
{
    public interface IDistanceMeasure
    {
        double Distance(Vector<double> vec1, Vector<double> vec2);
        double Distance(IDataVector<double> vec1, IDataVector<double> vec2);
    }
}
