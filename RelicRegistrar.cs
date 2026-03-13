using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;

namespace ImperiumEvents
{
    public class RelicRegistration
    {
        public string PrefabName;
        public string DisplayName;
        public RequirementConfig[] Requirements;
        public string Description;
        public int Comfort;
        public bool IsWerewolf;
        public bool IsHorn;

        public RelicRegistration(string prefab, string display, RequirementConfig[] reqs, string desc, int comfort = 0, bool isWerewolf = false, bool isHorn = false)
        {
            PrefabName = prefab;
            DisplayName = display;
            Requirements = reqs;
            Description = desc;
            Comfort = comfort;
            IsWerewolf = isWerewolf;
            IsHorn = isHorn;
        }
    }

    public static class RelicRegistrar
    {
        private static readonly List<GameObject> placedWerewolves = new();

        private static bool wasAlreadyRegistered = false;

        public static readonly List<RelicRegistration> AllRegistrations = new()
        {
            
            // Gates
            
            new RelicRegistration("Start_Gate", "Start Gate", new[] {
                new RequirementConfig("RoundLog", 10), new RequirementConfig("FineWood", 5)
            }, "Imperium event Start Gate."),

            new RelicRegistration("Finish_Gate", "Finish Gate", new[] {
                new RequirementConfig("RoundLog", 10), new RequirementConfig("FineWood", 5)
            }, "Imperium event Start Gate."),

            // Banners


            new RelicRegistration("Welcome_Banner", "Welcome Banner", new[] {
                new RequirementConfig("Wood", 5), new RequirementConfig("FineWood", 2)
            }, "Imperium event banner."),

            new RelicRegistration("Imperium_Standing_Banner", "Imperium Standing Banner", new[] {
                new RequirementConfig("Wood", 5), new RequirementConfig("FineWood", 2)
            }, "Imperium event banner."),

                        new RelicRegistration("Imperium_Hanging_Banner", "Imperium Hanging Banner", new[] {
                new RequirementConfig("Wood", 5), new RequirementConfig("FineWood", 3)
            }, "Imperium event banner."),

                        new RelicRegistration("Arrow_Left", "Left Arrow Banner", new[] {
                new RequirementConfig("Wood", 5), new RequirementConfig("FineWood", 3)
            }, "Imperium event banner."),

                        new RelicRegistration("Arrow_Right", "Right Arrow Banner", new[] {
                new RequirementConfig("Wood", 5), new RequirementConfig("FineWood", 3)
            }, "Imperium event banner."),

                        new RelicRegistration("Arrow_Straight", "Straight Arrow Banner", new[] {
                new RequirementConfig("Wood", 5), new RequirementConfig("FineWood", 2)
            }, "Imperium event banner."),

                        new RelicRegistration("Safe_Zone", "Safe Zone Banner", new[] {
                new RequirementConfig("Wood", 5), new RequirementConfig("FineWood", 2)
            }, "Imperium event banner."),

                        new RelicRegistration("Danger_Other", "Other kind of danger Banner", new[] {
                new RequirementConfig("Wood", 5), new RequirementConfig("FineWood", 2)
            }, "Imperium event banner."),

                        new RelicRegistration("Danger_Troll", "Danger of Troll Banner", new[] {
                new RequirementConfig("Wood", 5), new RequirementConfig("FineWood", 2)
            }, "Imperium event banner."),

                        new RelicRegistration("Imperium_Horn", "Horn of Farting", new[] {
                new RequirementConfig("Wood", 5), new RequirementConfig("FineWood", 2)
            }, "Imperium event banner."),

                        new RelicRegistration("Start_Rune", "Start Registration", new[] {
                new RequirementConfig("Wood", 5), new RequirementConfig("FineWood", 2)
            }, "For player registration to the event."),

                        new RelicRegistration("Finish_Rune", "Finish line", new[] {
                new RequirementConfig("Wood", 5), new RequirementConfig("FineWood", 2)
            }, "To stop timers."),

            new RelicRegistration("Imperium_Scoreboard", "Scoreboard", new[] {
                new RequirementConfig("Wood", 5), new RequirementConfig("FineWood", 2)
            }, "Timers.")


        };

        public static void RegisterAllRelics(AssetBundle bundle)
        {
            if (wasAlreadyRegistered || bundle == null) return;

            int ok = 0, fail = 0;
            foreach (var reg in AllRegistrations)
            {
                if (RegisterRelic(bundle, reg)) ok++; else fail++;
            }
            wasAlreadyRegistered = true;
        }

        private static bool RegisterRelic(AssetBundle bundle, RelicRegistration reg)
        {
            if (bundle == null) return false;
            GameObject prefab = bundle.LoadAsset<GameObject>(reg.PrefabName);
            if (prefab == null)
            {
                return false;
            }

            prefab.name = reg.PrefabName;

            var znv = prefab.GetComponent<ZNetView>();
            if (znv == null)
                znv = prefab.AddComponent<ZNetView>();
            znv.m_persistent = true;
            znv.m_syncInitialScale = true;

            if (!prefab.GetComponent<ZSyncTransform>())
                prefab.AddComponent<ZSyncTransform>();

            Piece piece = prefab.GetComponent<Piece>() ?? prefab.AddComponent<Piece>();
            piece.m_name = reg.DisplayName;
            piece.m_description = reg.Description;
            piece.m_groundOnly = false;

            GameObject vfxPlace, sfxPlace, destroyVFX, destroySFX;

            SFX_VFX_Registry.GetEffects(
                reg.PrefabName,
                out vfxPlace,
                out sfxPlace,
                out destroyVFX,
                out destroySFX
            );

            var placeFX = new EffectList();
            var placeList = new List<EffectList.EffectData>();
            if (vfxPlace != null) placeList.Add(new EffectList.EffectData { m_prefab = vfxPlace, m_enabled = true });
            if (sfxPlace != null) placeList.Add(new EffectList.EffectData { m_prefab = sfxPlace, m_enabled = true });
            placeFX.m_effectPrefabs = placeList.ToArray();
            piece.m_placeEffect = placeFX;

            WearNTear wear = prefab.GetComponent<WearNTear>() ?? prefab.AddComponent<WearNTear>();
            wear.m_health = 10000f;
            wear.m_noRoofWear = true;

            var destroyFX = new EffectList();
            var destroyList = new List<EffectList.EffectData>();
            if (destroyVFX != null) destroyList.Add(new EffectList.EffectData { m_prefab = destroyVFX, m_enabled = true });
            if (destroySFX != null) destroyList.Add(new EffectList.EffectData { m_prefab = destroySFX, m_enabled = true });
            destroyFX.m_effectPrefabs = destroyList.ToArray();
            wear.m_destroyedEffect = destroyFX;


            if (reg.Comfort > 0) piece.m_comfort = reg.Comfort;

            Sprite icon = bundle.LoadAsset<Sprite>(reg.PrefabName);
            if (icon != null) piece.m_icon = icon;

            string craftingStation = "piece_workbench";

            var config = new PieceConfig
            {
                PieceTable = "Hammer",
                Category = ImperiumEvents.PlayerPreferredCategory.Value,
                CraftingStation = craftingStation,
                Requirements = reg.Requirements
            };

            PieceManager.Instance.AddPiece(new CustomPiece(prefab, true, config));

            if (reg.IsWerewolf && prefab.GetComponent<PlacementWatcher>() == null)
                prefab.AddComponent<PlacementWatcher>().RegisterList = placedWerewolves;

            return true;
        }
    }
}