using System;
using System.Collections.Generic;
using System.Linq;
using Mlie;
using UnityEngine;
using Verse;
using static SyrTraits.CompIndividuality;

namespace SyrTraits;

public class SyrIndividuality : Mod
{
    public static SyrIndividualitySettings settings;
    private static string currentVersion;
    private static bool forcedDisabled;

    public static bool RationalRomanceActive;
    public static bool WayBetterRomanceActive;
    public static bool PsychologyActive;

    public SyrIndividuality(ModContentPack content)
        : base(content)
    {
        PsychologyActive = ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name.Contains("Psychology"));
        RationalRomanceActive = ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name.Contains("Rational Romance"));
        WayBetterRomanceActive = ModsConfig.IsActive("divineDerivative.Romance");
        forcedDisabled = PsychologyActive || RationalRomanceActive || WayBetterRomanceActive;
        settings = GetSettings<SyrIndividualitySettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    public static bool RomanceDisabled => forcedDisabled || SyrIndividualitySettings.disableRomance;

    public override string SettingsCategory()
    {
        return "SyrTraitsSettingsCategory".Translate();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        var listing_Standard = new Listing_Standard();
        listing_Standard.Begin(inRect);
        listing_Standard.Label("SyrTraitsTraitCount".Translate());
        listing_Standard.IntRange(ref SyrIndividualitySettings.traitCount, 0, 8);
        listing_Standard.Gap(24f);
        if (PsychologyActive || RationalRomanceActive || WayBetterRomanceActive)
        {
            GUI.color = Color.red;
            var text = "";
            if (PsychologyActive)
            {
                text = "SyrTraitsDisabledPsychology".Translate();
            }
            else if (RationalRomanceActive || WayBetterRomanceActive)
            {
                text = "SyrTraitsDisabledRationalRomance".Translate();
            }

            listing_Standard.Label("SyrTraitsDisabled".Translate() + ": " + text);
            GUI.color = Color.white;
        }
        else
        {
            listing_Standard.CheckboxLabeled("SyrTraitsDisableRomance".Translate(),
                ref SyrIndividualitySettings.disableRomance, "SyrTraitsDisableRomanceTooltip".Translate());
        }

        //Saving old orientation values
        float oldCommonalityStraight = SyrIndividualitySettings.commonalityStraight;
        float oldCommonalityBi = SyrIndividualitySettings.commonalityBi;
        float oldCommonalityGay = SyrIndividualitySettings.commonalityGay;
        float oldCommonalityAsexual = SyrIndividualitySettings.commonalityAsexual;

        //Drawing orienration commonality sliders
        listing_Standard.Gap(24f);
        if (!RomanceDisabled)
        {
            listing_Standard.Label("SyrTraitsSexualityCommonality".Translate());

            listing_Standard.Label("SyrTraitsSexualityCommonalityStraight".Translate() + ": " + SyrIndividualitySettings.commonalityStraight.ToStringByStyle(ToStringStyle.PercentZero));
            SyrIndividualitySettings.commonalityStraight = GenMath.RoundedHundredth(listing_Standard.Slider(SyrIndividualitySettings.commonalityStraight, 0f, 1f));

            listing_Standard.Label("SyrTraitsSexualityCommonalityBi".Translate() + ": " + SyrIndividualitySettings.commonalityBi.ToStringByStyle(ToStringStyle.PercentZero));
            SyrIndividualitySettings.commonalityBi = GenMath.RoundedHundredth(listing_Standard.Slider(SyrIndividualitySettings.commonalityBi, 0f, 1f));

            listing_Standard.Label("SyrTraitsSexualityCommonalityGay".Translate() + ": " + SyrIndividualitySettings.commonalityGay.ToStringByStyle(ToStringStyle.PercentZero));
            SyrIndividualitySettings.commonalityGay = GenMath.RoundedHundredth(listing_Standard.Slider(SyrIndividualitySettings.commonalityGay, 0f, 1f));

            listing_Standard.Label("SyrTraitsSexualityCommonalityAsexual".Translate() + ": " + SyrIndividualitySettings.commonalityAsexual.ToStringByStyle(ToStringStyle.PercentZero));
            SyrIndividualitySettings.commonalityAsexual = GenMath.RoundedHundredth(listing_Standard.Slider(SyrIndividualitySettings.commonalityAsexual, 0f, 1f));

            //If a commonality valie was changed, inversely change other commonalities
            if (oldCommonalityBi != SyrIndividualitySettings.commonalityBi)
            {
                ChangeOtherCommonalities(SyrIndividualitySettings.commonalityBi - oldCommonalityBi, Sexuality.Bisexual);
            }
            else if (oldCommonalityGay != SyrIndividualitySettings.commonalityGay)
            {
                ChangeOtherCommonalities(SyrIndividualitySettings.commonalityGay - oldCommonalityGay, Sexuality.Gay);
            }
            else if (oldCommonalityAsexual != SyrIndividualitySettings.commonalityAsexual)
            {
                ChangeOtherCommonalities(SyrIndividualitySettings.commonalityAsexual - oldCommonalityAsexual, Sexuality.Asexual);
            }
            else if (oldCommonalityStraight != SyrIndividualitySettings.commonalityStraight)
            {
                ChangeOtherCommonalities(SyrIndividualitySettings.commonalityStraight - oldCommonalityStraight, Sexuality.Straight);
            }

            //If the total somehow doesn't equal 1
            if (GenMath.RoundedHundredth(SyrIndividualitySettings.commonalityStraight + SyrIndividualitySettings.commonalityBi + SyrIndividualitySettings.commonalityGay + SyrIndividualitySettings.commonalityAsexual) != 1f)
            {
                Log.Error("Orientation chance total is not equal to 1! Resetting.");
                //Reset
                SyrIndividualitySettings.commonalityStraight = 0.8f;
                SyrIndividualitySettings.commonalityBi = 0.1f;
                SyrIndividualitySettings.commonalityGay = 0.1f;
                SyrIndividualitySettings.commonalityAsexual = 0f;
            }

            listing_Standard.Gap();
        }

        //Reset settings button
        if (listing_Standard.ButtonText("SyrTraitsDefaultSettings".Translate(), "SyrTraitsDefaultSettingsTooltip".Translate()))
        {
            SyrIndividualitySettings.traitCount.min = 2;
            SyrIndividualitySettings.traitCount.max = 3;
            SyrIndividualitySettings.commonalityStraight = 0.8f;
            SyrIndividualitySettings.commonalityBi = 0.1f;
            SyrIndividualitySettings.commonalityGay = 0.1f;
            SyrIndividualitySettings.commonalityAsexual = 0f;
        }

        if (currentVersion != null)
        {
            listing_Standard.Gap();
            GUI.contentColor = Color.gray;
            listing_Standard.Label("SyrTraitsCurrentModVersion".Translate(currentVersion));
            GUI.contentColor = Color.white;
        }

        listing_Standard.End();
    }

    //Increase/decrease some other commonality value(s) inversely to the one changed
    //Keep a list of changed commonalities to prevent a loop of increases/decreases
    private void ChangeOtherCommonalities(float delta, Sexuality sexuality) => ChangeOtherCommonalities(delta, [sexuality]);

    private void ChangeOtherCommonalities(float delta, List<Sexuality> sexualities)
    {
        if (!sexualities.Contains(Sexuality.Straight))
        {
            float newCommonality = GenMath.RoundedHundredth(SyrIndividualitySettings.commonalityStraight - delta);
            //In case of underflow
            if (newCommonality < 0f)
            {
                SyrIndividualitySettings.commonalityStraight = 0f;
                sexualities.Add(Sexuality.Straight); //Add the changed commonality to the list to ensure that it's not changed again
                ChangeOtherCommonalities(newCommonality * -1f, sexualities); //Recurse
                return;
            }
            //In case of overflow
            if (newCommonality > 1f)
            {
                SyrIndividualitySettings.commonalityStraight = 1f;
                sexualities.Add(Sexuality.Straight);
                ChangeOtherCommonalities(newCommonality - 1f, sexualities);
                return;
            }
            SyrIndividualitySettings.commonalityStraight = newCommonality;
            return;
        }
        if (!sexualities.Contains(Sexuality.Bisexual))
        {
            float newCommonality = GenMath.RoundedHundredth(SyrIndividualitySettings.commonalityBi - delta);
            if (newCommonality < 0f)
            {
                SyrIndividualitySettings.commonalityBi = 0f;
                sexualities.Add(Sexuality.Bisexual);
                ChangeOtherCommonalities(newCommonality * -1f, sexualities);
                return;
            }
            if (newCommonality > 1f)
            {
                SyrIndividualitySettings.commonalityBi = 1f;
                sexualities.Add(Sexuality.Bisexual);
                ChangeOtherCommonalities(newCommonality - 1f, sexualities);
                return;
            }
            SyrIndividualitySettings.commonalityBi = newCommonality;
            return;
        }
        if (!sexualities.Contains(Sexuality.Gay))
        {
            float newCommonality = GenMath.RoundedHundredth(SyrIndividualitySettings.commonalityGay - delta);
            if (newCommonality < 0f)
            {
                SyrIndividualitySettings.commonalityGay = 0f;
                sexualities.Add(Sexuality.Gay);
                ChangeOtherCommonalities(newCommonality * -1f, sexualities);
                return;
            }
            if (newCommonality > 1f)
            {
                SyrIndividualitySettings.commonalityGay = 1f;
                sexualities.Add(Sexuality.Gay);
                ChangeOtherCommonalities(newCommonality - 1f, sexualities);
                return;
            }
            SyrIndividualitySettings.commonalityGay = newCommonality;
            return;
        }
        if (!sexualities.Contains(Sexuality.Asexual))
        {
            float newCommonality = GenMath.RoundedHundredth(SyrIndividualitySettings.commonalityAsexual - delta);
            if (newCommonality < 0f)
            {
                SyrIndividualitySettings.commonalityAsexual = 0f;
                sexualities.Add(Sexuality.Asexual);
                ChangeOtherCommonalities(newCommonality * -1f, sexualities);
                return;
            }
            if (newCommonality > 1f)
            {
                SyrIndividualitySettings.commonalityAsexual = 1f;
                sexualities.Add(Sexuality.Asexual);
                ChangeOtherCommonalities(newCommonality - 1f, sexualities);
                return;
            }
            SyrIndividualitySettings.commonalityAsexual = newCommonality;
            return;
        }
        //Based on my testing, this should be unreachable, but I leave this here just in case
        Log.Error("[SYR] Individuality settings error: failed to balance sexual orientation commonality values.");
    }
}