namespace BrainSharper.Implementations.Algorithms.Knn
{
    public struct RowIndexDistanceDto<TPredictionResult>
    {
        public RowIndexDistanceDto(int rowIndex, double distance, TPredictionResult dependentFeatureValue)
        {
            RowIndex = rowIndex;
            Distance = distance;
            DependentFeatureValue = dependentFeatureValue;
        }

        public int RowIndex { get; }
        public double Distance { get; }
        public TPredictionResult DependentFeatureValue { get; }
    }
}