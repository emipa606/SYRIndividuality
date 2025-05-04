using Verse;

namespace SyrTraits;

[StaticConstructorOnStartup]
public static class CompToHumanlikes
{
    static CompToHumanlikes()
    {
        AddCompToHumanlikes();
    }

    private static void AddCompToHumanlikes()
    {
        foreach (var allDef in DefDatabase<ThingDef>.AllDefs)
        {
            if (allDef.race is { intelligence: Intelligence.Humanlike } && !allDef.IsCorpse)
            {
                allDef.comps.Add(new CompProperties_Individuality());
            }
        }
    }
}