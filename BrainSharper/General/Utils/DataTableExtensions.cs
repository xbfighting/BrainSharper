using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainSharper.General.Utils
{
    public static class DataTableExtensions
    {
        public static bool IsEqualTo(this DataTable source, DataTable other)
        {
            if (other == null)
            {
                return false;
            }
            return source.AsEnumerable().SequenceEqual(other.AsEnumerable(), DataRowComparer.Default);
        }
    }
}
