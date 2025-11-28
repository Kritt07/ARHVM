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
        /// <summary>
        /// Транслирует инструкции ассемблерного кода (без меток) в бинарное представление.
        /// </summary>
        /// <param name="instructions">Ассемблерный код без меток</param>
        /// <param name="symbolTable">Таблица символов</param>
        /// <returns>Строки инструкций в бинарном формате</returns>
        /// <exception cref="FormatException">Ошибка трансляции</exception>
        public string[] TranslateAsmToHack(string[] instructions, Dictionary<string, int> symbolTable)
        {


            throw new NotImplementedException();

        }

        /// <summary>
        /// Транслирует одну A-инструкцию ассемблерного кода в бинарное представление
        /// </summary>
        /// <param name="aInstruction">Ассемблерная A-инструкция, например, @42 или @SCREEN</param>
        /// <param name="symbolTable">Таблица символов</param>
        /// <returns>Строка, содержащее нули и единицы — бинарное представление ассемблерной инструкции, например, "0000000000000101"</returns>
        public string AInstructionToCode(string aInstruction, Dictionary<string, int> symbolTable)
        {
            var value = aInstruction.Substring(1, aInstruction.Length - 1);

            if (int.TryParse(value, out var num))
                return Convert.ToString(num, 2).PadLeft(16, '0');
            else
            {
                if (symbolTable.ContainsKey(value))
                    return Convert.ToString(symbolTable[value], 2).PadLeft(16, '0');
                else
                {
                    var lastValue = symbolTable.Last().Value;
                    symbolTable.Add(value, lastValue + 1);
                    return Convert.ToString(lastValue + 1, 2).PadLeft(16, '0');
                }
            }
        }

        /// <summary>
        /// Транслирует одну C-инструкцию ассемблерного кода в бинарное представление
        /// </summary>
        /// <param name="cInstruction">Ассемблерная C-инструкция, например, A=D+M</param>
        /// <returns>Строка, содержащее нули и единицы — бинарное представление ассемблерной инструкции, например, "1111000010100000"</returns>
        public string CInstructionToCode(string cInstruction)
        {
            var result = new int[15] {1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

            var destCompJumpArray = cInstruction.Split('=', ';');
            var dest = destCompJumpArray[0];
            var comp = destCompJumpArray[1];
            var jump = destCompJumpArray.Length > 2 ? destCompJumpArray[2] : "";

            ParseDest(dest, result);
            ParseCamp(comp, result);
            ParseJump(jump, result);

            return String.Join();
        }

        public static void ParseCamp(string comp, int[] result)
        {
            var aluInstruction = new int[6];

            var compToBinary = new Dictionary<string, int[]>()
            {
                {"0", new[] {0, 1, 0, 1, 0, 1}},
                {"1", new[] {1, 1, 1, 1, 1, 1}},
                {"-1", new[] {0, 1, 0, 1, 1, 1}},
                {"D", new[] {0, 0, 1, 1, 0, 0}},
                {"A", new[] {0, 0, 0, 0, 1, 1}},
                {"!D", new[] {1, 0, 1, 1, 0, 0,}},
                {"!A", new[] {1, 0, 0, 0, 1, 1}},
                {"-D", new[] {1, 1, 1, 1, 0, 0}},
                {"-A", new[] {1, 1, 0, 0, 1, 1}},
                {"D+1", new[] {1, 1, 1, 1, 1, 0}},
                {"A+1", new[] {1, 1, 1, 0, 0, 1}},
                {"D-1", new[] {0, 1, 1, 1, 0, 0}},
                {"A-1", new[] {0, 1, 0, 0, 1, 1}},
                {"D+A", new[] {0, 1, 0, 0, 0, 0}},
                {"D-A", new[] {1, 1, 0, 0, 1, 0}},
                {"A-D", new[] {1, 1, 1, 0, 0, 0}},
                {"D&A", new[] {0, 0, 0, 0, 0, 0}},
                {"D|A", new[] {1, 0, 1, 0, 1, 0}}
            };

            if (comp.Contains('M'))
            {
                result[12] = 1; 
                comp.Replace('M', 'A');
            }

            foreach (var line in compToBinary)
                if (line.Key == comp)
                    aluInstruction = line.Value;

            for (var i = 0; i < 6; i++)
                result[i + 6] = aluInstruction[0];
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
                        result[i + 3] = line.Value[i];
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
                        result[i] = line.Value[i];
        }
    }
}
