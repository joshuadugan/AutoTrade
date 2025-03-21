﻿using System.Collections.ObjectModel;
using System.Text.Json;

namespace AutoTradeMobile.DataClasses
{
    internal static class ObservableCollectionExtensions
    {
        public static void PersistToFile<T>(this ObservableCollection<T> data, string FileName)
        {
            //write the values to file as JSON
            if (data == null) throw new ArgumentNullException("data required in PersistToFile");
            var jsonData = JsonSerializer.Serialize(data);
            Helpers.WriteTextToFileAsync(jsonData, FileName);
        }

        public static void LoadFromFile<T>(this ObservableCollection<T> data, string FileName)
        {
            if (data == null) throw new ArgumentNullException("object required in PersistToFile");
            string jsonData = Helpers.ReadTextFile(FileName);
            if (!string.IsNullOrWhiteSpace(jsonData))
            {
                var newData = JsonSerializer.Deserialize<ObservableCollection<T>>(jsonData);
                foreach (var val in newData)
                {
                    data.Add(val);
                }
            }
        }

    }
}
