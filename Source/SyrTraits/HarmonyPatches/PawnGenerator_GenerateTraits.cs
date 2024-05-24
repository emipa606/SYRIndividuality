using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(PawnGenerator), "GenerateTraits")]
public static class PawnGenerator_GenerateTraits
{
    public static void Prefix(ref PawnGenerationRequest request)
    {
        request.MinimumAgeTraits = SyrIndividualitySettings.traitCount.min;
        request.MaximumAgeTraits = SyrIndividualitySettings.traitCount.max;
    }

    public static void Postfix(Pawn pawn)
    {
        var compIndividuality = pawn.TryGetComp<CompIndividuality>();
        if (pawn != null && compIndividuality != null)
        {
            pawn.BroadcastCompSignal("traitsGenerated");
        }
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var RangeInclusive = AccessTools.Method(typeof(Rand), nameof(Rand.RangeInclusive));
        var IntRange = AccessTools.Field(typeof(SyrIndividualitySettings), nameof(SyrIndividualitySettings.traitCount));
        var min = AccessTools.Field(typeof(IntRange), "min");
        var max = AccessTools.Field(typeof(IntRange), "max");
        foreach (var i in instructions)
        {
            if (i.opcode == OpCodes.Call && (MethodInfo)i.operand == RangeInclusive)
            {
                yield return new CodeInstruction(OpCodes.Pop);
                yield return new CodeInstruction(OpCodes.Pop);
                yield return new CodeInstruction(OpCodes.Ldsfld, IntRange);
                yield return new CodeInstruction(OpCodes.Ldfld, min);
                yield return new CodeInstruction(OpCodes.Ldsfld, IntRange);
                yield return new CodeInstruction(OpCodes.Ldfld, max);
                yield return i;
            }
            else
            {
                yield return i;
            }
        }
    }
}