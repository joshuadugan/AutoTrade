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

        private static Queue<LogQueueObj> FileLogQueue = new Queue<LogQueueObj>();

        Timer FileLogTimer = new Timer(PersistStringsToFile, null, 10000, 10000);

        private static void LogToFile(string fileData, string filename)
        {
            FileLogQueue.Enqueue(new LogQueueObj() { fileData = fileData, fileName = filename });
        }

        private static void PersistStringsToFile(object state)
        {
            lock (FileLogQueue)
            {
                List<LogQueueObj> logs = new List<LogQueueObj>();
                while (FileLogQueue.Count > 0)
                {
                    logs.Add(FileLogQueue.Dequeue());

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
