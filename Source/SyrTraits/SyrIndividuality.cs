using System.Linq;
using Mlie;
using UnityEngine;
using Verse;

namespace SyrTraits;

public class SyrIndividuality : Mod
{
    public static SyrIndividualitySettings settings;
    private static string currentVersion;
    private static bool forcedDisabled;

    public static bool RationalRomanceActive;
    public static bool WayBetterRomanceActive;
    public static bool PsychologyActive;

    public const string logPrefix = "[Individuality]:";

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
        var num = Mathf.Max(
            SyrIndividualitySettings.commonalityStraight + SyrIndividualitySettings.commonalityBi +
            SyrIndividualitySettings.commonalityGay + SyrIndividualitySettings.commonalityAsexual, 0.05f);
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

        listing_Standard.Gap(24f);
        if (!RomanceDisabled)
        {
            listing_Standard.Label("SyrTraitsSexualityCommonality".Translate());
            listing_Standard.Label("SyrTraitsSexualityCommonalityStraight".Translate() + ": " +
                                   (SyrIndividualitySettings.commonalityStraight / num).ToStringByStyle(ToStringStyle
                                       .PercentZero));
            SyrIndividualitySettings.commonalityStraight =
                listing_Standard.Slider(GenMath.RoundTo(SyrIndividualitySettings.commonalityStraight, 0.1f), 0f, 1f);
            listing_Standard.Label("SyrTraitsSexualityCommonalityBi".Translate() + ": " +
                                   (SyrIndividualitySettings.commonalityBi / num).ToStringByStyle(ToStringStyle
                                       .PercentZero));
            SyrIndividualitySettings.commonalityBi =
                listing_Standard.Slider(GenMath.RoundTo(SyrIndividualitySettings.commonalityBi, 0.1f), 0f, 1f);
            listing_Standard.Label("SyrTraitsSexualityCommonalityGay".Translate() + ": " +
                                   (SyrIndividualitySettings.commonalityGay / num).ToStringByStyle(ToStringStyle
                                       .PercentZero));
            SyrIndividualitySettings.commonalityGay =
                listing_Standard.Slider(GenMath.RoundTo(SyrIndividualitySettings.commonalityGay, 0.1f), 0f, 1f);
            listing_Standard.Label("SyrTraitsSexualityCommonalityAsexual".Translate() + ": " +
                                   (SyrIndividualitySettings.commonalityAsexual / num).ToStringByStyle(ToStringStyle
                                       .PercentZero));
            SyrIndividualitySettings.commonalityAsexual =
                listing_Standard.Slider(GenMath.RoundTo(SyrIndividualitySettings.commonalityAsexual, 0.1f), 0f, 1f);
            listing_Standard.Gap();
        }

        if (listing_Standard.ButtonText("SyrTraitsDefaultSettings".Translate(),
                "SyrTraitsDefaultSettingsTooltip".Translate()))
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
}