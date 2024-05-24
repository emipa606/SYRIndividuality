using HarmonyLib;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(DamageWorker_AddInjury), "FinalizeAndAddInjury", typeof(Pawn), typeof(float), typeof(DamageInfo),
    typeof(DamageWorker.DamageResult))]
public static class DamageWorker_AddInjury_FinalizeAndAddInjury
{
    public static void Prefix(Pawn pawn, ref float totalDamage, DamageInfo dinfo,
        DamageWorker.DamageResult result)
    {
        var pawn2 = dinfo.Instigator as Pawn;
        if (pawn2?.story?.traits != null && pawn2.story.traits.HasTrait(SyrTraitDefOf.SYR_MechanoidExpert) &&
            pawn?.def?.race is { FleshType: not null, IsMechanoid: true })
        {
            totalDamage *= 1.5f;
        }
    }
}