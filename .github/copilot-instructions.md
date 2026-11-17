# GitHub Copilot Instructions for [SYR] Individuality (Continued) Mod

Welcome to the [SYR] Individuality (Continued) mod for RimWorld! This mod adds a variety of new traits, rebalances vanilla traits, and introduces new stats like sexuality, confidence, weight, and psychic sensitivity. Below are detailed instructions to guide you through the coding patterns and conventions, XML integration, Harmony patching, and general suggestions for using GitHub Copilot in this project.

## Mod Overview and Purpose

The [SYR] Individuality (Continued) mod is an update and continuation of the Syrchaliss mod. It enhances the character customization and personality aspects of RimWorld by introducing new traits and rebalancing existing ones. The mod also shifts the focus from simple stat bonuses to more complex and unique effects, including:

- New traits with C#-based effects
- Rebalancing vanilla traits to give minor downsides to positive traits and minor upsides to negative traits
- Removal of boring traits
- Introduction of new stats like sexuality, confidence, weight, and psychic sensitivity
- Small tweaks to the romance system to allow for more diverse interactions

## Key Features and Systems

### New Traits
- Several new profession-related traits are added.
- Most new traits use C# for unique effects rather than simple stat changes.

### Rebalanced Vanilla Traits
- Traits like Expert Shooter, Slothful/Lazy, HardWorker/Industrious, etc., have been rebalanced to provide more nuanced effects.

### New Stats
- **Sexuality**: Represents the sexual orientation of the pawn, affecting romance attempts.
- **Confidence**: Affects the likelihood of successful romance attempts.
- **Body Weight**: Randomly assigned to pawns, affecting certain interactions.
- **Psychic Sensitivity**: Represents the pawn's sensitivity to psychic phenomena.

### Romance System Enhancements
- Asexual pawns can now hook up with each other.
- Beauty is no longer a binary trait; pawns can have varying levels of attractiveness.
- Other small tweaks to accommodate the new stats and traits.

### Edit Mode
- Pawns can be edited in-game by enabling edit mode and using left-click, right-click, or the mouse wheel to change their stats and traits.

## Coding Patterns and Conventions

### C# Conventions
- **Namespaces**: All C# code is organized within the `SyrTraits` namespace.
- **Class Structure**: Each class has a clear purpose, with methods and properties organized logically.
- **Comments**: Extensive comments are provided to explain complex logic and methods.
- **Constants**: Common values are stored in constants for easy modification and reference.

### XML Conventions
- **Def Files**: Definitions are stored in XML files within the `Defs` directory.
- **Patch Files**: Harmony patches are defined in XML files within the `Patches` directory.
- **Attribute Usage**: Proper use of XML attributes to define traits, stats, and thoughts.

## XML Integration

The [SYR] Individuality (Continued) mod uses XML extensively for defining traits, stats, and thoughts. Here are some key XML files and their purposes:

- **StatDefs.xml**: Defines new stats like sexuality, confidence, weight, and psychic sensitivity.
- **ThoughtDefs.xml**: Defines new thoughts related to the new stats and traits.
- **TraitDefs.xml**: Defines new traits and rebalances existing ones.
- **Patches_Traits.xml**: Contains Harmony patches to modify vanilla behavior.
- **TraitValue.xml**: Defines the values and effects of various traits.

### Example XML Entry

xml
<StatDef Name="Sexuality" label="Sexuality" description="Represents the sexual orientation of the pawn" category="Personality" hidden="False" defaultValue="0" minValue="0" maxValue="100" formatString="0.0" formatterClass="NumericFormatter" defName="Sexuality" />


## Harmony Patching

Harmony patching is used to modify vanilla behavior without altering the original RimWorld code. Key Harmony patches in the mod include:

- **CharacterCardUtility_DrawCharacterCard**: Adds a button to the character card for viewing individuality stats.
- **TraitValue**: Modifies the value of certain traits based on new stats and logic.

### Example Harmony Patch

csharp
[HarmonyPatch(typeof(CharacterCardUtility), "DrawCharacterCard")]
public static class CharacterCardUtility_DrawCharacterCard
{
    public static void Postfix(Rect rect, Pawn pawn)
    {
        if (pawn.HasComp<CompIndividuality>())
        {
            // Draw additional stats and traits
        }
    }
}


## Suggestions for Copilot

To make the most out of GitHub Copilot, consider the following suggestions:

1. **Frequent Use**: Utilize Copilot frequently to generate code snippets and suggestions for both C# and XML files.
2. **Custom Prompts**: Use custom prompts to guide Copilot in generating specific code or XML structures, such as defining new traits or creating Harmony patches.
3. **Review and Refine**: Always review and refine the generated code to ensure it aligns with the mod's coding patterns and conventions.
4. **Documentation**: Keep detailed documentation of the mod's architecture and coding patterns to help Copilot generate more relevant suggestions.
5. **Feedback**: Provide feedback to Copilot on its suggestions to improve the accuracy and relevance of future suggestions.

By following these guidelines, you can effectively use GitHub Copilot to enhance the development of the [SYR] Individuality (Continued) mod for RimWorld. Happy coding!

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

