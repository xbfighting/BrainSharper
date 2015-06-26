using System.Data;
using System;
using System.Linq;

namespace BrainSharperTests.TestUtils
{
    public static class DataTableComparer
    {
        public static bool AreTheSame(this DataTable source, DataTable other)
        {
            if (other == null)
            {
                return false;
            }
            return source.AsEnumerable().SequenceEqual(other.AsEnumerable(), DataRowComparer.Default);
        }
    }
}
