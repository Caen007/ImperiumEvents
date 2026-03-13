using System.IO;
using UnityEngine;
using Jotunn.Managers;

namespace ImperiumEvents
{
    public static class RelicConfigManager
    {
        private static string configPath = Path.Combine(BepInEx.Paths.ConfigPath, "ImperiumEvents.cfg");

        public static void SaveConfig(string key, string value)
        {
            File.AppendAllText(configPath, $"{key}={value}\n");
        }

        public static string LoadConfig(string key)
        {
            if (!File.Exists(configPath))
                return null;

            foreach (var line in File.ReadAllLines(configPath))
            {
                if (line.StartsWith(key + "="))
                    return line.Substring(key.Length + 1);
            }

            return null;
        }
    }
}