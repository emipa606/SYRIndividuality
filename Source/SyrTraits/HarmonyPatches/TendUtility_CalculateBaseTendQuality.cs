using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(TendUtility), nameof(TendUtility.CalculateBaseTendQuality), typeof(Pawn), typeof(Pawn),
    typeof(float), typeof(float))]
[HarmonyPriority(800)]
public static class TendUtility_CalculateBaseTendQuality
{
    public static void Postfix(ref float __result, Pawn doctor, Pawn patient,
        float medicinePotency, float medicineQualityMax)
    {
        if (doctor?.story?.traits == null || !doctor.story.traits.HasTrait(SyrTraitDefOf.SYR_SteadyHands))
        {
            return;
        }

        var num = doctor.GetStatValue(StatDefOf.MedicalTendQuality);
        num *= medicinePotency;
        var building_Bed = patient?.CurrentBed();
        if (building_Bed != null)
        {
            num += building_Bed.GetStatValue(StatDefOf.MedicalTendQualityOffset);
        }

        __result = Mathf.Clamp(num, 0f, medicineQualityMax);
    }
}