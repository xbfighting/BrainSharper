using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.Abstract.MathUtils.Normalizers
{
    public interface IQuantitativeDataNormalizer
    {
        Matrix<double> NormalizeColumns(Matrix<double> dataToNormalize, IList<int> columnsToNormalize = null);
    }
}