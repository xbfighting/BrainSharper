using System.Collections.Generic;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.RuleInduction.Heuristics
{
    public interface IComplexQualityChecker
    {
        ComplexQualityData CalculateComplexQuality(
            IDataFrame dataFrame,
            string dependentFeatureName,
            IList<int> examplesCoveredByComplex);
    }

    public struct ComplexQualityData
    {
        public ComplexQualityData(double qualityValue, bool isBestPossible = false)
        {
            QualityValue = qualityValue;
            IsBestPossible = isBestPossible;
        }

        public double QualityValue { get; }
        public bool IsBestPossible { get; }

        public bool Equals(ComplexQualityData other)
        {
            return QualityValue.Equals(other.QualityValue) && IsBestPossible == other.IsBestPossible;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is ComplexQualityData && Equals((ComplexQualityData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (QualityValue.GetHashCode()*397) ^ IsBestPossible.GetHashCode();
            }
        }
    }
}