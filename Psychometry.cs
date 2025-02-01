using System;
using Qud.API;
using XRL.Language;
using XRL.World.Parts;
using HarmonyLib;

namespace GravensMutationMod.HarmonyPatches
{
	[HarmonyPatch(typeof(XRL.World.Parts.Mutation.Psychometry))]
	public class PsychoPartAdder
	{
		[HarmonyPrefix]
		[HarmonyPatch("Mutate")]
		static void Prefix(XRL.World.GameObject GO, XRL.World.Parts.Mutation.Psychometry __instance)
		{
			__instance.ParentObject.SetIntProperty("TechScannerEquipped", 1);
			__instance.ParentObject.SetIntProperty("StructureScannerEquipped", 1);
		}
	}
	
	[HarmonyPatch(typeof(XRL.World.Parts.Mutation.Psychometry))]
	public class PsychoPartRemover
	{
		[HarmonyPrefix]
		[HarmonyPatch("Unmutate")]
		static void Prefix(XRL.World.GameObject GO, XRL.World.Parts.Mutation.Psychometry __instance)
		{
			__instance.ParentObject.RemoveProperty("TechScannerEquipped");
			__instance.ParentObject.RemoveProperty("StructureScannerEquipped");
		}
	}
}