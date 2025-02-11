using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace SyrTraits
{
    internal static class ComplementBranchOpcodeExtension
    {
        private readonly struct ComplementOpCodeInfo(string name, bool standalone = false, string requiredNewPart = null, string requiredRemovePart = null)
        {
            public readonly string name = name;
            public readonly bool standalone = standalone && requiredNewPart is null && requiredRemovePart is null;
            public readonly string requiredNewPart = requiredNewPart;
            public readonly string requiredRemovePart = requiredRemovePart;
        }

        private static readonly Dictionary<string, ComplementOpCodeInfo> _complementOpCodes = new()
        {
            ["Br"] = new("Nop", standalone: true),
            ["Brfalse"] = new("Brtrue"),
            ["Brtrue"] = new("Brfalse"),
            ["Beq"] = new("Bne", requiredNewPart: "Un"),
            ["Bne"] = new("Beq", requiredRemovePart: "Un"),
            ["Blt"] = new("Bge"),
            ["Bge"] = new("Blt"),
            ["Bgt"] = new("Ble"),
            ["Ble"] = new("Bgt"),
        };
        
        public static OpCode Complement(this OpCode opCode)
        {
            string[] splitOpCodeName = CaptalizeFirstSplit(opCode.Name, '.');
            string mainPart = splitOpCodeName[0];
            ComplementOpCodeInfo complement = _complementOpCodes.GetValueOrDefault(mainPart);

            if (complement.standalone)
                splitOpCodeName = [complement.name];
            else
                splitOpCodeName[0] = complement.name;

            if (complement.requiredNewPart is not null && !splitOpCodeName.Contains(complement.requiredNewPart))
            {
                string[] newSplitOpCodeName = new string[splitOpCodeName.Length + 1];
                newSplitOpCodeName[0] = splitOpCodeName[0];
                newSplitOpCodeName[1] = complement.requiredNewPart;

                for (int i = 1; i < splitOpCodeName.Length; i++)
                    newSplitOpCodeName[i + 1] = splitOpCodeName[i];

                splitOpCodeName = newSplitOpCodeName;
            }
            if (complement.requiredRemovePart is not null && splitOpCodeName.Contains(complement.requiredRemovePart))
            {
                string[] newSplitOpCodeName = new string[splitOpCodeName.Length - 1];

                int index;
                for (index = 0; splitOpCodeName[index] != complement.requiredRemovePart; index++)
                    newSplitOpCodeName[index] = splitOpCodeName[index];

                int indexOfBadPart = index;
                for (index = indexOfBadPart + 1; index < splitOpCodeName.Length; index++)
                    newSplitOpCodeName[index - 1] = splitOpCodeName[index];

                splitOpCodeName = newSplitOpCodeName;
            }

            string complementOpCodeName = string.Join("_", splitOpCodeName);
            FieldInfo complementOpCodeField = typeof(OpCodes).GetField(complementOpCodeName);

            if (complementOpCodeField is null)
                throw new ArgumentException($"Could not find complement of {opCode.Name}.", nameof(opCode));
            return (OpCode)complementOpCodeField.GetValue(null);
        }

        private static string[] CaptalizeFirstSplit(string str, char separator)
        {
            string[] split = str.Split(separator);
            for (int i = 0; i < split.Length; i++)
                split[i] = split[i].CapitalizeFirst();

            return split;
        }
    }
}
