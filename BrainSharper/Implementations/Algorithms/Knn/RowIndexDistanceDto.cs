using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainSharper.Implementations.Algorithms.Knn
{
    public struct RowIndexDistanceDto
    {
        public RowIndexDistanceDto(int rowIndex, double distance, double dependentFeatureValue)
        {
            RowIndex = rowIndex;
            Distance = distance;
            DependentFeatureValue = dependentFeatureValue;
        }

        public int RowIndex { get; }
        public double Distance { get; }
        public double DependentFeatureValue { get; }
    }
}
