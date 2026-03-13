using System.Collections.Generic;
using UnityEngine;

namespace ImperiumEvents
{
    public static class SFX_VFX_Registry
    {
        private static readonly HashSet<string> MetalObjects = new()
        {
        };

        private static readonly HashSet<string> WoodObjects = new()
        {
            "Imperium_Standing_Banner",
            "Imperium_Hanging_Banner",
            "Start_Gate",
            "Finish_Gate",
            "Arrow_Left",
            "Arrow_Right",
            "Arrow_Straight",
            "Welcome_Banner",
            "Danger_Troll",
            "Danger_Other",
            "Safe_Zone",
            "Imperium_Horn",
            "Start_Rune",
            "Finish_Rune",
            "Imperium_Scoreboard"
        };

        public static void GetEffects(
            string prefabName,
            out GameObject vfxPlace,
            out GameObject sfxPlace,
            out GameObject destroyVFX,
            out GameObject destroySFX)
        {
            if (MetalObjects.Contains(prefabName))
            {
                vfxPlace = ZNetScene.instance?.GetPrefab("vfx_Place_stone");
                sfxPlace = ZNetScene.instance?.GetPrefab("sfx_build_hammer_metal");
                destroyVFX = ZNetScene.instance?.GetPrefab("vfx_destroyed");
                destroySFX = ZNetScene.instance?.GetPrefab("sfx_metal_blocked");
                return;
            }

            if (WoodObjects.Contains(prefabName))
            {
                vfxPlace = ZNetScene.instance?.GetPrefab("vfx_Place_wood");
                sfxPlace = ZNetScene.instance?.GetPrefab("sfx_build_hammer_wood");
                destroyVFX = ZNetScene.instance?.GetPrefab("vfx_destroyed");
                destroySFX = ZNetScene.instance?.GetPrefab("sfx_wood_break");
                return;
            }

            vfxPlace = ZNetScene.instance?.GetPrefab("vfx_Place_stone");
            sfxPlace = ZNetScene.instance?.GetPrefab("sfx_build_hammer_stone");
            destroyVFX = ZNetScene.instance?.GetPrefab("vfx_destroyed");
            destroySFX = ZNetScene.instance?.GetPrefab("sfx_rock_destroyed");
        }
    }
}
