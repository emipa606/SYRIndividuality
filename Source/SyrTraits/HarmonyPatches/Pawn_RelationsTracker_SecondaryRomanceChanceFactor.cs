using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.SecondaryRomanceChanceFactor))]
public static class Pawn_RelationsTracker_SecondaryRomanceChanceFactor
{
    public static void Postfix(ref float __result, Pawn ___pawn, Pawn otherPawn)
    {
        if (SyrIndividuality.RomanceDisabled || ___pawn == null || otherPawn == null)
        {
            return;
        }

        var compIndividuality = otherPawn.TryGetComp<CompIndividuality>();
        var compIndividuality2 = ___pawn.TryGetComp<CompIndividuality>();
        if (compIndividuality2 != null && compIndividuality is { sexuality: CompIndividuality.Sexuality.Asexual } &&
            compIndividuality2.sexuality == CompIndividuality.Sexuality.Asexual)
        {
            __result = 2f;
        }
    }
}