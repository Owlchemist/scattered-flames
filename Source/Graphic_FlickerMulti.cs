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
			if (this.subGraphics == null || !fireCache.TryGetValue(thing, out SubFlame subFlame)) return;

			if (!Current.gameInt.tickManager.Paused)
			{
				float fireSize = ((Fire)thing).fireSize;
				if (nextFrame && ++subFlame.frame == base.subGraphics.Length) 
				{
					subFlame.frame = 0;
					for (int i = 0; i < subFlame.numOfOffsets; ++i)
					{
						subFlame.matrix[i].m00 = subFlame.matrix[i].m22 = Mathf.Min(subFlame.maxFireSize, fireSize) * fastRandom.Next(90,110) / 100f;
					}
				}

				if (fireSize > 0.5f)
				{
					if (fastRandom.Next(100) < 2) FleckMaker.ThrowMicroSparks(loc, thing.Map);
					if (fastRandom.Next(100) < 1) ThrowLongFireGlow(loc, thing.Map, fireSize);
					if (fastRandom.Next(100) < 1) FleckMaker.ThrowDustPuffThick(loc, thing.Map, fireSize * 2f, Color.yellow);
				}
			}

			for (int i = 0; i < subFlame.numOfOffsets; ++i)
			{
				Graphics.Internal_DrawMesh_Injected
				(
					MeshPool.plane10, //Mesh
					0, //SubMeshIndex
					ref subFlame.matrix[i], //Matrix
					this.subGraphics[subFlame.frame].MatSingle, //Material
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
