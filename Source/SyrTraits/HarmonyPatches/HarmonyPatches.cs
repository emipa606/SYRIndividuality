using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

[StaticConstructorOnStartup]
public static class HarmonyPatches
{
    public static readonly IEnumerable<ThingDef> beautyPlants;

    static HarmonyPatches()
    {
        var harmony = new Harmony("Syrchalis.Rimworld.Traits");
        harmony.Patch(
            typeof(RimWorld.JobDriver_ConstructFinishFrame)
                .GetNestedType("<>c__DisplayClass8_0", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetMethod("<MakeNewToils>b__1", BindingFlags.Instance | BindingFlags.NonPublic), null, null,
            new HarmonyMethod(
                typeof(JobDriver_ConstructFinishFrame).GetMethod(nameof(JobDriver_ConstructFinishFrame
                    .Transpiler))));
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        beautyPlants =
            DefDatabase<ThingDef>.AllDefs.Where(x => x?.plant is { purpose: PlantPurpose.Beauty });
    }
}