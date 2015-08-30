namespace BrainSharper.General.DataQuality
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using MathNet.Numerics.LinearAlgebra;

    public interface IConfusionMatrix<TPredictionResult>
    {
        IList<TPredictionResult> ExpectedValues { get; }
        IList<TPredictionResult> ActualValues { get; }
        int CasesCount { get; }
        Matrix<double> ConfusionMatrixValues { get; }
        double Accuracy { get; }
    }

    public class ConfusionMatrix<TPredictionResult> : IConfusionMatrix<TPredictionResult>, IDataQualityReport<TPredictionResult>
    {
        private IList<double> perClassPercentageCorrect;
        private IList<TPredictionResult> allEncounteredValues;

        public ConfusionMatrix(IList<TPredictionResult> expectedValues, IList<TPredictionResult> actualValues)
        {
            ExpectedValues = expectedValues;
            ActualValues = actualValues;
            BuildConfusionMatrix();
        }

        public IList<TPredictionResult> ExpectedValues { get; }
        public IList<TPredictionResult> ActualValues { get; }
        public Matrix<double> ConfusionMatrixValues { get; private set; }
        public double Accuracy { get; private set; }
        public int CasesCount => this.ActualValues.Count;

        private void BuildConfusionMatrix()
        {
            this.perClassPercentageCorrect = new double[this.ActualValues.Count];
            this.allEncounteredValues = new HashSet<TPredictionResult>(this.ActualValues.Union(this.ExpectedValues)).ToList();
            this.ConfusionMatrixValues = Matrix<double>.Build.Dense(this.allEncounteredValues.Count, this.allEncounteredValues.Count);
            int correct = 0;
            for (int actualIdx = 0; actualIdx < this.ActualValues.Count; actualIdx++)
            {
                var actualValue = this.ActualValues[actualIdx];
                var actualValueIndex = this.allEncounteredValues.IndexOf(actualValue);

                var expectedValue = this.ExpectedValues[actualIdx];
                if (actualValue.Equals(expectedValue))
                {
                    this.ConfusionMatrixValues[actualValueIndex, actualValueIndex] += 1;
                    this.perClassPercentageCorrect[actualValueIndex] += 1;
                    correct += 1;
                }
                else
                {
                    var expectedValueIndex = this.allEncounteredValues.IndexOf(expectedValue);
                    this.ConfusionMatrixValues[actualValueIndex, expectedValueIndex] += 1;
                }
            }
            Parallel.For(0, this.perClassPercentageCorrect.Count, elemIdx =>
            {
                this.perClassPercentageCorrect[elemIdx] = this.perClassPercentageCorrect[elemIdx]/CasesCount;
            });
            this.Accuracy = correct/(double)this.ActualValues.Count;
        }
    }
}
