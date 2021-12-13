using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderMeal
{
    public class ConfigFileUtil
    {
        public static Dictionary<string, string> LoadConfigFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            var allLines = File.ReadAllLines(path);

            var configDict = new Dictionary<string, string>(allLines.Length);

            for (int i = 0; i < allLines.Length; i++)
            {
                var line = allLines[i];
                line?.Trim();

                var pair = line.Split('=');
                if (pair?.Length != 2)
                    continue;

                var k = pair[0];
                var v = pair[1];

                configDict.Add(k, v);
            }

            return configDict;
        }

    }
}
