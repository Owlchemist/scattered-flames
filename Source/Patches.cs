using HarmonyLib;
using RimWorld;
using Verse;
using static ScatteredFlames.ScatteredFlamesUtility;
using static ScatteredFlames.ModSettings_ScatteredFlames;

namespace ScatteredFlames
{
	[HarmonyPatch (typeof(Fire), nameof(Fire.SpawnSetup))]
    static class Patch_SpawnSetup
    {
        static void Postfix(Fire __instance)
        {
			fireCache.Add(__instance, new SubFlame(__instance));
        }
    }

	[HarmonyPatch (typeof(Fire), nameof(Fire.DeSpawn))]
    static class Patch_DeSpawn
    {
        static void Prefix(Fire __instance)
        {
            /*
            if (fireCache.TryGetValue(__instance, out SubFlame subFlame) && subFlame.firelight != null)
            {
                __instance.Map.mapDrawer.MapMeshDirty(__instance.Position, MapMeshFlag.Things);
                __instance.Map.glowGrid.DeRegisterGlower(subFlame.firelight);
            }
            UpdateLights(__instance, fireCache.TryGetValue(__instance));
            */
            fireCache.Remove(__instance);
        }
    }

    [HarmonyPatch (typeof(GameComponentUtility), nameof(GameComponentUtility.GameComponentUpdate))]
    static class Patch_GameComponentUpdate
    {
        static int ticker;
        static void Prefix()
        {
            ticker += 1 * (int)Current.gameInt.tickManager.curTimeSpeed;
            
            if (nextFrame = ticker >= 15) ticker = 0;
        }
    }

	[HarmonyPatch (typeof(Game), nameof(Game.LoadGame))]
    static class Patch_LoadGame
    {
        static void Prefix()
        {
            fireCache.Clear();
        }
    }

	[HarmonyPatch (typeof(Game), nameof(Game.InitNewGame))]
    static class Patch_InitNewGame
    {
        static void Prefix()
        {
            fireCache.Clear();
        }
    }

    [HarmonyPatch (typeof(FireWatcher), nameof(FireWatcher.FireWatcherTick))]
    static class Patch_FireWatcherTick
    {
        static bool Prefix()
        {
            return !disableFireWatcher;
        }
    }
}