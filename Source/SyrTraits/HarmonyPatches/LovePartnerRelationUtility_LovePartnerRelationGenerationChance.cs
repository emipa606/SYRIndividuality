using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(LovePartnerRelationUtility),
    nameof(LovePartnerRelationUtility.LovePartnerRelationGenerationChance))]
[HarmonyPriority(0)]
public static class LovePartnerRelationUtility_LovePartnerRelationGenerationChance
{
    private static readonly MethodInfo GetGenerationChanceAgeGapFactor =
        AccessTools.Method(typeof(LovePartnerRelationUtility), "GetGenerationChanceAgeGapFactor");

    public static void Postfix(ref float __result, Pawn generated, Pawn other, bool ex)
    {
        if (!SyrIndividuality.RomanceDisabled)
        {
            __result = LovePartnerRelationGenerationChance_Method(generated, other, ex);
        }
    }

    private static float LovePartnerRelationGenerationChance_Method(Pawn generated, Pawn other, bool ex)
    {
        if (generated.ageTracker.AgeBiologicalYearsFloat < 14f || other.ageTracker.AgeBiologicalYearsFloat < 14f)
        {
            return 0f;
        }

        var num = 1f;
        var num2 = 1f;
        var compIndividuality = other.TryGetComp<CompIndividuality>();
        if (generated.gender != other.gender && compIndividuality != null)
        {
            switch (compIndividuality.sexuality)
            {
                case CompIndividuality.Sexuality.Straight:
                    num2 = 1f;
                    break;
                case CompIndividuality.Sexuality.Bisexual:
                    num2 = 0.75f;
                    break;
                case CompIndividuality.Sexuality.Gay:
                    num2 = 0.05f;
                    break;
            }
        }

        if (generated.gender == other.gender && compIndividuality != null)
        {
            switch (compIndividuality.sexuality)
            {
                case CompIndividuality.Sexuality.Gay:
                    num2 = 1f;
                    break;
                case CompIndividuality.Sexuality.Bisexual:
                    num2 = 0.75f;
                    break;
                case CompIndividuality.Sexuality.Straight:
                    num2 = 0.05f;
                    break;
            }
        }

        if (ex)
        {
            var num3 = 0;
            var directRelations = other.relations.DirectRelations;
            foreach (var pawnRelation in directRelations)
            {
                if (LovePartnerRelationUtility.IsExLovePartnerRelation(pawnRelation.def))
                {
                    num3++;
                }
            }

            num = Mathf.Pow(0.2f, num3);
        }
        else if (LovePartnerRelationUtility.HasAnyLovePartner(other))
        {
            return 0f;
        }

        var num4 = Mathf.Clamp(GenMath.LerpDouble(14f, 27f, 0f, 1f, generated.ageTracker.AgeBiologicalYearsFloat), 0f,
            1f);
        var num5 = Mathf.Clamp(GenMath.LerpDouble(14f, 27f, 0f, 1f, other.ageTracker.AgeBiologicalYearsFloat), 0f, 1f);
        var num6 = (float)GetGenerationChanceAgeGapFactor.Invoke(null, [generated, other, ex]);
        var num7 = 1f;
        if (generated.GetRelations(other).Any(x => x.familyByBloodRelation))
        {
            num7 = 0.01f;
        }

        return num * num4 * num5 * num6 * num2 * num7;
    }
}