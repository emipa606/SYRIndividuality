using HarmonyLib;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(Command_SetPlantToGrow), "WarnAsAppropriate")]
public static class Command_SetPlantToGrow_WarnAsAppropriate
{
    public static void Prefix()
    {
        WorkGiver_GrowerSow_JobOnCell.greenThumb = true;
    }

    public static void Postfix()
    {
        WorkGiver_GrowerSow_JobOnCell.greenThumb = false;
    }
}