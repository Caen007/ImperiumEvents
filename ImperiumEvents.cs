using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Managers;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ImperiumEvents
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class ImperiumEvents : BaseUnityPlugin
    {
        public const string PluginGUID = "Imperium.Events";
        public const string PluginName = "Imperium Events";
        public const string PluginVersion = "1.0.0";

        private AssetBundle relicsBundle;

        public static BepInEx.Configuration.ConfigEntry<string> PlayerPreferredCategory;
        public static ManualLogSource ModLogger;

        private void Awake()
        {
            ModLogger = Logger;

            new Harmony("imperium.events.harmony").PatchAll();

            PlayerPreferredCategory = Config.Bind(
                "General",
                "CustomHammerTab",
                "ImperiumEvents",
                "Custom hammer tab category name");

            ImperiumTriathlonManager.Initialize(Config);
            gameObject.AddComponent<ImperiumTriathlonManager>();

            var asm = Assembly.GetExecutingAssembly();
            const string wanted = "imperium";
            string resName = asm.GetManifestResourceNames()
                                .FirstOrDefault(r => r.ToLower().Contains(wanted));

            if (string.IsNullOrEmpty(resName))
            {
                Logger.LogError($"Could not find embedded asset bundle resource containing '{wanted}'.");
                return;
            }

            using (var s = asm.GetManifestResourceStream(resName))
            {
                if (s == null)
                {
                    Logger.LogError($"Embedded asset bundle stream was null for '{resName}'.");
                    return;
                }

                using (var ms = new MemoryStream())
                {
                    s.CopyTo(ms);
                    relicsBundle = AssetBundle.LoadFromMemory(ms.ToArray());
                }
            }

            if (relicsBundle == null)
            {
                Logger.LogError("Failed to load AssetBundle from embedded resource!");
                return;
            }

            PrefabManager.OnPrefabsRegistered -= RegisterNow;
            PrefabManager.OnPrefabsRegistered += RegisterNow;
        }

        private void RegisterNow()
        {
            if (relicsBundle == null)
            {
                return;
            }

            RelicRegistrar.RegisterAllRelics(relicsBundle);
            ImperiumTriathlonBootstrap.AttachTriathlonComponents(Logger);

            Logger.LogInfo($"[ImperiumEvents] Registration complete. Hammer tab: '{PlayerPreferredCategory.Value}'.");
        }
    }
}