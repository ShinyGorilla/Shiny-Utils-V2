using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Shiny_Utils_V2
{
    internal class Config
    {
        public static string designatedPath = @"C:\Program Files (x86)\Steam\steamapps\common\Gorilla Tag";

        public static string fileName = "ShinyUtils.txt";

        public static string fullPath = Path.Combine(designatedPath, fileName);

        public static void Create()
        {
            try
            {
                if (!Directory.Exists(designatedPath))
                {
                    Directory.CreateDirectory(designatedPath);
                    Debug.Log("Directory created: " + designatedPath);
                }

                if (!File.Exists(fullPath))
                {
                    File.WriteAllText(fullPath, "0\r\n0\r\n0\r\n0\r\n0\r\n1\r\n67");
                    Debug.Log("File created: " + fullPath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error creating file or directory: " + e.Message);
            }
        
        }

        public static void Save(int line, string value)
        {
            string[] lines = File.ReadAllLines(fullPath);
            lines[line] = value;
            File.WriteAllLines(fullPath, lines);
            Debug.Log($"Changed line {line.ToString()} of {fileName} (ShinyUtils Cfg) to {value}");
        }
    }
}
