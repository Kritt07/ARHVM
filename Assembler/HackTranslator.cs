using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection.Emit;
using NUnit.Framework.Interfaces;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace Assembler
{

    public class HackTranslator
    {
        public int currentMaxVariableAddress = 16;
        /// <summary>
        /// Транслирует инструкции ассемблерного кода (без меток) в бинарное представление.
        /// </summary>
        /// <param name="instructions">Ассемблерный код без меток</param>
        /// <param name="symbolTable">Таблица символов</param>
        /// <returns>Строки инструкций в бинарном формате</returns>
        /// <exception cref="FormatException">Ошибка трансляции</exception>
        public string[] TranslateAsmToHack(string[] instructions, Dictionary<string, int> symbolTable)
        {
            var result = new List<string>();
            foreach (var line in instructions)
            {
                if (line[0] == '@')
                    result.Add(AInstructionToCode(line, symbolTable));
                else
                    result.Add(CInstructionToCode(line));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Транслирует одну A-инструкцию ассемблерного кода в бинарное представление
        /// </summary>
        /// <param name="aInstruction">Ассемблерная A-инструкция, например, @42 или @SCREEN</param>
        /// <param name="symbolTable">Таблица символов</param>
        /// <returns>Строка, содержащее нули и единицы — бинарное представление ассемблерной инструкции, например, "0000000000000101"</returns>
        public string AInstructionToCode(string aInstruction, Dictionary<string, int> symbolTable)
        {
            var address = aInstruction.Substring(1);

            if (symbolTable.ContainsKey(address))
                return Convert.ToString(symbolTable[address], 2).PadLeft(16, '0');

            if (int.TryParse(address, out var num))
                return Convert.ToString(num, 2).PadLeft(16, '0');
            else
            {
                var maxVariablesValue = currentMaxVariableAddress;
                symbolTable.Add(address, maxVariablesValue);
                currentMaxVariableAddress++;
                return Convert.ToString(maxVariablesValue, 2).PadLeft(16, '0');
            }
        }

        /// <summary>
        /// Транслирует одну C-инструкцию ассемблерного кода в бинарное представление
        /// </summary>
        /// <param name="cInstruction">Ассемблерная C-инструкция, например, A=D+M</param>
        /// <returns>Строка, содержащее нули и единицы — бинарное представление ассемблерной инструкции, например, "1111000010100000"</returns>
        public string CInstructionToCode(string cInstruction)
        {
            var result = new int[16] {1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

            var destCompJumpArray = ParseInstruction(cInstruction);
            var dest = destCompJumpArray[0];
            var comp = destCompJumpArray[1];
            var jump = destCompJumpArray[2];

            ParseDest(dest, result);
            ParseCamp(comp, result);
            ParseJump(jump, result);

            return String.Join("", result);
        }

        public static string[] ParseInstruction(string input)
        {
            string[] result = new string[3] { "", "", "" };
            
            if (string.IsNullOrEmpty(input))
                return result;

            // Разделяем по =
            string[] equalParts = input.Split('=', 2);
            
            if (equalParts.Length == 2)
            {
                result[0] = equalParts[0]; // dest
                input = equalParts[1];     // comp;jump или comp
            }

            // Разделяем оставшуюся часть по ;
            string[] semicolonParts = input.Split(';', 2);
            
            if (semicolonParts.Length == 2)
            {
                result[1] = semicolonParts[0]; // comp
                result[2] = semicolonParts[1]; // jump
            }
            else
            {
                result[1] = semicolonParts[0]; // comp
                result[2] = "";               // jump пустой
            }

            return result;
        }

        public static void ParseCamp(string comp, int[] result)
        {
            var compToBinary = new Dictionary<string, int[]>()
            {
                {"0", new[] {1, 0, 1, 0, 1, 0}},
                {"1", new[] {1, 1, 1, 1, 1, 1}},
                {"-1", new[] {1, 1, 1, 0, 1, 0}},
                {"D", new[] {0, 0, 1, 1, 0, 0}},
                {"A", new[] {1, 1, 0, 0, 0, 0}},
                {"!D", new[] {0, 0, 1, 1, 0, 1}},
                {"!A", new[] {1, 1, 0, 0, 0, 1}},
                {"-D", new[] {0, 0, 1, 1, 1, 1}},
                {"-A", new[] {1, 1, 0, 0, 1, 1}},
                {"D+1", new[] {0, 1, 1, 1, 1, 1}},
                {"A+1", new[] {1, 1, 0, 1, 1, 1}},
                {"D-1", new[] {0, 0, 1, 1, 1, 0}},
                {"A-1", new[] {1, 1, 0, 0, 1, 0}},
                {"D+A", new[] {0, 0, 0, 0, 1, 0}},
                {"D-A", new[] {0, 1, 0, 0, 1, 1}},
                {"A-D", new[] {0, 0, 0, 1, 1, 1}},
                {"D&A", new[] {0, 0, 0, 0, 0, 0}},
                {"D|A", new[] {0, 1, 0, 1, 0, 1}}
            };

            if (comp.Contains('M'))
            {
                result[3] = 1; 
                comp = comp.Replace('M', 'A');
            }


            foreach (var line in compToBinary)
            {
                if (line.Key == comp)
                    for (var i = 0; i < 6; i++)
                        result[i + 4] = line.Value[i];
            }
        }

        public static void ParseDest(string dest, int[] result)
        {
            
            var destToBinory = new Dictionary<string, int[]>()
            {
                {"", new[] {0, 0, 0}},
                {"M", new[] {0, 0, 1}},
                {"D", new[] {0, 1, 0}},
                {"MD", new[] {0, 1, 1}},
                {"A", new[] {1, 0, 0}},
                {"AM", new[] {1, 0, 1}},
                {"AD", new[] {1, 1, 0}},
                {"AMD", new[] {1, 1, 1}}
            };

            foreach (var line in destToBinory)
                if (line.Key == dest)
                    for (var i = 0; i < 3; i++)
                        result[i + 10] = line.Value[i];
        }

        public static void ParseJump(string jump, int[] result)
        {

            var jumpToBinory = new Dictionary<string, int[]>()
            {
                {"", new[] {0, 0, 0}},
                {"JGT", new[] {0, 0, 1}},
                {"JEQ", new[] {0, 1, 0}},
                {"JGE", new[] {0, 1, 1}},
                {"JLT", new[] {1, 0, 0}},
                {"JNE", new[] {1, 0, 1}},
                {"JLE", new[] {1, 1, 0}},
                {"JMP", new[] {1, 1, 1}}
            };

            foreach (var line in jumpToBinory)
                if (line.Key == jump)
                    for (var i = 0; i < 3; i++)
                        result[i + 13] = line.Value[i];
        }
    }
}
