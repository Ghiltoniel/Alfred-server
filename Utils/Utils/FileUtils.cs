using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Alfred.Utils.Utils
{
    public class FileUtils
    {
        public static Dictionary<string, string> ReadConfig(string file)
        {
            return File
                .ReadAllLines(file, Encoding.UTF8)
                .Select(line => line.Split(new[] {"="}, StringSplitOptions.RemoveEmptyEntries))
                .Where(infoLine => infoLine.Length == 2)
                .ToDictionary(infoLine => infoLine[0], infoLine => infoLine[1]);
        }

        public static Dictionary<string, List<string>> ReadFileToArray(string file)
        {
            var res = new Dictionary<string, List<string>>();
            foreach (var line in File.ReadAllLines(file, Encoding.UTF8))
            {
                var infoLine = line.Split(new[]{"=>"}, StringSplitOptions.RemoveEmptyEntries);
                if (infoLine.Length == 2)
                {
                    var infoLine2 = infoLine[1].Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                    res.Add(infoLine[0], infoLine2.ToList());
                }
            }
            return res;
        }

        public static void WriteDictionaryToConfig(Dictionary<string, object> settings, string filename)
        {
            if (!File.Exists(filename))
            {
                var stream = File.Create(filename);
                stream.Dispose();
            }

            File.WriteAllLines(filename, settings.Select(i=>string.Format("{0}={1}", i.Key, i.Value)));
        }
    }
}
