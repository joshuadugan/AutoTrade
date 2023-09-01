using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradeMobile
{
    internal static partial class Extensions
    {
        public static decimal? ToDecimal(this double? d)
        {
            return d.HasValue ? (decimal?)d.Value : null;
        }

    }
}
