using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.SecondaryLovinChanceFactor))]
public static class Pawn_RelationsTracker_SecondaryLovinChanceFactor
{
    public static readonly SimpleCurve beautyCurve =
    [
        new CurvePoint(-10f, 0.01f),
        new CurvePoint(-2f, 0.3f),
        new CurvePoint(0f, 1f),
        new CurvePoint(1f, 1.8f),
        new CurvePoint(2f, 2.5f),
        new CurvePoint(5f, 3f),
        new CurvePoint(10f, 4f)
    ];

    public static void Postfix(ref float __result, Pawn ___pawn, Pawn otherPawn)
    {
        if (SyrIndividuality.RomanceDisabled)
        {
            return;
        }

        var compIndividuality = ___pawn.TryGetComp<CompIndividuality>();
        var num = 1f;
        if (___pawn == otherPawn)
        {
            __result = 0f;
        }

        if (compIndividuality != null && ___pawn.gender != otherPawn.gender)
        {
            switch (compIndividuality.sexuality)
            {
                case CompIndividuality.Sexuality.Straight:
                    num = 1f;
                    break;
                case CompIndividuality.Sexuality.Bisexual:
                    num = 0.75f;
                    break;
                case CompIndividuality.Sexuality.Gay:
                    num = 0.1f;
                    break;
                case CompIndividuality.Sexuality.Asexual:
                    num = 0f;
                    break;
            }
        }
        else if (compIndividuality != null && ___pawn.gender == otherPawn.gender)
        {
            switch (compIndividuality.sexuality)
            {
                case CompIndividuality.Sexuality.Gay:
                    num = 1f;
                    break;
                case CompIndividuality.Sexuality.Bisexual:
                    num = 0.75f;
                    break;
                case CompIndividuality.Sexuality.Straight:
                    num = 0.1f;
                    break;
                case CompIndividuality.Sexuality.Asexual:
                    num = 0f;
                    break;
            }
        }

        var ageBiologicalYearsFloat = ___pawn.ageTracker.AgeBiologicalYearsFloat;
        var ageBiologicalYearsFloat2 = otherPawn.ageTracker.AgeBiologicalYearsFloat;
        var num2 = 1f;
        if (___pawn.gender == Gender.Male)
        {
            var min = ageBiologicalYearsFloat - 30f;
            var lower = ageBiologicalYearsFloat - 10f;
            var upper = ageBiologicalYearsFloat + 5f;
            var max = ageBiologicalYearsFloat + 15f;
            num2 = GenMath.FlatHill(0.2f, min, lower, upper, max, 0.2f, ageBiologicalYearsFloat2);
        }
        else if (___pawn.gender == Gender.Female)
        {
            var min2 = ageBiologicalYearsFloat - 20f;
            var lower2 = ageBiologicalYearsFloat - 10f;
            var upper2 = ageBiologicalYearsFloat + 10f;
            var max2 = ageBiologicalYearsFloat + 30f;
            num2 = GenMath.FlatHill(0.2f, min2, lower2, upper2, max2, 0.2f, ageBiologicalYearsFloat2);
        }

        var num3 = 1f;
        num3 *= Mathf.Lerp(0.2f, 1f, otherPawn.health.capacities.GetLevel(PawnCapacityDefOf.Talking));
        var x = 0f;
        if (otherPawn.RaceProps.Humanlike)
        {
            x = otherPawn.GetStatValue(StatDefOf.PawnBeauty);
        }

        var num4 = beautyCurve.Evaluate(x);
        var num5 = Mathf.InverseLerp(16f, 18f, ageBiologicalYearsFloat);
        var num6 = Mathf.InverseLerp(16f, 18f, ageBiologicalYearsFloat2);
        __result = num * num2 * num3 * num4 * num5 * num6;
    }
}