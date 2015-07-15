using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainSharper.Abstract.Data
{
    /// <summary>
    /// Contains indices of rows meeting the filtering criteria
    /// </summary>
    public interface IFilteringResult
    {
        IList<int> IndicesOfRowsMeetingCriteria { get; }
        IList<int> IndicesOfRowsNotMeetingCriteria { get; }
    }
}
