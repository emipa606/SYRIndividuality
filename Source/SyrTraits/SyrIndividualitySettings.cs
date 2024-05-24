using Verse;

namespace SyrTraits;

public class SyrIndividualitySettings : ModSettings
{
    public static float commonalityStraight = 0.8f;

    public static float commonalityBi = 0.1f;

    public static float commonalityGay = 0.1f;

    public static float commonalityAsexual;

    public static IntRange traitCount = new IntRange(2, 3);

    public static bool disableRomance;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref commonalityStraight, "Individuality_CommonalityStraight", 0.8f, true);
        Scribe_Values.Look(ref commonalityBi, "Individuality_CommonalityBi", 0.1f, true);
        Scribe_Values.Look(ref commonalityGay, "Individuality_CommonalityGay", 0.1f, true);
        Scribe_Values.Look(ref commonalityAsexual, "Individuality_CommonalityAsexual", 0.1f, true);
        Scribe_Values.Look(ref traitCount, "Individuality_TraitCount", new IntRange(2, 3), true);
        Scribe_Values.Look(ref disableRomance, "Individuality_DisableRomance", false, true);
    }
}