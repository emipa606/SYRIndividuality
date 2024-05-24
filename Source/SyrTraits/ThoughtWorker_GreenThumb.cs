using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraits;

public class ThoughtWorker_GreenThumb : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        if (!p.IsColonist)
        {
            return ThoughtState.Inactive;
        }

        var ownedRoom = p.ownership.OwnedRoom;
        if (ownedRoom == null)
        {
            return ThoughtState.ActiveAtStage(0);
        }

        var num = 0;
        foreach (var beautyPlant in HarmonyPatches.beautyPlants)
        {
            num += ownedRoom.ThingCount(beautyPlant);
        }

        return ThoughtState.ActiveAtStage(Mathf.RoundToInt(Mathf.Clamp((num + 1f) / 2, 0f, 5f)));
    }
}