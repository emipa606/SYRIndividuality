using System.Text;
using RimWorld;
using Verse;

namespace SyrTraits;

public class StatWorker_Mass : StatWorker
{
    public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
    {
        var thing = req.Thing;
        if (thing.def.IsCorpse)
        {
            thing = (thing as Corpse)?.InnerPawn;
        }

        var compIndividuality = thing.TryGetComp<CompIndividuality>();
        return compIndividuality != null
            ? base.GetValueUnfinalized(req, applyPostProcess) + compIndividuality.BodyWeight
            : base.GetValueUnfinalized(req, applyPostProcess);
    }

    public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
    {
        var stringBuilder = new StringBuilder();
        var thing = req.Thing;
        if (thing.def.IsCorpse)
        {
            thing = (thing as Corpse)?.InnerPawn;
        }

        var compIndividuality = thing.TryGetComp<CompIndividuality>();
        if (compIndividuality == null)
        {
            return base.GetExplanationUnfinalized(req, numberSense);
        }

        stringBuilder.AppendLine("BodyWeightOffset".Translate() + ": " +
                                 stat.ValueToString(compIndividuality.BodyWeight));
        return $"{base.GetExplanationUnfinalized(req, numberSense)}\n{stringBuilder.ToString().TrimEndNewlines()}";
    }
}