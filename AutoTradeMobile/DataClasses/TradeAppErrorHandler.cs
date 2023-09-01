using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradeMobile
{
    internal static partial class Extensions
    {
        public const string ErrorFileName = "Errors.txt";
        public static void WriteExceptionToLog(this Exception ex)
        {
            Debug.Assert(ex != null);
            Helpers.WriteTextToFileAsync(ex.ToString(), ErrorFileName);
        }

    }
}
