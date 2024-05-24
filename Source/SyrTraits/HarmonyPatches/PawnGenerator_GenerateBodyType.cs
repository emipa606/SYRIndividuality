using HarmonyLib;
using Verse;

namespace SyrTraits;

[HarmonyPatch(typeof(PawnGenerator), "GenerateBodyType")]
public static class PawnGenerator_GenerateBodyType
{
    public static void Postfix(Pawn pawn)
    {
        var compIndividuality = pawn.TryGetComp<CompIndividuality>();
        if (pawn != null && compIndividuality != null)
        {
            pawn.BroadcastCompSignal("bodyTypeSelected");
        }
    }
}