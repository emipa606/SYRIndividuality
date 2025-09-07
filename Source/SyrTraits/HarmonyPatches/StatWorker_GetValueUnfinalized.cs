using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraits;

using static StatWorker_GetValueUnfinalized.StatBonuses;

[HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetValueUnfinalized))]
public static class StatWorker_GetValueUnfinalized
{
    public static class StatBonuses
    {
        public const float baseSlowLearnerBonus = 0.02f;
        public const int minSkillsBeforeSlowLearnerBonus = 40;
        public const int maxSlowLearnerBonusMultiplier = 100;

        public const float baseCreativeThinkerBonus = 0.115f;

        internal static int TotalSkillLevel(Pawn pawn)
        {
            int totalSkillLevel = 0;
            foreach (SkillRecord skill in pawn.skills.skills)
                totalSkillLevel += skill.levelInt;

            return totalSkillLevel;
        }

        internal static float SlowLearnerBonus(int totalSkillLevel)
        {
            int bonusMultiplier = Mathf.Clamp(totalSkillLevel - minSkillsBeforeSlowLearnerBonus, 0, maxSlowLearnerBonusMultiplier);
            return baseSlowLearnerBonus * bonusMultiplier;
        }

        public static float SlowLearnerBonus(Pawn pawn) => SlowLearnerBonus(TotalSkillLevel(pawn));

        public static float CreativeThinkerBonus(Pawn pawn)
        {
            StatModifier researchSpeedStatBase = pawn.def.statBases.Find(x => x != null && x.stat == StatDefOf.ResearchSpeed);
            float baseResearchSpeed = researchSpeedStatBase?.value ?? 1f;

            return baseCreativeThinkerBonus * pawn.skills.GetSkill(SkillDefOf.Artistic).Level * baseResearchSpeed;
        }
    }

    private static readonly MethodInfo _getBaseValueForMethod = typeof(StatWorker).GetMethod(nameof(StatWorker.GetBaseValueFor));
    private static readonly FieldInfo _traitsField = typeof(Pawn_StoryTracker).GetField(nameof(Pawn_StoryTracker.traits));
    private static readonly FieldInfo _hediffSetField = typeof(Pawn_HealthTracker).GetField(nameof(Pawn_HealthTracker.hediffSet));
    private static readonly FieldInfo _statField = typeof(StatWorker).Field("stat");
    private static readonly MethodInfo _applyCustomTraitOffsetsMethod = typeof(StatWorker_GetValueUnfinalized).Method(nameof(ApplyCustomTraitOffsets));

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher codeMatcher = new CodeMatcher(instructions);

        // Getting the stat value variable
        codeMatcher.MatchStartForward(CodeMatch.Calls(_getBaseValueForMethod));
        if (codeMatcher.IsInvalid)
        {
            Log.Error($"{SyrIndividuality.logPrefix} Could not find call instruction for StatWorker.GetBaseValueFor in StatWorker.GetValueUnfinalized.");
            return instructions;
        }

        codeMatcher.MatchStartForward(CodeMatch.StoresLocal());
        int statValueLocalIndes = codeMatcher.Instruction.LocalIndex();

        // Finding the correct location for the new code
        codeMatcher.MatchStartForward(CodeMatch.LoadsField(_traitsField));
        if (codeMatcher.IsInvalid)
        {
            Log.Error($"{SyrIndividuality.logPrefix} Could not find load field instruction for Pawn_StoryTracker.traits in StatWorker.GetValueUnfinalized.");
            return instructions;
        }

        codeMatcher.MatchStartForward(CodeMatch.LoadsField(_hediffSetField));
        if (codeMatcher.IsInvalid)
        {
            Log.Error($"{SyrIndividuality.logPrefix} Could not find load field instruction for Pawn_HealthTracker.hediffSet in StatWorker.GetValueUnfinalized.");
            return instructions;
        }

        codeMatcher.MatchStartBackwards(CodeMatch.Branches());
        if (codeMatcher.IsInvalid)
        {
            Log.Error($"{SyrIndividuality.logPrefix} Could not find loop branch for applying trait offsets in StatWorker.GetValueUnfinalized.");
            return instructions;
        }
        codeMatcher.Advance(1); // we are right after the trait offsets loop now

        // New code to change the stat value
        List<CodeInstruction> newInstructions = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldfld, _statField),
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Ldloca, statValueLocalIndes),
            new CodeInstruction(OpCodes.Call, _applyCustomTraitOffsetsMethod)
        };
        codeMatcher.Insert(newInstructions);

        return codeMatcher.InstructionEnumeration();
    }

    private static void ApplyCustomTraitOffsets(StatDef stat, StatRequest req, ref float statValue)
    {
        Pawn pawn = req.Thing as Pawn;
        if (pawn.skills?.skills is null || pawn.def.statBases is null)
            return;

        TraitSet pawnTraits = pawn.story.traits;
        if (stat == StatDefOf.GlobalLearningFactor && pawnTraits.HasTrait(SyrTraitDefOf.SlowLearner))
            statValue += SlowLearnerBonus(pawn);
        else if (stat == StatDefOf.ResearchSpeed && pawnTraits.HasTrait(SyrTraitDefOf.SYR_CreativeThinker))
            statValue += CreativeThinkerBonus(pawn);
    }
}