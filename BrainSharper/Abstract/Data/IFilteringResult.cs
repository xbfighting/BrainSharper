namespace BrainSharper.Abstract.Data
{
    #region

    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// Contains indices of rows meeting the filtering criteria
    /// </summary>
    public interface IFilteringResult
    {
        IList<int> IndicesOfRowsMeetingCriteria { get; }

        IList<int> IndicesOfRowsNotMeetingCriteria { get; }
    }
}