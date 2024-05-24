using HarmonyLib;
using Verse;
using Verse.AI;

namespace SyrTraits;

[HarmonyPatch(typeof(Pawn_MindState), "CheckStartMentalStateBecauseRecruitAttempted")]
public static class Pawn_MindState_CheckStartMentalStateBecauseRecruitAttempted
{
    public static bool Prefix(ref bool __result, Pawn tamer)
    {
        if (tamer?.story?.traits == null || !tamer.story.traits.HasTrait(SyrTraitDefOf.SYR_AnimalAffinity))
        {
            return true;
        }

        __result = false;
        return false;
    }
}