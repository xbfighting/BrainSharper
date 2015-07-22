using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BrainSharper.Abstract.Algorithms.Infrastructure;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees
{
    public interface IDecisionTreeModelBuilder : IPredictionModelBuilder
    {
        // TODO: need to implement: 1) result selector (categorical/numerical), 2) best split selector (numerical attr/categ.attr)
    }
}
