using System;
using System.Collections.Generic;

namespace Game.Serialization
{
    public static class CSVReader
    {
        public const string Comma = "[comma]";
        public const string NewLine = "[newline]";

        [Serializable]
        public abstract class CSVData
        {
            public virtual string SetData(string[] properties)
            {
                for (var i = 0; i < properties.Length; i++)
                {
                    properties[i] = properties[i].Replace(Comma, ",");
                    properties[i] = properties[i].Replace(NewLine, "\n");
                }
                return string.Empty;
            }
        }
    }
    public static class CSVReader<TData> where TData : CSVReader.CSVData, new()
    {
        public static void ReadCSV(IDictionary<string, TData> dict, string csv)
        {
            var lines = csv.Split('\n');

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                if (line[0] == '#') continue;
                
                var property = line.Split(',');
                
                var data = new TData();
                var key = data.SetData(property);
                
                dict.Add(key, data);
            }
        }
    }
}
