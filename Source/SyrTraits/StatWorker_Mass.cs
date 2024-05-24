using System.Text;
using RimWorld;
using Verse;

namespace SyrTraits;

public class StatWorker_Mass : StatWorker
{
    public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
    {
        if (req.Thing.TryGetComp<CompIndividuality>() != null)
        {
            return base.GetValueUnfinalized(req, applyPostProcess) +
                   req.Thing.TryGetComp<CompIndividuality>().BodyWeight;
        }

        return base.GetValueUnfinalized(req, applyPostProcess);
    }

    public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
    {
        var stringBuilder = new StringBuilder();
        var compIndividuality = req.Thing.TryGetComp<CompIndividuality>();
        if (compIndividuality == null)
        {
            return base.GetExplanationUnfinalized(req, numberSense);
        }

        stringBuilder.AppendLine("BodyWeightOffset".Translate() + ": " +
                                 stat.ValueToString(compIndividuality.BodyWeight));
        return $"{base.GetExplanationUnfinalized(req, numberSense)}\n{stringBuilder.ToString().TrimEndNewlines()}";
    }
}