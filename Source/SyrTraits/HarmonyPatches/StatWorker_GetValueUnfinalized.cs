using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized))]
public static class StatWorker_GetValueUnfinalized
{
    public static void Postfix(ref float __result, StatRequest req, StatDef ___stat)
    {
        var pawn = req.Thing as Pawn;
        if (pawn?.story?.traits == null || ___stat == null)
        {
            return;
        }

        float num;
        if (pawn.story.traits.HasTrait(SyrTraitDefOf.SlowLearner) && ___stat == StatDefOf.GlobalLearningFactor &&
            pawn.skills?.skills != null && pawn.def.statBases != null)
        {
            num = 0f;
            foreach (var skill in pawn.skills.skills)
            {
                num += skill.levelInt;
            }

            __result += 0.02f * Mathf.Clamp(num, 40f, 140f);
            return;
        }

        if (!pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_CreativeThinker) || ___stat != StatDefOf.ResearchSpeed ||
            pawn.skills == null || pawn.def.statBases == null)
        {
            return;
        }

        num = 1f;
        if (pawn.def.statBases.Find(x => x?.stat != null && x.stat == ___stat) != null)
        {
            num = pawn.def.statBases.Find(x => x?.stat != null && x.stat == ___stat).value;
        }

        __result += 0.115f * pawn.skills.GetSkill(SkillDefOf.Artistic).Level * num;
    }
}