using Verse;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static ScatteredFlames.ModSettings_ScatteredFlames;

namespace ScatteredFlames
{
	public static class ScatteredFlamesUtility
	{
		public static Dictionary<Thing, SubFlame> fireCache = new Dictionary<Thing, SubFlame>();
		public static bool nextFrame, flickerNow, smokeInstalled;
		private static Vector3 centering = new Vector3(0.5f, 0, 0.5f);
		//public static float radiusCache;
		//public static ColorInt colorCache;
		static DefModExtension backup;
		public static FastRandom fastRandom;

		public static void Setup()
		{
			fastRandom = new FastRandom();
			var fire = DefDatabase<ThingDef>.GetNamed("Fire");
			/*
			if (fire.HasModExtension<Firelight>())
			{
				radiusCache = fire.GetModExtension<Firelight>().glowRadius;
				colorCache = fire.GetModExtension<Firelight>().glowColor * 0.9f;
			}
			*/

            smokeInstalled = LoadedModManager.RunningMods.Any(x => x.Name == "Simple FX: Smoke");

			//Remove smoke extension?
			bool restartNeeded = false;
			if (!smoke)
			{
				backup = fire.modExtensions.FirstOrDefault(x => x.GetType().Name == "Flecker");
				if (backup != null && fire.modExtensions.Remove(backup)) restartNeeded = true;
			}
			else if (fire.modExtensions.FirstOrDefault(x => x.GetType().Name == "Flecker") == null)
			{
				fire.modExtensions.Add(backup);
				restartNeeded = true;
			}

			//Reload save?
			if (restartNeeded && Current.ProgramState == ProgramState.Playing)
			{
				Find.WindowStack.Add(new Dialog_MessageBox("ScatteredFlames.ReloadRequired".Translate(), null, null, null, null, "ScatteredFlames.ReloadHeader".Translate(), true, null, null, WindowLayer.Dialog));
			}
		}
		/*
		public static void UpdateLights(Thing key, SubFlame value)
		{
			foreach (IntVec3 cell in value.adjacentCells)
			{
				if (fireCache.FirstOrDefault(x => x.Key.Position == cell).Value?.glowHolder != null) return;
			}

			//Make glower
			value.firelight = new CompGlower();
			value.firelight.props = new CompProperties_Glower()
			{ 
				glowRadius = radiusCache,
				glowColor = colorCache
			};
			value.firelight.glowOnInt = true;

			//Setup dummy holder
			value.glowHolder = new ThingWithComps();
			value.glowHolder.def = ResourceBank.ThingDefOf.Owl_DummyHolder;
			value.glowHolder.Position = key.Position;
			value.firelight.parent = value.glowHolder;

			//Update glow grid now
			key.Map.mapDrawer.MapMeshDirty(key.Position, MapMeshFlag.Things);
			key.Map.glowGrid.RegisterGlower(value.firelight);	
		}
		*/

		public class SubFlame
		{
			public SubFlame(Fire fire)
			{
				//Determine starting frame
				frame = Random.Range(0, 3);

				//Determine if multiflame or not
				numOfOffsets = multiFlames ? Random.Range(1, 3) : 1;

				//Determine offsets
				offsets = new Vector3[numOfOffsets];
				if (numOfOffsets == 1)
				{
					offsets[0] = new Vector3(fastRandom.Next(-250,250) / 1000f, 0, fastRandom.Next(-250,250) / 1000f);
					maxFireSize = fastRandom.Next(1300,1850) / 1000f;
				}
				else
				{
					offsets[0] = new Vector3(fastRandom.Next(-400,-200) / 1000f, 0, fastRandom.Next(-400, 400) / 1000f);
					offsets[1] = new Vector3(fastRandom.Next(200, 400) / 1000f, 0, fastRandom.Next(-400, 400) / 1000f);
					maxFireSize = fastRandom.Next(700,1000) / 1000f;
				}

				//Cache matrix and compensate offsets
				matrix = new Matrix4x4[numOfOffsets];
				for (int i = 0; i < numOfOffsets; ++i)
				{
					offsets[i] += fire.Position.ToVector3() + centering;
					matrix[i].SetTRS(new Vector3(offsets[i].x, fire.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.PawnState).y, offsets[i].z), Quaternion.identity, new Vector3(fire.fireSize, 1f, fire.fireSize));
				}

				//Cache cells
				//adjacentCells = fire.OccupiedRect().ExpandedBy(2).ToArray();
				
				//Lights
				//if (light) UpdateLights(fire, this);
			}
			
			public int frame;
			public Vector3[] offsets;
			public int numOfOffsets;
			public Matrix4x4[] matrix;
			public float maxFireSize;
			//public CompGlower firelight;
			//public ThingWithComps glowHolder;
			//public IntVec3[] adjacentCells;
		}
	
		public static void ThrowLongFireGlow(Vector3 c, Map map, float size)
		{
			if (!c.ShouldSpawnMotesAt(map)) return;
			
			Vector3 vector = c + size * new Vector3(fastRandom.Next(1, 100) / 100f - 0.5f, 0f, fastRandom.Next(1, 100) / 100f - 0.5f);
			if (!vector.InBounds(map)) return;

			FleckCreationData dataStatic = FleckMaker.GetDataStatic(vector, map, ResourceBank.FleckDefOf.Owl_FireGlow, fastRandom.Next(400, 600) / 100f * size);
			dataStatic.rotationRate = fastRandom.Next(-300, 300) / 100f;
			dataStatic.velocityAngle = fastRandom.Next(0, 360) / 100f;
			dataStatic.velocitySpeed = 0.12f;
			map.flecks.CreateFleck(dataStatic);
		}
	}
}