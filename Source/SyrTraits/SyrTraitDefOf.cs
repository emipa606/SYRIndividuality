using RimWorld;

namespace SyrTraits;

[DefOf]
public static class SyrTraitDefOf
{
    public static InspirationDef Frenzy_Shoot;
    public static InspirationDef Inspired_Surgery;

    public static StatDef JoyNeedRateMultiplier;

    public static TraitDef Jealous;

    public static TraitDef Neurotic;

    public static TraitDef SlowLearner;

    public static TraitDef SYR_GreenThumb;

    public static TraitDef SYR_KeenEye;

    public static TraitDef SYR_CreativeThinker;

    public static TraitDef SYR_MechanoidExpert;

    public static TraitDef SYR_Architect;

    public static TraitDef SYR_Perfectionist;

    public static TraitDef Gourmand;

    public static TraitDef SYR_SteadyHands;

    public static TraitDef SYR_HandEyeCoordination;

    public static TraitDef SYR_Haggler;

    public static TraitDef SYR_AnimalAffinity;
    public static TraitDef PsychicSensitivity;
    public static TraitDef Beauty;

    static SyrTraitDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(SyrTraitDefOf));
    }
}