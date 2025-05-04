using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraits;

public class CompIndividuality : ThingComp
{
    public enum Sexuality : byte
    {
        Undefined,
        Straight,
        Bisexual,
        Gay,
        Asexual
    }

    private readonly Sexuality[] SexualityArray =
    [
        Sexuality.Straight,
        Sexuality.Bisexual,
        Sexuality.Gay,
        Sexuality.Asexual
    ];

    public int BodyWeight = -29;

    public float PsychicFactor = -2f;

    public float RomanceFactor = -1f;

    public Sexuality sexuality;

    public CompProperties_Individuality Props => (CompProperties_Individuality)props;

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        if (parent is Pawn)
        {
            ReplaceVanillaTraits();
        }

        IndividualityValueSetup();
    }

    public override void ReceiveCompSignal(string signal)
    {
        switch (signal)
        {
            case "bodyTypeSelected" when parent is Pawn pawn:
                BodyWeight = RandomBodyWeightByBodyType(pawn);
                break;
            case "traitsGenerated":
                ReplaceVanillaTraits();
                break;
        }
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        base.PostSpawnSetup(respawningAfterLoad);
        IndividualityValueSetup();
        ReplaceVanillaTraits();
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref sexuality, "Individuality_Sexuality");
        Scribe_Values.Look(ref BodyWeight, "Individuality_BodyWeight", -29);
        Scribe_Values.Look(ref RomanceFactor, "Individuality_RomanceFactor", -1f);
        Scribe_Values.Look(ref PsychicFactor, "Individuality_PsychicFactor", -2f);
    }

    private void IndividualityValueSetup()
    {
        var pawn = parent as Pawn;
        switch (parent.def.defName)
        {
            case "ChjDroid":
            {
                if (sexuality == Sexuality.Undefined)
                {
                    sexuality = Sexuality.Asexual;
                }

                if (RomanceFactor == -1f)
                {
                    RomanceFactor = 0f;
                }

                if (PsychicFactor == -2f)
                {
                    PsychicFactor = RandomPsychicFactor();
                }

                break;
            }
            case "Harpy" when sexuality == Sexuality.Undefined:
                sexuality = Sexuality.Bisexual;
                break;
        }

        if (sexuality == Sexuality.Undefined)
        {
            sexuality = RandomSexualityByWeight();
        }

        if (RomanceFactor == -1f)
        {
            RomanceFactor = GenMath.RoundTo(Rand.Range(0.1f, 1f), 0.1f);
        }

        if (PsychicFactor == -2f)
        {
            PsychicFactor = RandomPsychicFactor();
        }

        if (pawn != null && BodyWeight == -29)
        {
            BodyWeight = RandomBodyWeightByBodyType(pawn);
        }
    }

    private static float RandomPsychicFactor()
    {
        var num = Mathf.Clamp(Rand.Gaussian(0f, 0.5f), -1f, 1f);
        if (num > -0.3f && num < 0.3)
        {
            num = 0f;
        }

        return GenMath.RoundTo(num, 0.2f);
    }

    private int RandomBodyWeightByBodyType(Pawn pawn)
    {
        if (pawn?.story?.bodyType == null)
        {
            return -29;
        }

        var value = parent.def.defName == "Harpy" ? GenMath.RoundTo(Rand.Range(0, 10), 10) :
            pawn.story.bodyType == BodyTypeDefOf.Fat ? GenMath.RoundTo(Rand.Range(30, 40), 10) :
            pawn.story.bodyType == BodyTypeDefOf.Hulk ? GenMath.RoundTo(Rand.Range(10, 20), 10) :
            pawn.story.bodyType == BodyTypeDefOf.Thin ? GenMath.RoundTo(Rand.Range(-20, -10), 10) :
            pawn.story.bodyType != BodyTypeDefOf.Female ? GenMath.RoundTo(Rand.Range(0, 10), 10) :
            GenMath.RoundTo(Rand.Range(-10, 0), 10);
        return Mathf.Clamp(value, -20, 40);
    }

    private Sexuality RandomSexualityByWeight()
    {
        return SexualityArray.RandomElementByWeight(Probability);
    }

    private static float Probability(Sexuality val)
    {
        return val switch
        {
            Sexuality.Straight => SyrIndividualitySettings.CommonalityStraight,
            Sexuality.Bisexual => SyrIndividualitySettings.CommonalityBi,
            Sexuality.Gay => SyrIndividualitySettings.CommonalityGay,
            Sexuality.Asexual => SyrIndividualitySettings.CommonalityAsexual,
            _ => 0f
        };
    }

    private void ReplaceVanillaTraits()
    {
        var thingWithComps = parent;
        if (thingWithComps is not Pawn pawn)
        {
            return;
        }

        var compIndividuality = pawn.TryGetComp<CompIndividuality>();
        if (compIndividuality == null || pawn.story?.traits == null || !pawn.story.traits.HasTrait(TraitDefOf.Gay) &&
            !pawn.story.traits.HasTrait(TraitDefOf.Bisexual) && !pawn.story.traits.HasTrait(TraitDefOf.Asexual) &&
            !pawn.story.traits.HasTrait(SyrTraitDefOf.PsychicSensitivity))
        {
            return;
        }

        if (pawn.story.traits.HasTrait(TraitDefOf.Gay))
        {
            compIndividuality.sexuality = Sexuality.Gay;
        }
        else if (pawn.story.traits.HasTrait(TraitDefOf.Bisexual))
        {
            compIndividuality.sexuality = Sexuality.Bisexual;
        }
        else if (pawn.story.traits.HasTrait(TraitDefOf.Asexual))
        {
            compIndividuality.sexuality = Sexuality.Asexual;
        }

        if (pawn.story.traits.HasTrait(SyrTraitDefOf.PsychicSensitivity))
        {
            switch (pawn.story.traits.GetTrait(SyrTraitDefOf.PsychicSensitivity).Degree)
            {
                case -2:
                    compIndividuality.PsychicFactor = -1f;
                    break;
                case -1:
                    compIndividuality.PsychicFactor = -0.4f;
                    break;
                case 1:
                    compIndividuality.PsychicFactor = 0.4f;
                    break;
                case 2:
                    compIndividuality.PsychicFactor = 0.8f;
                    break;
            }
        }

        if (SyrIndividuality.RomanceDisabled)
        {
            return;
        }

        pawn.story.traits.allTraits.RemoveAll(t =>
            t.def == TraitDefOf.Bisexual || t.def == TraitDefOf.Asexual || t.def == TraitDefOf.Gay ||
            t.def == SyrTraitDefOf.PsychicSensitivity);
        IEnumerable<TraitDef> allDefsListForReading = DefDatabase<TraitDef>.AllDefsListForReading;
        var newTraitDef = allDefsListForReading.RandomElementByWeight(WeightSelector);
        if (pawn.story.traits.HasTrait(newTraitDef) ||
            pawn.Faction != null && Faction.OfPlayerSilentFail != null && pawn.Faction.HostileTo(Faction.OfPlayer) &&
            !newTraitDef.allowOnHostileSpawn ||
            pawn.story.traits.allTraits.Any(tr => newTraitDef.ConflictsWith(tr)) ||
            newTraitDef.requiredWorkTypes != null && pawn.OneOfWorkTypesIsDisabled(newTraitDef.requiredWorkTypes) ||
            pawn.WorkTagIsDisabled(newTraitDef.requiredWorkTags))
        {
            return;
        }

        var degree = PawnGenerator.RandomTraitDegree(newTraitDef);
        if (pawn.story.Childhood != null && (pawn.story.Childhood.DisallowsTrait(newTraitDef, degree) ||
                                             pawn.story.Adulthood != null &&
                                             pawn.story.Adulthood.DisallowsTrait(newTraitDef, degree)))
        {
            return;
        }

        var trait = new Trait(newTraitDef, degree);
        pawn.story.traits.GainTrait(trait);

        return;

        float WeightSelector(TraitDef tr)
        {
            return tr.GetGenderSpecificCommonality(pawn.gender);
        }
    }
}