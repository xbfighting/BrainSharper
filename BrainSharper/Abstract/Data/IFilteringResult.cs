using System.Collections.Generic;

namespace BrainSharper.Abstract.Data
{
    #region

    

    #endregion

    /// <summary>
    ///     Contains indices of rows meeting the filtering criteria
    /// </summary>
    public interface IFilteringResult
    {
        IList<int> IndicesOfRowsMeetingCriteria { get; }
        IList<int> IndicesOfRowsNotMeetingCriteria { get; }
    }
}