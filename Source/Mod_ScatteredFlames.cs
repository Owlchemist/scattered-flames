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
			LongEventHandler.QueueLongEvent(() => ScatteredFlamesUtility.Setup(), null, false, null);
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
			options.Gap();
			options.Label("ScatteredFlames.Settings.Header.Misc".Translate());
			options.GapLine(); //======================================
			options.CheckboxLabeled("ScatteredFlames.Settings.FireWatcher".Translate(), ref disableFireWatcher, "ScatteredFlames.Settings.FireWatcher.Desc".Translate());
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
			Scribe_Values.Look<bool>(ref multiFlames, "multiFlames", true, false);
			Scribe_Values.Look<bool>(ref specialFX, "specialFX", true, false);
			Scribe_Values.Look<bool>(ref smoke, "smoke", true, false);
			Scribe_Values.Look<bool>(ref disableFireWatcher, "disableFireWatcher", false, false);
			base.ExposeData();
		}

		public static bool multiFlames = true;
		public static bool smoke = true;
		public static bool specialFX = true;
		public static bool disableFireWatcher = false;
	}
}