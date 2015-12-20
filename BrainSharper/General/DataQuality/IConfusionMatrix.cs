using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace BrainSharper.General.DataQuality
{
    public interface IConfusionMatrix<TPredictionResult>
    {
        IList<TPredictionResult> ExpectedValues { get; }
        IList<TPredictionResult> ActualValues { get; }
        int CasesCount { get; }
        Matrix<double> ConfusionMatrixValues { get; }
        double Accuracy { get; }
    }

    public class ConfusionMatrix<TPredictionResult> : IConfusionMatrix<TPredictionResult>,
        IDataQualityReport<TPredictionResult>
    {
        private IList<TPredictionResult> allEncounteredValues;
        private IList<double> perClassPercentageCorrect;

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
        public int CasesCount => ActualValues.Count;

        private void BuildConfusionMatrix()
        {
            perClassPercentageCorrect = new double[ActualValues.Count];
            allEncounteredValues = new HashSet<TPredictionResult>(ActualValues.Union(ExpectedValues)).ToList();
            ConfusionMatrixValues = Matrix<double>.Build.Dense(allEncounteredValues.Count, allEncounteredValues.Count);
            var correct = 0;
            for (var actualIdx = 0; actualIdx < ActualValues.Count; actualIdx++)
            {
                var actualValue = ActualValues[actualIdx];
                var actualValueIndex = allEncounteredValues.IndexOf(actualValue);

                var expectedValue = ExpectedValues[actualIdx];
                if (actualValue.Equals(expectedValue))
                {
                    ConfusionMatrixValues[actualValueIndex, actualValueIndex] += 1;
                    perClassPercentageCorrect[actualValueIndex] += 1;
                    correct += 1;
                }
                else
                {
                    var expectedValueIndex = allEncounteredValues.IndexOf(expectedValue);
                    ConfusionMatrixValues[actualValueIndex, expectedValueIndex] += 1;
                }
            }
            Parallel.For(0, perClassPercentageCorrect.Count,
                elemIdx => { perClassPercentageCorrect[elemIdx] = perClassPercentageCorrect[elemIdx]/CasesCount; });
            Accuracy = correct/(double) ActualValues.Count;
        }
    }
}