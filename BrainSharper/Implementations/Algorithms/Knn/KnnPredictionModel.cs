using BrainSharper.Abstract.Algorithms.Knn;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Implementations.Algorithms.Knn
{
    public class KnnPredictionModel : IKnnPredictionModel
    {
        public KnnPredictionModel(Matrix<double> database)
        {
            Database = database;
        }

        public Matrix<double> Database { get; private set; }
    }
}
