using System;
using System.Collections.Generic;

namespace Assembler
{
    public class SymbolAnalyzer
    {
		public Dictionary<string, int> CreateSymbolsTable(string[] instructionsWithLabels,
 out string[] instructionsWithoutLabels)
        {
            var tableSymbol = new Dictionary<string, int>()
            {
                {"R0", 0}, {"R1", 1}, {"R2", 2}, {"R3", 3}, {"R4", 4},
                {"R5", 5}, {"R6", 6}, {"R7", 7}, {"R8", 8}, {"R9", 9},
                {"R10", 10}, {"R11", 11}, {"R12", 12}, {"R13", 13},
                {"R14", 14}, {"R15", 15}, {"SCREEN", 0x4000},
                {"KBD", 0x6000}, {"SP", 0}, {"LCL", 1},
                {"ARG", 2}, {"THIS", 3}, {"THAT", 4}
            };

            var result = DeleteLabeles(tableSymbol, instructionsWithLabels);
            instructionsWithoutLabels = result.ToArray();
            return tableSymbol;
        }

        private List<string> DeleteLabeles(Dictionary<string, int> tableSymbol, string[] instructionsWithLabels)
        {
            var result = new List<string>();
            var lineIndex = 0;
            foreach (var line in instructionsWithLabels)
            {
                if (line.StartsWith("(") && line.EndsWith(")"))
                {
                    var label = line.Substring(1, line.Length - 2);
                    if (!tableSymbol.ContainsKey(label))
                        tableSymbol.Add(label, lineIndex);
                }
                else
                {
                    result.Add(line);
                    lineIndex++;
                }
            }

            return result;
        }
    }
}
