using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(InspirationHandler), nameof(InspirationHandler.InspirationHandlerTick))]
public class InspirationHandler_InspirationHandlerTick
{
    public static void Postfix(InspirationHandler __instance)
    {
        if (__instance.pawn.IsHashIntervalTick(100))
        {
            CheckStartTrait_RandomInspiration(__instance);
        }
    }

    private static void CheckStartTrait_RandomInspiration(InspirationHandler __instance)
    {
        if (__instance.Inspired || __instance.pawn?.story == null)
        {
            return;
        }

        var allTraits = __instance.pawn.story.traits.allTraits;
        foreach (var trait in allTraits)
        {
            if (trait.CurrentData is not RandomInspirationMtbDays
                {
                    randomInspirationMtbDays: > 0f
                } randomInspirationMtbDays ||
                !Rand.MTBEventOccurs(randomInspirationMtbDays.randomInspirationMtbDays, 60000f, 100f))
            {
                continue;
            }

            var randomAvailableInspirationDef = GetRandomAvailableInspirationDef(__instance);
            if (randomAvailableInspirationDef != null)
            {
                __instance.TryStartInspiration(randomAvailableInspirationDef);
            }
        }
    }

    private static InspirationDef GetRandomAvailableInspirationDef(InspirationHandler __instance)
    {
        return DefDatabase<InspirationDef>.AllDefsListForReading
            .Where(x => x.Worker.InspirationCanOccur(__instance.pawn) && x != SyrTraitDefOf.Inspired_Surgery &&
                        x != SyrTraitDefOf.Frenzy_Shoot)
            .RandomElementByWeightWithFallback(x => x.Worker.CommonalityFor(__instance.pawn));
    }
}