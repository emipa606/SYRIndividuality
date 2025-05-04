using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(ThoughtWorker_Disfigured), "CurrentSocialStateInternal")]
public static class ThoughtWorker_Disfigured_CurrentSocialStateInternal
{
    public static void Postfix(ref ThoughtState __result, Pawn other)
    {
        if (other?.story?.traits == null || !other.story.traits.HasTrait(SyrTraitDefOf.Beauty))
        {
            return;
        }

        var num = other.story.traits.DegreeOfTrait(SyrTraitDefOf.Beauty);
        if (num is -1 or -2)
        {
            __result = false;
        }
    }
}