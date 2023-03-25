using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using RimWorld.Planet;
using static ScatteredFlames.ScatteredFlamesUtility;
using static ScatteredFlames.ModSettings_ScatteredFlames;

namespace ScatteredFlames
{
	[HarmonyPatch (typeof(Fire), nameof(Fire.SpawnSetup))]
    static class Patch_SpawnSetup
    {
        static void Postfix(Fire __instance)
        {
            if (__instance.parent?.def.category == ThingCategory.Pawn) __instance.graphicInt = ResourceBank.FireGraphic;
            else
            {
                fireCache.Add(__instance.thingIDNumber, new FlameData(__instance));
                burningCache.Add(__instance.Position);
                somethingBurning = true;
            }
        }
    }

	[HarmonyPatch (typeof(Fire), nameof(Fire.DeSpawn))]
    static class Patch_DeSpawn
    {
        static void Prefix(Fire __instance)
        {
            fireCache.Remove(__instance.thingIDNumber);
            burningCache.Remove(__instance.Position);
            somethingBurning = burningCache.Count > 0;
        }
    }

	[HarmonyPatch (typeof(World), nameof(World.FinalizeInit))]
    static class Patch_World_FinalizeInit
    {
        static void Prefix()
        {
            fireCache = new System.Collections.Generic.Dictionary<int, FlameData>();
            burningCache = new System.Collections.Generic.HashSet<IntVec3>();
            somethingBurning = false;
        }
    }

    [HarmonyPatch (typeof(FireWatcher), nameof(FireWatcher.UpdateObservations))]
    static class Patch_FireWatcher_UpdateObservations
    {
        static bool Prefix()
        {
            return !disableFireWatcher;
        }
    }


    [HarmonyPatch (typeof(TickManager), nameof(TickManager.DoSingleTick))]
    static class Patch_TickManager_DoSingleTick
    {
        static void Postfix()
        {            
            var tickManager = Current.gameInt.tickManager;
            curTimeSpeed = (int)tickManager.curTimeSpeed;
            if (nextFrame = (tickManager.ticksGameInt % 15) * curTimeSpeed >= 14)
            {
                triggeringFrameID = RealTime.frameCount;
            }
        }
    }

    [HarmonyPatch (typeof(Printer_Shadow), nameof(Printer_Shadow.PrintShadow), new System.Type[]
    { 
        typeof(SectionLayer),
        typeof(Vector3),
        typeof(Vector3),
        typeof(Rot4)
    })]
    static class Patch_PrintShadow
    {
        static bool Prefix(Vector3 center)
        {
            return !(optimizeShadows && somethingBurning && burningCache.Contains(center.ToIntVec3()));
        }
    }
}