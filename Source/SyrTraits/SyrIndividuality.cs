using System.Collections.Generic;
using System.Linq;
using Mlie;
using UnityEngine;
using Verse;
using static SyrTraits.CompIndividuality;

namespace SyrTraits;

public class SyrIndividuality : Mod
{
    private static string currentVersion;
    private static bool forcedDisabled;
    private static bool rationalRomanceActive;
    private static bool wayBetterRomanceActive;
    private static bool psychologyActive;

    public const string logPrefix = "[Individuality]:";

    public SyrIndividuality(ModContentPack content)
        : base(content)
    {
        psychologyActive = ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name.Contains("Psychology"));
        rationalRomanceActive = ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name.Contains("Rational Romance"));
        wayBetterRomanceActive = ModsConfig.IsActive("divineDerivative.Romance");
        forcedDisabled = psychologyActive || rationalRomanceActive || wayBetterRomanceActive;
        GetSettings<SyrIndividualitySettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public static bool RomanceDisabled => forcedDisabled || SyrIndividualitySettings.DisableRomance;

    public override string SettingsCategory()
    {
        return "SyrTraitsSettingsCategory".Translate();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listingStandard = new Listing_Standard();
        listingStandard.Begin(inRect);
        listingStandard.Label("SyrTraitsTraitCount".Translate());
        listingStandard.IntRange(ref SyrIndividualitySettings.TraitCount, 0, 8);
        listingStandard.Gap(24f);
        if (psychologyActive || rationalRomanceActive || wayBetterRomanceActive)
        {
            GUI.color = Color.red;
            var text = "";
            if (psychologyActive)
            {
                text = "SyrTraitsDisabledPsychology".Translate();
            }
            else if (rationalRomanceActive || wayBetterRomanceActive)
            {
                text = "SyrTraitsDisabledRationalRomance".Translate();
            }

            listingStandard.Label("SyrTraitsDisabled".Translate() + ": " + text);
            GUI.color = Color.white;
        }
        else
        {
            listingStandard.CheckboxLabeled("SyrTraitsDisableRomance".Translate(),
                ref SyrIndividualitySettings.DisableRomance, "SyrTraitsDisableRomanceTooltip".Translate());
        }


        //Drawing orientation commonality sliders
        listingStandard.Gap(24f);
        if (!RomanceDisabled)
        {
            listingStandard.Label("SyrTraitsSexualityCommonality".Translate());

            var oldCommonalityStraight = SyrIndividualitySettings.CommonalityStraight;
            listingStandard.Label("SyrTraitsSexualityCommonalityStraight".Translate() + ": " +
                                  SyrIndividualitySettings.CommonalityStraight.ToStringByStyle(ToStringStyle
                                      .PercentZero));
            SyrIndividualitySettings.CommonalityStraight =
                GenMath.RoundedHundredth(listingStandard.Slider(SyrIndividualitySettings.CommonalityStraight, 0f, 1f));
            if (oldCommonalityStraight != SyrIndividualitySettings.CommonalityStraight)
            {
                ChangeOtherCommonalities(SyrIndividualitySettings.CommonalityStraight - oldCommonalityStraight,
                    Sexuality.Straight);
            }

            var oldCommonalityBi = SyrIndividualitySettings.CommonalityBi;
            listingStandard.Label("SyrTraitsSexualityCommonalityBi".Translate() + ": " +
                                  SyrIndividualitySettings.CommonalityBi.ToStringByStyle(ToStringStyle.PercentZero));
            SyrIndividualitySettings.CommonalityBi =
                GenMath.RoundedHundredth(listingStandard.Slider(SyrIndividualitySettings.CommonalityBi, 0f, 1f));
            if (oldCommonalityBi != SyrIndividualitySettings.CommonalityBi)
            {
                ChangeOtherCommonalities(SyrIndividualitySettings.CommonalityBi - oldCommonalityBi, Sexuality.Bisexual);
            }

            var oldCommonalityGay = SyrIndividualitySettings.CommonalityGay;
            listingStandard.Label("SyrTraitsSexualityCommonalityGay".Translate() + ": " +
                                  SyrIndividualitySettings.CommonalityGay.ToStringByStyle(ToStringStyle.PercentZero));
            SyrIndividualitySettings.CommonalityGay =
                GenMath.RoundedHundredth(listingStandard.Slider(SyrIndividualitySettings.CommonalityGay, 0f, 1f));
            if (oldCommonalityGay != SyrIndividualitySettings.CommonalityGay)
            {
                ChangeOtherCommonalities(SyrIndividualitySettings.CommonalityGay - oldCommonalityGay, Sexuality.Gay);
            }

            var oldCommonalityAsexual = SyrIndividualitySettings.CommonalityAsexual;
            listingStandard.Label("SyrTraitsSexualityCommonalityAsexual".Translate() + ": " +
                                  SyrIndividualitySettings.CommonalityAsexual.ToStringByStyle(
                                      ToStringStyle.PercentZero));
            SyrIndividualitySettings.CommonalityAsexual =
                GenMath.RoundedHundredth(listingStandard.Slider(SyrIndividualitySettings.CommonalityAsexual, 0f, 1f));
            if (oldCommonalityAsexual != SyrIndividualitySettings.CommonalityAsexual)
            {
                ChangeOtherCommonalities(SyrIndividualitySettings.CommonalityAsexual - oldCommonalityAsexual,
                    Sexuality.Asexual);
            }

            //If the total somehow doesn't equal 1
            if (GenMath.RoundedHundredth(SyrIndividualitySettings.CommonalityStraight +
                                         SyrIndividualitySettings.CommonalityBi +
                                         SyrIndividualitySettings.CommonalityGay +
                                         SyrIndividualitySettings.CommonalityAsexual) != 1f)
            {
                Log.Warning("[SYR] Individuality: Orientation chance total is not equal to 1! Resetting.");
                //Reset
                SyrIndividualitySettings.CommonalityStraight = 0.8f;
                SyrIndividualitySettings.CommonalityBi = 0.1f;
                SyrIndividualitySettings.CommonalityGay = 0.1f;
                SyrIndividualitySettings.CommonalityAsexual = 0f;
            }

            listingStandard.Gap();
        }

        //Reset settings button
        if (listingStandard.ButtonText("SyrTraitsDefaultSettings".Translate(),
                "SyrTraitsDefaultSettingsTooltip".Translate()))
        {
            SyrIndividualitySettings.TraitCount.min = 2;
            SyrIndividualitySettings.TraitCount.max = 3;
            SyrIndividualitySettings.CommonalityStraight = 0.8f;
            SyrIndividualitySettings.CommonalityBi = 0.1f;
            SyrIndividualitySettings.CommonalityGay = 0.1f;
            SyrIndividualitySettings.CommonalityAsexual = 0f;
        }

        if (currentVersion != null)
        {
            listingStandard.Gap();
            GUI.contentColor = Color.gray;
            listingStandard.Label("SyrTraitsCurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listingStandard.End();
    }

    //Increase/decrease some other commonality value(s) inversely to the one changed
    //Keep a list of changed commonalities to prevent a loop of increases/decreases
    private static void ChangeOtherCommonalities(float delta, Sexuality sexuality)
    {
        List<Sexuality> sexualities = [sexuality];
        while (true)
        {
            if (!sexualities.Contains(Sexuality.Straight))
            {
                var newCommonality = GenMath.RoundedHundredth(SyrIndividualitySettings.CommonalityStraight - delta);
                switch (newCommonality)
                {
                    //In case of underflow
                    case < 0f:
                        SyrIndividualitySettings.CommonalityStraight = 0f;
                        sexualities.Add(Sexuality
                            .Straight); //Add the changed commonality to the list to ensure that it's not changed again
                        delta = newCommonality * -1f;
                        continue;
                    //In case of overflow
                    case > 1f:
                        SyrIndividualitySettings.CommonalityStraight = 1f;
                        sexualities.Add(Sexuality.Straight);
                        delta = newCommonality - 1f;
                        continue;
                    default:
                        SyrIndividualitySettings.CommonalityStraight = newCommonality;
                        return;
                }
            }

            if (!sexualities.Contains(Sexuality.Bisexual))
            {
                var newCommonality = GenMath.RoundedHundredth(SyrIndividualitySettings.CommonalityBi - delta);
                switch (newCommonality)
                {
                    case < 0f:
                        SyrIndividualitySettings.CommonalityBi = 0f;
                        sexualities.Add(Sexuality.Bisexual);
                        delta = newCommonality * -1f;
                        continue;
                    case > 1f:
                        SyrIndividualitySettings.CommonalityBi = 1f;
                        sexualities.Add(Sexuality.Bisexual);
                        delta = newCommonality - 1f;
                        continue;
                    default:
                        SyrIndividualitySettings.CommonalityBi = newCommonality;
                        return;
                }
            }

            if (!sexualities.Contains(Sexuality.Gay))
            {
                var newCommonality = GenMath.RoundedHundredth(SyrIndividualitySettings.CommonalityGay - delta);
                switch (newCommonality)
                {
                    case < 0f:
                        SyrIndividualitySettings.CommonalityGay = 0f;
                        sexualities.Add(Sexuality.Gay);
                        delta = newCommonality * -1f;
                        continue;
                    case > 1f:
                        SyrIndividualitySettings.CommonalityGay = 1f;
                        sexualities.Add(Sexuality.Gay);
                        delta = newCommonality - 1f;
                        continue;
                    default:
                        SyrIndividualitySettings.CommonalityGay = newCommonality;
                        return;
                }
            }

            if (!sexualities.Contains(Sexuality.Asexual))
            {
                var newCommonality = GenMath.RoundedHundredth(SyrIndividualitySettings.CommonalityAsexual - delta);
                switch (newCommonality)
                {
                    case < 0f:
                        SyrIndividualitySettings.CommonalityAsexual = 0f;
                        sexualities.Add(Sexuality.Asexual);
                        delta = newCommonality * -1f;
                        continue;
                    case > 1f:
                        SyrIndividualitySettings.CommonalityAsexual = 1f;
                        sexualities.Add(Sexuality.Asexual);
                        delta = newCommonality - 1f;
                        continue;
                    default:
                        SyrIndividualitySettings.CommonalityAsexual = newCommonality;
                        return;
                }
            }

            //Based on my testing, this should be unreachable, but I leave this here just in case
            Log.Error("[SYR] Individuality: Settings error, failed to balance sexual orientation commonality values.");
            break;
        }
    }
}