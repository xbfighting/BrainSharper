namespace BrainSharper.Abstract.Algorithms.Infrastructure
{
    /// <summary>
    /// Represents abstract prediction model. It is the product of any prediction algorithm work. This interface
    /// is the most generic one, and can be extended ways and by itself does not depend on anything nor contain any usefull information.
    /// </summary>
    public interface IPredictionModel
    {
        /// <summary>
        /// Accuracy measure of the prediction model calculated as: |correct cases| / |all cases|
        /// </summary>
        double TrainingDataAccuracy { get; }
    }
}
