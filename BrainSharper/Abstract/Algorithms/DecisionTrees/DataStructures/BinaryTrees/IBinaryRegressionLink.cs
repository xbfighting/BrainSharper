using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures.BinaryTrees
{
    public interface IBinaryRegressionLink : IBinaryDecisionTreeLink
    {
        double Variance { get; }
        double Mean { get; }
    }
}
