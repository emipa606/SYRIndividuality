using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(Mineable), "TrySpawnYield", typeof(Map), typeof(bool), typeof(Pawn))]
public static class Mineable_TrySpawnYield
{
    public static void Postfix(Map map, Pawn pawn)
    {
        if (pawn?.story?.traits == null || !pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_KeenEye) ||
            !(Rand.Value < FactorFromHilliness(map.TileInfo.hilliness)))
        {
            return;
        }

        var thingDef = DefDatabase<ThingDef>.AllDefs.RandomElementByWeightWithFallback(delegate(ThingDef d)
        {
            if (d.building == null)
            {
                return 0f;
            }

            return d.building.mineableYield < 5
                ? d.building.mineableScatterCommonality * 0.2f * d.building.mineableYield
                : d.building.mineableScatterCommonality;
        });
        var stackCount = Mathf.Max(1,
            Mathf.RoundToInt(thingDef.building.mineableYield * Find.Storyteller.difficulty.mineYieldFactor * 0.2f *
                             pawn.GetStatValue(StatDefOf.MiningYield)));
        var thing = ThingMaker.MakeThing(thingDef.building.mineableThing);
        thing.stackCount = stackCount;
        GenSpawn.Spawn(thing, pawn.Position, map);
        if (!pawn.IsColonist && thing.def.EverHaulable && !thing.def.designateHaulable)
        {
            thing.SetForbidden(true);
        }
    }

    public static float FactorFromHilliness(Hilliness hilliness)
    {
        switch (hilliness)
        {
            case Hilliness.Flat:
                return 0.2f;
            case Hilliness.SmallHills:
                return 0.12f;
            case Hilliness.LargeHills:
                return 0.1f;
            default:
                _ = 4;
                return 0.05f;
        }
    }
}