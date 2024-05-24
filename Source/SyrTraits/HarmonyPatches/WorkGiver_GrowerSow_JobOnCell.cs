using HarmonyLib;
using RimWorld;

namespace SyrTraits;

[HarmonyPatch(typeof(WorkGiver_GrowerSow), nameof(WorkGiver_GrowerSow.JobOnCell))]
public static class WorkGiver_GrowerSow_JobOnCell
{
    public static bool greenThumb;

    public static void Prefix()
    {
        greenThumb = true;
    }

    public static void Postfix()
    {
        greenThumb = false;
    }
}