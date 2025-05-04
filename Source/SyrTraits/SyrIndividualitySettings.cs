using Verse;

namespace SyrTraits;

public class SyrIndividualitySettings : ModSettings
{
    public static float CommonalityStraight = 0.8f;

    public static float CommonalityBi = 0.1f;

    public static float CommonalityGay = 0.1f;

    public static float CommonalityAsexual;

    public static IntRange TraitCount = new IntRange(2, 3);

    public static bool DisableRomance;

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref CommonalityStraight, "Individuality_CommonalityStraight", 0.8f, true);
        Scribe_Values.Look(ref CommonalityBi, "Individuality_CommonalityBi", 0.1f, true);
        Scribe_Values.Look(ref CommonalityGay, "Individuality_CommonalityGay", 0.1f, true);
        Scribe_Values.Look(ref CommonalityAsexual, "Individuality_CommonalityAsexual", 0.1f, true);
        Scribe_Values.Look(ref TraitCount, "Individuality_TraitCount", new IntRange(2, 3), true);
        Scribe_Values.Look(ref DisableRomance, "Individuality_DisableRomance", false, true);
    }
}