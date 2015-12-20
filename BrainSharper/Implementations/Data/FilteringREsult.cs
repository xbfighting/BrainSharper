using System.Collections.Generic;
using BrainSharper.Abstract.Data;

namespace BrainSharper.Implementations.Data
{
    public class FilteringResult : IFilteringResult
    {
        public FilteringResult(IList<int> indicesOfRowsMeetingCriteria, IList<int> indicesOfRowsNotMeetingCriteria)
        {
            IndicesOfRowsMeetingCriteria = indicesOfRowsMeetingCriteria;
            IndicesOfRowsNotMeetingCriteria = indicesOfRowsNotMeetingCriteria;
        }

        public IList<int> IndicesOfRowsMeetingCriteria { get; }
        public IList<int> IndicesOfRowsNotMeetingCriteria { get; }
    }
}