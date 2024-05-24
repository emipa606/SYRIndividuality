using RimWorld;
using Verse;

namespace SyrTraits;

public class ThoughtWorker_IsCarryingMeleeWeapon : ThoughtWorker
{
    protected override ThoughtState CurrentStateInternal(Pawn p)
    {
        return p.equipment.Primary != null && p.equipment.Primary.def.IsMeleeWeapon;
    }
}