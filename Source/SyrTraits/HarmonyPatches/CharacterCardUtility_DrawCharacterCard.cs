using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SyrTraits;

[HarmonyPatch(typeof(CharacterCardUtility), nameof(CharacterCardUtility.DrawCharacterCard))]
public static class CharacterCardUtility_DrawCharacterCard
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var operand = AccessTools.Method(typeof(CharacterCardUtility_DrawCharacterCard),
            nameof(IndividualityCardButton));
        var list = instructions.ToList();
        var num = list.FindIndex(ins =>
            ins.IsStloc() && ins.operand is LocalBuilder { LocalIndex: 20 });
        list.InsertRange(num + 1, new List<CodeInstruction>
        {
            new CodeInstruction(OpCodes.Ldloca, 20),
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Ldarg_3),
            new CodeInstruction(OpCodes.Call, operand)
        });
        return list;
    }

    public static void IndividualityCardButton(ref float x, Rect rect, Pawn pawn, Rect creationRect)
    {
        if (pawn == null)
        {
            return;
        }

        TipSignal tip = "IndividualityTooltip".Translate();
        var rect2 = new Rect(x, 1f, 24f, 24f);
        x -= 40f;
        if (Current.ProgramState != ProgramState.Playing)
        {
            rect2 = new Rect(creationRect.width - 24f, 80f, 24f, 24f);
        }

        var color = GUI.color;
        GUI.color = rect2.Contains(Event.current.mousePosition)
            ? new Color(0.25f, 0.59f, 0.75f)
            : new Color(1f, 1f, 1f);

        GUI.DrawTexture(rect2, ContentFinder<Texture2D>.Get("Buttons/Individuality"));
        TooltipHandler.TipRegion(rect2, tip);
        if (Widgets.ButtonInvisible(rect2))
        {
            SoundDefOf.InfoCard_Open.PlayOneShotOnCamera();
            Find.WindowStack.Add(new Dialog_ViewIndividuality(pawn));
        }

        GUI.color = color;
    }
}