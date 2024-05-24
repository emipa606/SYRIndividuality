using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(ThoughtWorker_ApparelThought), "CurrentStateInternal")]
public static class ThoughtWorker_ApparelThought_CurrentStateInternal
{
    public static void Postfix(ref ThoughtState __result, Pawn p, ThoughtWorker_ApparelThought __instance)
    {
        if (__instance is ThoughtWorker_DeadMansApparel && p.story.traits.HasTrait(SyrTraitDefOf.Jealous))
        {
            __result = ThoughtState.Inactive;
        }
    }
}