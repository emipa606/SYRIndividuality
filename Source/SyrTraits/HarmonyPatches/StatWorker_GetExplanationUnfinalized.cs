using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using RimWorld;
using Verse;

namespace SyrTraits;

using static StatWorker_GetValueUnfinalized.StatBonuses;

[HarmonyPatch(typeof(StatWorker), nameof(StatWorker.GetExplanationUnfinalized))]
public static class StatWorker_GetExplanationUnfinalized
{
    private static readonly FieldInfo _storyField = typeof(Pawn).GetField(nameof(Pawn.story));
    private static readonly FieldInfo _traitsField = typeof(Pawn_StoryTracker).GetField(nameof(Pawn_StoryTracker.traits));
    private static readonly FieldInfo _creativeThinkerField = typeof(SyrTraitDefOf).GetField(nameof(SyrTraitDefOf.SYR_CreativeThinker));
    private static readonly MethodInfo _hasTraitMethod = typeof(TraitSet).GetMethod(nameof(TraitSet.HasTrait), [typeof(TraitDef)]);
    private static readonly MethodInfo _countGetter = typeof(List<Trait>).PropertyGetter(nameof(List<Trait>.Count));
    private static readonly FieldInfo _statField = typeof(StatWorker).Field("stat");
    private static readonly MethodInfo _addCustomTraitsExplanationsMethod = typeof(StatWorker_GetExplanationUnfinalized).Method(nameof(AddCustomTraitsExplanations));

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CodeMatcher codeMatcher = new CodeMatcher(instructions);

        // Finding the required variables
        Func<CodeInstruction, bool> stringBuilderPredicate = instruction =>
        {
            if (instruction.opcode != OpCodes.Newobj)
                return false;

            ConstructorInfo constructor = (ConstructorInfo)instruction.operand;
            return typeof(StringBuilder).IsAssignableFrom(constructor.DeclaringType);
        };
        CodeMatch stringBuilderMatch = new CodeMatch(stringBuilderPredicate);
        codeMatcher.MatchStartForward(stringBuilderMatch);
        codeMatcher.MatchStartForward(CodeMatch.StoresLocal());
        codeMatcher.ThrowIfInvalid($"{SyrIndividuality.logPrefix} Could not find StringBuilder variable in StatWorker.GetExplanationUnfinalized");
        int stringBuilderLocalIndex = codeMatcher.Instruction.LocalIndex();
        
        CodeMatch pawnMatch = new CodeMatch(OpCodes.Isinst, typeof(Pawn));
        codeMatcher.MatchStartForward(pawnMatch);
        codeMatcher.MatchStartForward(CodeMatch.StoresLocal());
        codeMatcher.ThrowIfInvalid($"{SyrIndividuality.logPrefix} Could not find Pawn variable in StatWorker.GetExplanationUnfinalized");
        int pawnLocalIndex = codeMatcher.Instruction.LocalIndex();

        // Entering the relevant section
        codeMatcher.MatchStartForward(CodeMatch.LoadsField(_traitsField));
        codeMatcher.ThrowIfInvalid($"{SyrIndividuality.logPrefix} Could not find load field instruction for Pawn_StoryTracker.traits in StatWorker.GetExplanationUnfinalized");

        codeMatcher.MatchStartForward(CodeMatch.LoadsConstant("StatsReport_RelevantTraits"));
        codeMatcher.MatchStartBackwards(new CodeMatch(instruction => instruction.labels?.Count > 0));
        codeMatcher.ThrowIfInvalid($"{SyrIndividuality.logPrefix} Could not find jump location for relevant traits explanations in StatWorker.GetExplanationUnfinalized");
        List<Label> labels = codeMatcher.Instruction.labels;

        /* 
         * We need to change the condition for the trait offsets and factors code
         * to be run, because Creative Thinker would not make the code run since
         * the offsets it gives are all custom code and don't use the normal system
         * at all
        */
        codeMatcher.MatchStartBackwards(new CodeMatch(instruction =>
            instruction.operand is Label label && labels.Contains(label)));
        codeMatcher.ThrowIfInvalid($"{SyrIndividuality.logPrefix} Could not find branch to relevant traits explanations in StatWorker.GetExplanationUnfinalized");
        Label relevantTraitsLabel = (Label)codeMatcher.Instruction.operand;

        codeMatcher.MatchStartForward(new CodeMatch(instruction =>
            instruction.operand is Label label && !labels.Contains(label)));
        codeMatcher.ThrowIfInvalid($"{SyrIndividuality.logPrefix} Could not find branch that avoids relevant traits explanations in StatWorker.GetExplanationUnfinalized");
        Label noRelevantTraitsLabel = (Label)codeMatcher.Instruction.operand;
        codeMatcher.Opcode = codeMatcher.Opcode.Complement();
        codeMatcher.Operand = relevantTraitsLabel;

        codeMatcher.Advance(1);
        List<CodeInstruction> newConditionInstructions =
        [
            new CodeInstruction(OpCodes.Ldloc_S, pawnLocalIndex),
            new CodeInstruction(OpCodes.Ldfld, _storyField),
            new CodeInstruction(OpCodes.Ldfld, _traitsField),
            new CodeInstruction(OpCodes.Ldsfld, _creativeThinkerField),
            new CodeInstruction(OpCodes.Callvirt, _hasTraitMethod),
            new CodeInstruction(OpCodes.Brfalse, noRelevantTraitsLabel)
        ];
        codeMatcher.InsertAndAdvance(newConditionInstructions);

        // Adding the new explanations
        codeMatcher.MatchStartForward(new CodeMatch() { opcodeSet = [OpCodes.Br, OpCodes.Br_S] });
        codeMatcher.ThrowIfInvalid($"{SyrIndividuality.logPrefix} Could not find start branch to trait offsets loop.");
        Label traitOffsetsConditionLabel = (Label)codeMatcher.Instruction.operand;

        CodeMatch tocLabelMatch = new CodeMatch(ci => ci.labels.Contains(traitOffsetsConditionLabel));
        codeMatcher.MatchStartForward(tocLabelMatch);
        codeMatcher.ThrowIfInvalid($"{SyrIndividuality.logPrefix} Could not find start of trait offsets loop condition.");

        codeMatcher.MatchStartForward(CodeMatch.Branches());
        codeMatcher.ThrowIfInvalid($"{SyrIndividuality.logPrefix} Could not branch to trait offsets loop body.");
        codeMatcher.Advance(1); // we are right after the trait offsets loop now

        List<CodeInstruction> newInstructions = new List<CodeInstruction>()
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Dup),
            new CodeInstruction(OpCodes.Ldfld, _statField),
            new CodeInstruction(OpCodes.Ldarg_1),
            new CodeInstruction(OpCodes.Ldloc_S, stringBuilderLocalIndex),
            new CodeInstruction(OpCodes.Call, _addCustomTraitsExplanationsMethod)
        };
        codeMatcher.Insert(newInstructions);

        return codeMatcher.InstructionEnumeration();
    }

    private static void AddCustomTraitsExplanations(StatWorker instance, StatDef stat, StatRequest req, StringBuilder stringBuilder)
    {
        Pawn pawn = req.Thing as Pawn;
        if (pawn.skills?.skills is null || pawn.def.statBases is null)
            return;

        TraitSet pawnTraits = pawn.story.traits;
        if (stat == StatDefOf.GlobalLearningFactor && pawnTraits.HasTrait(SyrTraitDefOf.SlowLearner))
        {
            Trait slowLearner = pawnTraits.GetTrait(SyrTraitDefOf.SlowLearner);
            int totalSkillLevel = TotalSkillLevel(pawn);
            float slowLearnerTotalOffset = slowLearner.OffsetOfStat(stat)
                                         + SlowLearnerBonus(totalSkillLevel);
            int insertionIndex = RemoveSlowLearnerStatDisplay(stringBuilder, slowLearner);
            
            stringBuilder.Insert(insertionIndex, $" ({"SyrTraitsSlowLearner".Translate()}: {totalSkillLevel}): {slowLearnerTotalOffset.ToStringSign()}{instance.ValueToString(slowLearnerTotalOffset, false)}");
        }
        else if (stat == StatDefOf.ResearchSpeed && pawnTraits.HasTrait(SyrTraitDefOf.SYR_CreativeThinker))
        {
            Trait creativeThinker = pawnTraits.GetTrait(SyrTraitDefOf.SYR_CreativeThinker);
            float creativeThinkerBonus = CreativeThinkerBonus(pawn);

            stringBuilder.Append("    " + creativeThinker.LabelCap);
            stringBuilder.AppendLine($" ({SkillDefOf.Artistic.LabelCap}: {pawn.skills.GetSkill(SkillDefOf.Artistic).Level}): {creativeThinkerBonus.ToStringSign()}{instance.ValueToString(creativeThinkerBonus, false)}");
        }
    }

    // removes the colon and everything after it (on the same line), and returns where the colon was
    private static int RemoveSlowLearnerStatDisplay(StringBuilder stringBuilder, Trait slowLearnerTrait)
    {
        string stringBuilderString = stringBuilder.ToString();
        string slowLearnerLabel = slowLearnerTrait.LabelCap;
        int slowLearnerIndex = stringBuilderString.IndexOf(slowLearnerLabel);
        
        if (slowLearnerIndex < 0)
            throw new ArgumentException($"Could not find {slowLearnerLabel} in {nameof(stringBuilder)}");

        int statDisplayStart = slowLearnerIndex + slowLearnerLabel.Length;
        int statDisplayEnd = stringBuilderString.IndexOf(Environment.NewLine, statDisplayStart);
        stringBuilder.Remove(statDisplayStart, statDisplayEnd - statDisplayStart);

        return statDisplayStart;
    }
}