using HarmonyLib;
using Verse;
using UnityEngine;
using static ScatteredFlames.ModSettings_ScatteredFlames;

namespace ScatteredFlames
{
	public class Mod_ScatteredFlames : Mod
	{
		public Mod_ScatteredFlames(ModContentPack content) : base(content)
		{
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll();
			base.GetSettings<ModSettings_ScatteredFlames>();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			Listing_Standard options = new Listing_Standard();
			options.Begin(inRect);
			options.Label("ScatteredFlames.Settings.DisableAll".Translate());
			options.Gap();
			options.Label("ScatteredFlames.Settings.Header.Graphics".Translate());
			options.GapLine(); //======================================
			options.CheckboxLabeled("ScatteredFlames.Settings.MultiFlames".Translate(), ref multiFlames, "ScatteredFlames.Settings.MultiFlames.Desc".Translate());
			options.CheckboxLabeled("ScatteredFlames.Settings.SpecialFX".Translate(), ref specialFX, "ScatteredFlames.Settings.SpecialFX.Desc".Translate());
			if (ScatteredFlamesUtility.smokeInstalled) options.CheckboxLabeled("ScatteredFlames.Settings.Smoke".Translate(), ref smoke, "ScatteredFlames.Settings.Smoke.Desc".Translate());
			else 
			{
				smoke = false;
				options.CheckboxLabeled("ScatteredFlames.Settings.Smoke".Translate().Colorize(Color.gray), ref smoke, "ScatteredFlames.Settings.Smoke.Desc".Translate());
			}
			options.CheckboxLabeled("ScatteredFlames.Settings.OptimizeShadows".Translate(), ref optimizeShadows, "ScatteredFlames.Settings.OptimizeShadows.Desc".Translate());
			options.Gap();
			options.Label("ScatteredFlames.Settings.Header.Misc".Translate());
			options.GapLine(); //======================================
			options.CheckboxLabeled("ScatteredFlames.Settings.FireWatcher".Translate(), ref disableFireWatcher, "ScatteredFlames.Settings.FireWatcher.Desc".Translate());
			options.CheckboxLabeled("ScatteredFlames.Settings.IgniteGizmo".Translate(), ref enableIgniteGizmo, "ScatteredFlames.Settings.IgniteGizmo.Desc".Translate());
			options.End();
			base.DoSettingsWindowContents(inRect);
		}

		public override string SettingsCategory()
		{
			return "Scattered Flames";
		}

		public override void WriteSettings()
		{
			base.WriteSettings();
			ScatteredFlamesUtility.Setup();
		}
    }

	public class ModSettings_ScatteredFlames : ModSettings
	{
		public override void ExposeData()
		{
			Scribe_Values.Look(ref multiFlames, "multiFlames", true);
			Scribe_Values.Look(ref specialFX, "specialFX", true);
			Scribe_Values.Look(ref smoke, "smoke", true);
			Scribe_Values.Look(ref optimizeShadows, "optimizeShadows", true);
			Scribe_Values.Look(ref disableFireWatcher, "disableFireWatcher");
			Scribe_Values.Look(ref enableIgniteGizmo, "enableIgniteGizmo");
			base.ExposeData();
		}

		public static bool disableFireWatcher, enableIgniteGizmo, multiFlames = true, smoke = true, specialFX = true, optimizeShadows = true;
	}
}