using BepInEx.Logging;
using Jotunn.Managers;
using UnityEngine;

namespace ImperiumEvents
{
    public static class ImperiumTriathlonBootstrap
    {
        public static void AttachTriathlonComponents(ManualLogSource logger)
        {
            AttachIfFound("Imperium_Horn", typeof(HornInteract), logger);
            AttachIfFound("Start_Rune", typeof(StartRuneInteract), logger);
            AttachIfFound("Finish_Rune", typeof(FinishRuneInteract), logger);
            AttachIfFound("Imperium_Scoreboard", typeof(RaceScoreboard), logger);

            AttachNightLight(logger);
        }

        private static void AttachIfFound(string prefabName, System.Type componentType, ManualLogSource logger)
        {
            GameObject prefab = PrefabManager.Instance.GetPrefab(prefabName);
            if (prefab == null)
            {
                logger?.LogWarning($"[ImperiumEvents] Prefab not found for triathlon hookup: {prefabName}");
                return;
            }

            if (prefab.GetComponent(componentType) == null)
            {
                prefab.AddComponent(componentType);
                logger?.LogInfo($"[ImperiumEvents] Attached {componentType.Name} to {prefabName}.");
            }
        }

        private static void AttachNightLight(ManualLogSource logger)
        {
            GameObject prefab = PrefabManager.Instance.GetPrefab("Imperium_Scoreboard");
            if (prefab == null)
            {
                logger?.LogWarning("[ImperiumEvents] Imperium_Scoreboard prefab not found.");
                return;
            }

            Transform nightLight = prefab.transform.Find("NightLight");

            if (nightLight == null)
            {
                logger?.LogWarning("[ImperiumEvents] NightLight child not found on Imperium_Scoreboard.");
                return;
            }

            if (nightLight.GetComponent<ScoreboardEmissionNight>() == null)
            {
                nightLight.gameObject.AddComponent<ScoreboardEmissionNight>();
                logger?.LogInfo("[ImperiumEvents] Attached ScoreboardEmissionNight to NightLight.");
            }
        }
    }
}