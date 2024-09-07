using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(CompAbilityEffect_WordOfLove), nameof(CompAbilityEffect_WordOfLove.ValidateTarget))]
public static class CompAbilityEffect_WordOfLove_ValidateTarget
{
    public static bool Prefix(ref bool __result, LocalTargetInfo target,
        LocalTargetInfo ___selectedTarget)
    {
        if (SyrIndividuality.RomanceDisabled)
        {
            return true;
        }

        var pawn = ___selectedTarget.Pawn;
        var pawn2 = target.Pawn;
        var compIndividuality = pawn.TryGetComp<CompIndividuality>();
        if (pawn == pawn2)
        {
            __result = false;
            return false;
        }

        if (pawn != null && pawn2 != null && compIndividuality.sexuality != CompIndividuality.Sexuality.Bisexual &&
            (pawn.gender == pawn2.gender && compIndividuality.sexuality == CompIndividuality.Sexuality.Straight ||
             pawn.gender != pawn2.gender && compIndividuality.sexuality == CompIndividuality.Sexuality.Gay))
        {
            Messages.Message("AbilityCantApplyWrongAttractionGender".Translate(pawn, pawn2), pawn,
                MessageTypeDefOf.RejectInput, false);
            __result = false;
            return false;
        }

        __result = true;
        return false;
    }
}