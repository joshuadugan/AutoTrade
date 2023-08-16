using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoTradeMobile
{
    public static class Helpers
    {
        public static async void AppendLinesToFileAsync(IEnumerable<string> appendTexts, string targetFileName)
        {
            string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            await File.AppendAllTextAsync(targetFile, string.Join(Environment.NewLine, appendTexts) + Environment.NewLine);
        }

        public static async void WriteTextToFileAsync(string text, string targetFileName)
        {
            string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            await File.WriteAllTextAsync(targetFile, text);
        }

        public static async Task<string> ReadTextFileAsync(string targetFileName)
        {
            string targetFile = System.IO.Path.Combine(FileSystem.Current.AppDataDirectory, targetFileName);
            if (!File.Exists(targetFile))
            {
                return string.Empty;
            }
            return await File.ReadAllTextAsync(targetFile);
        }

    }
}
