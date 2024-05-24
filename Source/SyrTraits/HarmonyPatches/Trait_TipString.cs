using System.Linq;
using System.Text;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(Trait), nameof(Trait.TipString))]
public static class Trait_TipString
{
    public static bool Prefix(ref string __result, Trait __instance, Pawn pawn)
    {
        if (__instance?.def == null || __instance.def != SyrTraitDefOf.SlowLearner)
        {
            return true;
        }

        var stringBuilder = new StringBuilder();
        var currentData = __instance.CurrentData;
        stringBuilder.Append(currentData.description.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn).Resolve());
        if (pawn.skills != null && pawn.def.statBases != null)
        {
            var num = 0f;
            foreach (var skill in pawn.skills.skills)
            {
                num += skill.levelInt;
            }

            var f = (SyrTraitDefOf.SlowLearner.degreeDatas.First().statOffsets
                         .Find(x => x?.stat != null && x.stat == StatDefOf.GlobalLearningFactor).value +
                     (0.02f * Mathf.Clamp(num, 40f, 140f))) * 100f;
            string value = "    " + StatDefOf.GlobalLearningFactor.LabelCap + " " + f.ToStringWithSign() + "%";
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(value);
        }

        __result = stringBuilder.ToString();
        return false;
    }
}