using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace SyrTraits;

public static class IndividualityCardUtility
{
    private static bool editMode;

    public static void DrawIndividualityCard(Rect rect, Pawn pawn)
    {
        var compIndividuality = pawn.TryGetComp<CompIndividuality>();
        if (pawn == null || compIndividuality == null)
        {
            return;
        }

        Widgets.ThingIcon(new Rect(0f, 10f, 40f, 40f), pawn);
        Text.Font = GameFont.Medium;
        var rect2 = new Rect(56f, 0f, rect.width, 30f);
        Widgets.Label(rect2, pawn.Name.ToStringShort);
        Text.Font = GameFont.Tiny;
        var num = rect2.y + rect2.height;
        var rect3 = new Rect(56f, num, rect.width, 24f);
        Widgets.Label(rect3, "IndividualityWindow".Translate());
        Text.Font = GameFont.Small;
        num += rect3.height + 17f;
        if (!SyrIndividuality.RomanceDisabled)
        {
            var rect4 = new Rect(0f, num, rect.width - 10f, 24f);
            Widgets.Label(new Rect(10f, num, rect.width, 24f),
                string.Concat("SexualityPawn".Translate() + ": ", compIndividuality.sexuality.ToString()));
            TipSignal tip = "SexualityPawnTooltip".Translate();
            TooltipHandler.TipRegion(rect4, tip);
            Widgets.DrawHighlightIfMouseover(rect4);
            num += rect4.height + 2f;
            var rect5 = new Rect(0f, num, rect.width - 10f, 24f);
            Widgets.Label(new Rect(10f, num, rect.width, 24f),
                "RomanceFactor".Translate() + ": " +
                (compIndividuality.RomanceFactor * 2f).ToStringByStyle(ToStringStyle.PercentZero));
            TipSignal tip2 = "RomanceFactorTooltip".Translate();
            TooltipHandler.TipRegion(rect5, tip2);
            Widgets.DrawHighlightIfMouseover(rect5);
            num += rect5.height + 2f;
            if (editMode)
            {
                if (ScrolledDown(rect4, true) || LeftClicked(rect4))
                {
                    SoundDefOf.DragSlider.PlayOneShotOnCamera();
                    compIndividuality.sexuality++;
                    if ((int)compIndividuality.sexuality > 4)
                    {
                        compIndividuality.sexuality = CompIndividuality.Sexuality.Straight;
                    }
                }
                else if (ScrolledUp(rect4, true) || RightClicked(rect4))
                {
                    SoundDefOf.DragSlider.PlayOneShotOnCamera();
                    compIndividuality.sexuality--;
                    if ((int)compIndividuality.sexuality < 1)
                    {
                        compIndividuality.sexuality = CompIndividuality.Sexuality.Asexual;
                    }
                }
                else if (ScrolledDown(rect5, true) || LeftClicked(rect5))
                {
                    SoundDefOf.DragSlider.PlayOneShotOnCamera();
                    compIndividuality.RomanceFactor += 0.1f;
                    if (compIndividuality.RomanceFactor > 1.05f)
                    {
                        compIndividuality.RomanceFactor = 0.1f;
                    }
                }
                else if (ScrolledUp(rect5, true) || RightClicked(rect5))
                {
                    SoundDefOf.DragSlider.PlayOneShotOnCamera();
                    compIndividuality.RomanceFactor -= 0.1f;
                    if (compIndividuality.RomanceFactor < 0.05f)
                    {
                        compIndividuality.RomanceFactor = 1f;
                    }
                }
            }
        }

        var rect6 = new Rect(0f, num, rect.width - 10f, 24f);
        Widgets.Label(new Rect(10f, num, rect.width, 24f),
            "PsychicFactor".Translate() + ": " +
            compIndividuality.PsychicFactor.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Offset));
        TipSignal tip3 = "PsychicFactorTooltip".Translate();
        TooltipHandler.TipRegion(rect6, tip3);
        Widgets.DrawHighlightIfMouseover(rect6);
        num += rect6.height + 2f;
        var rect7 = new Rect(0f, num, rect.width - 10f, 24f);
        Widgets.Label(new Rect(10f, num, rect.width, 24f),
            "BodyWeight".Translate() + ": " + (compIndividuality.BodyWeight +
                                               (pawn.def.statBases.Find(s => s.stat == StatDefOf.Mass).value *
                                                pawn.BodySize) + " kg (" + pawn.story.bodyType + ")"));
        TipSignal tip4 = "BodyWeightTooltip".Translate();
        TooltipHandler.TipRegion(rect7, tip4);
        Widgets.DrawHighlightIfMouseover(rect7);
        num += rect7.height + 7f;
        var rect8 = new Rect(10f, num, rect.width, 24f);
        if (editMode)
        {
            GUI.color = Color.red;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect8, "SyrIndividuality_EditModeTooltip".Translate());
            GUI.color = Color.white;
            Text.Font = GameFont.Small;
        }

        num += rect8.height + 13f;
        var rect9 = new Rect((rect.width / 2f) - 90f, num, 180f, 40f);
        if (Event.current.type == EventType.KeyDown)
        {
            _ = Event.current.keyCode == KeyCode.Mouse0;
        }
        else
        {
            _ = 0;
        }

        if (Event.current.type == EventType.KeyDown)
        {
            _ = Event.current.keyCode == KeyCode.Mouse1;
        }
        else
        {
            _ = 0;
        }

        if (!editMode)
        {
            if (!Widgets.ButtonText(rect9, "SyrIndividuality_EditModeOn".Translate()))
            {
                return;
            }

            SoundDefOf.Tick_High.PlayOneShotOnCamera();
            editMode = true;

            return;
        }

        if (Widgets.ButtonText(rect9, "SyrIndividuality_EditModeOff".Translate()))
        {
            SoundDefOf.Tick_Low.PlayOneShotOnCamera();
            editMode = false;
        }

        if (ScrolledDown(rect6, true) || LeftClicked(rect6))
        {
            SoundDefOf.DragSlider.PlayOneShotOnCamera();
            compIndividuality.PsychicFactor += 0.2f;
            if (compIndividuality.PsychicFactor > 1f)
            {
                compIndividuality.PsychicFactor = -1f;
            }
        }
        else if (ScrolledUp(rect6, true) || RightClicked(rect6))
        {
            SoundDefOf.DragSlider.PlayOneShotOnCamera();
            compIndividuality.PsychicFactor -= 0.2f;
            if (compIndividuality.PsychicFactor < -1f)
            {
                compIndividuality.PsychicFactor = 1f;
            }
        }
        else if (ScrolledDown(rect7, true) || LeftClicked(rect7))
        {
            SoundDefOf.DragSlider.PlayOneShotOnCamera();
            compIndividuality.BodyWeight += 10;
            if (compIndividuality.BodyWeight > 40)
            {
                compIndividuality.BodyWeight = -20;
            }
        }
        else if (ScrolledUp(rect7, true) || RightClicked(rect7))
        {
            SoundDefOf.DragSlider.PlayOneShotOnCamera();
            compIndividuality.BodyWeight -= 10;
            if (compIndividuality.BodyWeight < -20)
            {
                compIndividuality.BodyWeight = 40;
            }
        }
    }

    private static bool Scrolled(Rect rect, ScrollDirection direction, bool stopPropagation)
    {
        var num = Event.current.type == EventType.ScrollWheel &&
                  (Event.current.delta.y > 0f && direction == ScrollDirection.Up ||
                   Event.current.delta.y < 0f && direction == ScrollDirection.Down) && Mouse.IsOver(rect);
        if (num && stopPropagation)
        {
            Event.current.Use();
        }

        return num;
    }

    private static bool ScrolledUp(Rect rect, bool stopPropagation = false)
    {
        return Scrolled(rect, ScrollDirection.Up, stopPropagation);
    }

    private static bool ScrolledDown(Rect rect, bool stopPropagation = false)
    {
        return Scrolled(rect, ScrollDirection.Down, stopPropagation);
    }

    private static bool Clicked(Rect rect, int button = 0)
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == button)
        {
            return Mouse.IsOver(rect);
        }

        return false;
    }

    private static bool LeftClicked(Rect rect)
    {
        return Clicked(rect);
    }

    private static bool RightClicked(Rect rect)
    {
        return Clicked(rect, 1);
    }

    private enum ScrollDirection
    {
        Up,
        Down
    }
}