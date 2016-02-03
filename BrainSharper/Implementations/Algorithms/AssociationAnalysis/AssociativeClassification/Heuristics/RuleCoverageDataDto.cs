using BrainSharper.Abstract.Algorithms.AssociationAnalysis.AssociativeClassification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainSharper.Implementations.Algorithms.AssociationAnalysis.AssociativeClassification.Heuristics
{
    internal class RuleCoverageDataDto<TValue>
    {
        private int _correctClassifications { get; set; }
        private int _incorrectClassifications { get; set; }

        public RuleCoverageDataDto(IClassificationAssociationRule<TValue> rule)
        {
            Rule = rule;
            CoveredExamples = new HashSet<int>();
            this._correctClassifications = 0;
            this._incorrectClassifications = 0;
            DefaultValueForRemainingData = default(TValue);
        }

        public IClassificationAssociationRule<TValue> Rule { get; }
        public ISet<int> CoveredExamples { get; set; }

        public TValue DefaultValueForRemainingData { get; set; }

        public double Accuracy
            => this._correctClassifications / (double)(_correctClassifications + _incorrectClassifications);

        public bool CoversAnyExample => CoveredExamples.Any();

        public void IncrementCorrectClassif()
        {
            this._correctClassifications++;
        }

        public void IncrementIncorrectClassif()
        {
            this._incorrectClassifications++;
        }
    }
}
