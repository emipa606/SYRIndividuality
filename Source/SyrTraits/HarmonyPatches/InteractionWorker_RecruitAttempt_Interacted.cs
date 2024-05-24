using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), nameof(InteractionWorker_RecruitAttempt.Interacted))]
public static class InteractionWorker_RecruitAttempt_Interacted
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var wildness = AccessTools.Field(typeof(RaceProperties), "wildness");
        var modifyWildness =
            AccessTools.Method(typeof(InteractionWorker_RecruitAttempt_Interacted), nameof(ModifyWildness));
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldfld && (FieldInfo)instruction.operand == wildness)
            {
                yield return instruction;
                yield return new CodeInstruction(OpCodes.Ldarg_1);
                yield return new CodeInstruction(OpCodes.Call, modifyWildness);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    public static float ModifyWildness(float wildness, Pawn pawn)
    {
        if (pawn?.story?.traits != null && pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_AnimalAffinity) &&
            wildness < 1f)
        {
            return Mathf.Clamp(wildness * 0.9f, 0f, 2f);
        }

        return wildness;
    }
}