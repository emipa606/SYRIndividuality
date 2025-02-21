using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

using static StatWorker_GetValueUnfinalized.StatBonuses;

[HarmonyPatch(typeof(Trait), nameof(Trait.TipString))]
public static class Trait_TipString
{
    public static bool Prefix(ref string __result, Trait __instance, Pawn pawn)
    {
        if (__instance?.def is null || __instance.def != SyrTraitDefOf.SlowLearner)
        {
            return true;
        }

        StringBuilder stringBuilder = new StringBuilder();
        TraitDegreeData currentData = __instance.CurrentData;
        stringBuilder.Append(currentData.description.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn).Resolve());
        if (pawn.skills != null && pawn.def.statBases != null)
        {
            float slowLearnerTotalOffset = __instance.OffsetOfStat(StatDefOf.GlobalLearningFactor)
                                         + SlowLearnerBonus(pawn);
            
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"    {StatDefOf.GlobalLearningFactor.LabelCap} {slowLearnerTotalOffset.ToStringPercentSigned()}");
        }

        __result = stringBuilder.ToString();
        return false;
    }
}