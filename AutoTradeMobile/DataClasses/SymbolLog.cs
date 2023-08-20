using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradeMobile
{
    public partial class TradeApp
    {

        private static Queue<LogQueueObj> SymbolLogQueue = new Queue<LogQueueObj>();

        Timer SymbolLogTimer = new Timer(PersistStringsToFile, null, 10000, 10000);

        private static void PersistStringsToFile(object state)
        {
            lock (SymbolLogQueue)
            {
                List<LogQueueObj> logs = new List<LogQueueObj>();
                while (SymbolLogQueue.Count > 0)
                {
                    logs.Add(SymbolLogQueue.Dequeue());

                }
                var groups = logs.GroupBy(l => l.fileName);
                foreach (var group in groups)
                {
                    var fileName = group.Key;
                    var fileData = group.Select(g => g.fileData);
                    Helpers.AppendLinesToFileAsync(fileData, fileName);
                    Trace.WriteLine($"Log Data persisted to {fileName}.");
                }
            }
        }

        private class LogQueueObj
        {
            public string fileData { get; set; }
            public string fileName { get; set; }
        }

    }
}
