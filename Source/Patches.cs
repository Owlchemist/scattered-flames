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
            if (__instance.parent is Pawn) __instance.graphicInt = ResourceBank.FireGraphic;
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
            if (burningCache.Count > 0) somethingBurning = false;
        }
    }

    [HarmonyPatch (typeof(GameComponent), nameof(GameComponent.GameComponentUpdate))]
    static class Patch_GameComponentUpdate
    {
        static void Prefix()
        {
            ++frameID;
            if (frameID == int.MaxValue) frameID = 0;
        }
    }

    [HarmonyPatch (typeof(World), nameof(World.WorldTick))]
    static class Patch_WorldTick
    {
        static int ticker;
        static void Prefix()
        {
            ticker += 1 * (int)Current.gameInt.tickManager.curTimeSpeed;
            
            if (nextFrame = ticker >= 14)
            {
                ticker = 0;
                triggeringFrameID = frameID;
            }
            if (Current.ProgramState == ProgramState.Playing) isPausedCache = Current.gameInt.tickManager.Paused;
        }
    }

	[HarmonyPatch (typeof(Game), nameof(Game.LoadGame))]
    static class Patch_LoadGame
    {
        static void Prefix()
        {
            fireCache.Clear();
            burningCache.Clear();
            somethingBurning = false;
        }
    }

	[HarmonyPatch (typeof(Game), nameof(Game.InitNewGame))]
    static class Patch_InitNewGame
    {
        static void Prefix()
        {
            fireCache.Clear();
            burningCache.Clear();
            somethingBurning = false;
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