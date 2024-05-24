using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

public static class JobDriver_ConstructFinishFrame
{
    public static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions)
    {
        var GetStatValueAbstract = AccessTools.Method(typeof(StatExtension), nameof(StatExtension.GetStatValueAbstract),
        [
            typeof(BuildableDef),
            typeof(StatDef),
            typeof(ThingDef)
        ]);
        var modifyConstructionSpeed =
            AccessTools.Method(typeof(JobDriver_ConstructFinishFrame), nameof(ModifyConstructionSpeed));
        AccessTools.Property(typeof(Thing), nameof(Thing.Stuff)).GetGetMethod();
        var found = false;
        foreach (var i in instructions)
        {
            yield return i;
            if (i.opcode == OpCodes.Call && (MethodInfo)i.operand == GetStatValueAbstract)
            {
                found = true;
            }

            if (!found || i.opcode != OpCodes.Mul)
            {
                continue;
            }

            yield return new CodeInstruction(OpCodes.Ldloc_0);
            yield return new CodeInstruction(OpCodes.Ldloc_1);
            yield return new CodeInstruction(OpCodes.Call, modifyConstructionSpeed);
            yield return new CodeInstruction(OpCodes.Mul);
            found = false;
        }
    }

    public static float ModifyConstructionSpeed(Pawn pawn, Frame frame)
    {
        if (pawn?.story?.traits != null && frame?.Stuff?.stuffProps?.categories != null &&
            pawn.story.traits.HasTrait(SyrTraitDefOf.SYR_Architect) &&
            frame.Stuff.stuffProps.categories.Contains(StuffCategoryDefOf.Stony))
        {
            return 3f;
        }

        return 1f;
    }
}