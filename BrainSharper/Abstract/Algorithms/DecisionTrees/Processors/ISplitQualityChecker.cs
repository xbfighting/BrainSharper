﻿using System.Collections.Generic;
using BrainSharper.Abstract.Algorithms.DecisionTrees.DataStructures;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Abstract.Algorithms.DecisionTrees.Processors
{
    public interface ISplitQualityChecker
    {
        double CalculateSplitQuality(IDataFrame baseData, IList<ISplittingResult> splittingResults, string dependentFeatureName);
    }
}
