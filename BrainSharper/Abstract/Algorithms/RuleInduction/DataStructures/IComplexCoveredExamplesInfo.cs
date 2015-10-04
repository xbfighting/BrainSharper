namespace BrainSharper.Abstract.Algorithms.RuleInduction.DataStructures
{
    using System.Collections.Generic;

    public interface IComplexCoveredExamplesInfo<TValue>
    {
        //TODO: !!! AAA Think about storing menemonics with EXACT COVERED DEPENDENT VALUES?
        int ExamplesCoveredByComplexCount(IComplex<TValue> complex);
        IList<int> ExamplesCoveredByComplex(IComplex<TValue> complex);
    }
}
