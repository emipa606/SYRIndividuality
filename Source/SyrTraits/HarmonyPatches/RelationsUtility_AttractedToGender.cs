using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(RelationsUtility), nameof(RelationsUtility.AttractedToGender))]
public static class RelationsUtility_AttractedToGender
{
    public static bool Prefix(Pawn pawn, Gender gender, ref bool __result)
    {
        if (SyrIndividuality.RomanceDisabled)
        {
            return true;
        }

        var compIndividuality = pawn?.TryGetComp<CompIndividuality>();
        if (compIndividuality == null)
        {
            return true;
        }

        switch (compIndividuality.sexuality)
        {
            case CompIndividuality.Sexuality.Straight:
                __result = pawn.gender != gender;
                break;
            case CompIndividuality.Sexuality.Gay:
                __result = pawn.gender == gender;
                break;
            case CompIndividuality.Sexuality.Bisexual:
                __result = true;
                break;
            case CompIndividuality.Sexuality.Asexual:
                break;
            case CompIndividuality.Sexuality.Undefined:
                return true;
        }

        return false;
    }
}