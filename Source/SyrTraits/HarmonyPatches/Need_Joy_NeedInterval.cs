using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(Need_Joy), nameof(Need_Joy.NeedInterval))]
public static class Need_Joy_NeedInterval
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var get_FallPerInterval = AccessTools.Method(typeof(Need_Joy), "get_FallPerInterval");
        var GetJoyNeedRateMultiplier = AccessTools.Method(typeof(Need_Joy_NeedInterval),
            nameof(Need_Joy_NeedInterval.GetJoyNeedRateMultiplier));
        var pawn = AccessTools.Field(typeof(Need), "pawn");
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Call && (MethodInfo)instruction.operand == get_FallPerInterval)
            {
                yield return instruction;
                yield return new CodeInstruction(OpCodes.Ldarg_0);
                yield return new CodeInstruction(OpCodes.Ldfld, pawn);
                yield return new CodeInstruction(OpCodes.Call, GetJoyNeedRateMultiplier);
                yield return new CodeInstruction(OpCodes.Mul);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    public static float GetJoyNeedRateMultiplier(Pawn pawn)
    {
        return pawn.GetStatValue(SyrTraitDefOf.JoyNeedRateMultiplier);
    }
}