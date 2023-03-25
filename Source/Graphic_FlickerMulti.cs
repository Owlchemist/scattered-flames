using UnityEngine;
using UnityEngine.Rendering;
using Verse;
using RimWorld;
using static ScatteredFlames.ScatteredFlamesUtility;

namespace ScatteredFlames
{
	public class Graphic_FlickerMulti : Graphic_Flicker
	{
		public Graphic_FlickerMulti() {}

		public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
		{
			if (this.subGraphics == null || !fireCache.TryGetValue(thing.thingIDNumber, out FlameData subFlame)) return;

			var totalFrames = base.subGraphics.Length;
			if (curTimeSpeed != 0 && nextFrame && triggeringFrameID == RealTime.frameCount && subFlame.frame-- == 0)
			{
				float fireSize = subFlame.fire.fireSize;
				subFlame.frame = totalFrames - 1;
				for (int i = subFlame.numOfOffsets; i-- > 0;)
				{
					subFlame.matrix[i].m00 = subFlame.matrix[i].m22 = Mathf.Min(subFlame.maxFireSize, fireSize) * fastRandom.Next(90,110) / 100f;
				}
				if (ModSettings_ScatteredFlames.specialFX && !subFlame.roofed && fireSize > 0.5f && fastRandom.NextBool())
				{
					if (fastRandom.Next(3) == 0) FleckMaker.ThrowMicroSparks(loc, thing.Map);
					if (fireSize > 0.75f && fastRandom.Next(20) == 0) ThrowLongFireGlow(loc, thing.Map, fireSize);
					if (fastRandom.Next(30) == 0) FleckMaker.ThrowHeatGlow(thing.Position, thing.Map, fireSize);
					if (fastRandom.Next(5) == 0) FleckMaker.ThrowDustPuffThick(loc, thing.Map, fireSize * 2f, ResourceBank.color);
				}
			}

			for (int i = subFlame.numOfOffsets; i-- > 0;)
			{
				Graphics.Internal_DrawMesh_Injected
				(
					MeshPool.plane10, //Mesh
					0, //SubMeshIndex
					ref subFlame.matrix[i], //Matrix
					//((Graphic_Single)this.subGraphics[(subFlame.frame + i) % totalFrames]).mat, //Material
					((Graphic_Single)this.subGraphics[(subFlame.frame + i) % totalFrames]).mat, //Material
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