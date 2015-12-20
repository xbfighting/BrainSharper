using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures;

namespace BrainSharper.Implementations.Algorithms.RuleInduction.DataStructures
{
    public class ComplexCoveredExamplesInfo<TValue> : IComplexCoveredExamplesInfo<TValue>
    {
        private readonly IDictionary<IComplex<TValue>, IList<int>> coverage;

        public ComplexCoveredExamplesInfo(IDictionary<IComplex<TValue>, IList<int>> examplesCoveredByComplexList)
        {
            coverage = examplesCoveredByComplexList;
        }

        public int ExamplesCoveredByComplexCount(IComplex<TValue> complex)
        {
            IList<int> coveredExamplesIndices;
            if (coverage.TryGetValue(complex, out coveredExamplesIndices))
            {
                return coveredExamplesIndices.Count;
            }
            return 0;
        }

        public IList<int> ExamplesCoveredByComplex(IComplex<TValue> complex)
        {
            IList<int> coveredExamplesIndices;
            if (coverage.TryGetValue(complex, out coveredExamplesIndices))
            {
                return coveredExamplesIndices;
            }
            return new int[0];
        }
    }
}