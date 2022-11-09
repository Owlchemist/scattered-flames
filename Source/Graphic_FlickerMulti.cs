using UnityEngine;
using UnityEngine.Rendering;
using Verse;
using RimWorld;
using static ScatteredFlames.ScatteredFlamesUtility;

namespace ScatteredFlames
{
	public class Graphic_FlickerMulti : Graphic_Flicker
	{
		public Graphic_FlickerMulti()
		{
		}

		public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
		{
			if (this.subGraphics == null || !fireCache.TryGetValue(thing.thingIDNumber, out FlameData subFlame)) return;

			if (!isPausedCache)
			{
				float fireSize = ((Fire)thing).fireSize;
				if (nextFrame && triggeringFrameID == frameID && ++subFlame.frame == base.subGraphics.Length) 
				{
					subFlame.frame = 0;
					for (int i = 0; i < subFlame.numOfOffsets; ++i)
					{
						subFlame.matrix[i].m00 = subFlame.matrix[i].m22 = Mathf.Min(subFlame.maxFireSize, fireSize) * fastRandom.Next(90,110) / 100f;
					}
					if (ModSettings_ScatteredFlames.specialFX && !subFlame.roofed && fireSize > 0.5f)
					{
						if (fastRandom.Next(5) < 1) FleckMaker.ThrowMicroSparks(loc, thing.Map);
						if (fireSize > 0.75f && fastRandom.Next(35) < 1) ThrowLongFireGlow(loc, thing.Map, fireSize);
						if (fastRandom.Next(60) < 1) FleckMaker.ThrowHeatGlow(thing.Position, thing.Map, fireSize);
						if (fastRandom.Next(10) < 1) FleckMaker.ThrowDustPuffThick(loc, thing.Map, fireSize * 2f, ResourceBank.color);
					}
				}
			}

			for (int i = 0; i < subFlame.numOfOffsets; ++i)
			{
				Graphics.Internal_DrawMesh_Injected
				(
					MeshPool.plane10, //Mesh
					0, //SubMeshIndex
					ref subFlame.matrix[i], //Matrix
					((Graphic_Single)this.subGraphics[subFlame.frame]).mat, //Material
					0, //Layer
					null, //Camera
					null, //MaterialPropertiesBlock
					ShadowCastingMode.Off, //castShadows
					false, //recieveShadows
					null, //probeAnchor
					LightProbeUsage.Off, //LightProbeUseage
					null //LightProbeProxyVolume
				);
			}
		}
	}
}
