using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.Heuristics
{
    public class PositivelyCorrelatedRulesHeuristic<TValue> : BasicCARRulesSelector<TValue>
    {
        private readonly double _liftThreshold;

        public PositivelyCorrelatedRulesHeuristic(double liftThreshold)
        {
            _liftThreshold = liftThreshold;
        }
    }
}
