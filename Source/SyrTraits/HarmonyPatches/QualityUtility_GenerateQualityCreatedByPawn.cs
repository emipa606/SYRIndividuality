using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(QualityUtility), nameof(QualityUtility.GenerateQualityCreatedByPawn), typeof(Pawn),
    typeof(SkillDef))]
[HarmonyPriority(0)]
public class QualityUtility_GenerateQualityCreatedByPawn
{
    public static void Postfix(ref QualityCategory __result, Pawn pawn)
    {
        if (pawn?.story?.traits == null || !pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_Perfectionist) ||
            !(Rand.Value < 0.5f))
        {
            return;
        }

        __result++;
        if ((int)__result > 6)
        {
            __result = QualityCategory.Legendary;
        }
    }
}