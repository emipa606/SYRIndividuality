using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetExplanationUnfinalized))]
public static class StatWorker_GetExplanationUnfinalized
{
    public static void Postfix(ref string __result, StatWorker __instance, StatRequest req,
        StatDef ___stat)
    {
        var pawn = req.Thing as Pawn;
        if (pawn?.story?.traits == null || ___stat == null)
        {
            return;
        }

        float num;
        float val;
        StringBuilder stringBuilder;
        if (___stat == StatDefOf.ResearchSpeed && pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_CreativeThinker) &&
            pawn.skills != null && pawn.def.statBases != null)
        {
            num = 1f;
            if (pawn.def.statBases.Find(x => x?.stat != null && x.stat == ___stat) != null)
            {
                num = pawn.def.statBases.Find(x => x?.stat != null && x.stat == ___stat).value;
            }

            val = 0.115f * pawn.skills.GetSkill(SkillDefOf.Artistic).Level * num;
            stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("SyrTraitsDaVinciGene".Translate());
            stringBuilder.AppendLine(
                $"{"    " + SkillDefOf.Artistic.LabelCap + " ("}{pawn.skills.GetSkill(SkillDefOf.Artistic).Level}): {val.ToStringSign()}{__instance.ValueToString(val, false)}");
            __result += stringBuilder.ToString();
            return;
        }

        if (___stat != StatDefOf.GlobalLearningFactor || !pawn.story.traits.HasTrait(SyrTraitDefOf.SlowLearner) ||
            pawn.skills == null || pawn.def.statBases == null)
        {
            return;
        }

        num = 0f;
        foreach (var skill in pawn.skills.skills)
        {
            num += skill.levelInt;
        }

        val = 0.02f * Mathf.Clamp(num, 40f, 140f);
        stringBuilder = new StringBuilder();
        stringBuilder.AppendLine("SyrTraitsSlowLearnerLabel".Translate());
        stringBuilder.AppendLine(
            $"{"    " + "SyrTraitsSlowLearner".Translate() + " ("}{num}): {val.ToStringSign()}{__instance.ValueToString(val, false)}");
        __result += stringBuilder.ToString();
    }
}