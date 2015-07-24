using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.Infrastructure;
using BrainSharper.Abstract.Data;

namespace BrainSharper.General.DataUtils
{
    public interface ICrossValidator
    {
        IList<double> CrossValidate(
            IPredictionModelBuilder modelBuilder,
            IModelBuilderParams modelBuilderParams,
            IDataFrame dataFrame,
            string dependentFeatureName);
    }
}
