using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(Pawn_SkillTracker), nameof(Pawn_SkillTracker.GetSkill))]
public static class Pawn_SkillTracker_GetSkill
{
    public static void Postfix(ref SkillRecord __result, Pawn ___pawn)
    {
        if (___pawn.story?.traits == null)
        {
            return;
        }

        if (!___pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_GreenThumb) || !WorkGiver_GrowerSow_JobOnCell.greenThumb)
        {
            return;
        }

        var skillRecord = new SkillRecord(___pawn, SkillDefOf.Plants)
        {
            Level = 20
        };
        __result = skillRecord;
    }
}