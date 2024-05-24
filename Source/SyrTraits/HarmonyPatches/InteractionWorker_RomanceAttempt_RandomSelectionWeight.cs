using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), nameof(InteractionWorker_RomanceAttempt.RandomSelectionWeight))]
public static class InteractionWorker_RomanceAttempt_RandomSelectionWeight
{
    public static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
    {
        if (!SyrIndividuality.RomanceDisabled)
        {
            __result = RandomSelectionWeight_Method(initiator, recipient);
        }
    }

    private static float RandomSelectionWeight_Method(Pawn initiator, Pawn recipient)
    {
        var compIndividuality = recipient.TryGetComp<CompIndividuality>();
        var compIndividuality2 = initiator.TryGetComp<CompIndividuality>();
        if (LovePartnerRelationUtility.LovePartnerRelationExists(initiator, recipient))
        {
            return 0f;
        }

        var num = initiator.relations.SecondaryRomanceChanceFactor(recipient);
        if (num < 0.15f)
        {
            return 0f;
        }

        var num2 = initiator.relations.OpinionOf(recipient);
        if (num2 < 5)
        {
            return 0f;
        }

        if (recipient.relations.OpinionOf(initiator) < 5)
        {
            return 0f;
        }

        var num3 = 1f;
        var pawn = LovePartnerRelationUtility.ExistingMostLikedLovePartner(initiator, false);
        if (pawn != null)
        {
            float value = initiator.relations.OpinionOf(pawn);
            num3 = Mathf.InverseLerp(50f, -50f, value);
        }

        var num4 = compIndividuality == null ? 1f : compIndividuality.RomanceFactor * 2f;
        var num5 = Mathf.InverseLerp(0.15f, 1f, num);
        var num6 = Mathf.InverseLerp(5f, 100f, num2);
        var num7 = 1f;
        if (initiator.gender != recipient.gender && compIndividuality != null && compIndividuality2 != null)
        {
            switch (compIndividuality.sexuality)
            {
                case CompIndividuality.Sexuality.Straight:
                    num7 = 1f;
                    break;
                case CompIndividuality.Sexuality.Bisexual:
                    num7 = 0.75f;
                    break;
                case CompIndividuality.Sexuality.Gay:
                    num7 = 0.1f;
                    break;
                case CompIndividuality.Sexuality.Asexual when
                    compIndividuality2.sexuality == CompIndividuality.Sexuality.Asexual:
                    num7 = 1f;
                    break;
            }
        }

        if (initiator.gender != recipient.gender || compIndividuality == null || compIndividuality2 == null)
        {
            return 1.15f * num4 * num5 * num6 * num3 * num7;
        }

        switch (compIndividuality.sexuality)
        {
            case CompIndividuality.Sexuality.Gay:
                num7 = 1f;
                break;
            case CompIndividuality.Sexuality.Bisexual:
                num7 = 0.75f;
                break;
            case CompIndividuality.Sexuality.Straight:
                num7 = 0.1f;
                break;
            case CompIndividuality.Sexuality.Asexual when
                compIndividuality2.sexuality == CompIndividuality.Sexuality.Asexual:
                num7 = 0.5f;
                break;
        }

        return 1.15f * num4 * num5 * num6 * num3 * num7;
    }
}