using System.Collections.Generic;

namespace BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification
{
    public interface IClassificationTransaction<TValue>
    {
        TValue ClassLabelValue { get; }
        string DecisiveFeature { get; }

        IList<string> Features { get; }
        IList<TValue> Values { get; }

        bool ContainsFeature(string feature);
        TValue GetValueForFeature(string feature);
        
    }
}
