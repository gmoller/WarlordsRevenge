using System.Collections.Generic;
using System.IO;

namespace WarlordsRevengeEditor
{
    public static class FileReader
    {
        public static string[] ReadFile(string path)
        {
            var lines = new List<string>();
            using (var readtext = new StreamReader(path))
            {
                string line;
                do
                {
                    line = readtext.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        lines.Add(line);
                    }
                } while (line != null);
            }

            return lines.ToArray();
        }
    }
}