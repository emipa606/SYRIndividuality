using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(CompAbilityEffect_WordOfLove), nameof(CompAbilityEffect_WordOfLove.Valid))]
public static class CompAbilityEffect_WordOfLove_Valid
{
    public static bool Prefix(ref bool __result, LocalTargetInfo target,
        bool throwMessages)
    {
        var pawn = target.Pawn;
        var compIndividuality = pawn.TryGetComp<CompIndividuality>();
        if (pawn != null)
        {
            if (compIndividuality.sexuality == CompIndividuality.Sexuality.Asexual)
            {
                if (throwMessages)
                {
                    Messages.Message("AbilityCantApplyOnAsexual".Translate(pawn), pawn, MessageTypeDefOf.RejectInput,
                        false);
                }

                __result = false;
                return false;
            }

            if (pawn.InMentalState)
            {
                __result = false;
                return false;
            }
        }

        __result = true;
        return false;
    }
}