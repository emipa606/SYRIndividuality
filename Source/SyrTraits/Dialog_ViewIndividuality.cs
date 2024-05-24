using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraits;

public class Dialog_ViewIndividuality(Pawn editFor) : Window
{
    public override Vector2 InitialSize => new Vector2(350f, 300f);

    public override void DoWindowContents(Rect inRect)
    {
        soundClose = SoundDefOf.InfoCard_Close;
        closeOnClickedOutside = true;
        absorbInputAroundWindow = false;
        forcePause = true;
        preventCameraMotion = false;
        doCloseX = true;
        closeOnAccept = true;
        closeOnCancel = true;
        var rect = new Rect(inRect.x - 10f, inRect.y, inRect.width + 10f, inRect.height);
        if (Find.WindowStack.IsOpen(typeof(Dialog_Trade)) || Current.ProgramState != ProgramState.Playing)
        {
            IndividualityCardUtility.DrawIndividualityCard(rect, editFor);
        }
        else
        {
            IndividualityCardUtility.DrawIndividualityCard(rect, Find.Selector.SingleSelectedThing as Pawn);
        }
    }
}