namespace Assembler
{
    public class Parser
    {
        /// <summary>
        /// Удаляет все комментарии и пустые строки из программы. Удаляет все пробелы из команд.
        /// </summary>
        /// <param name="asmLines">Строки ассемблерного кода</param>
        /// <returns>Только значащие строки строки ассемблерного кода без комментариев и лишних пробелов</returns>
        public string[] RemoveWhitespacesAndComments(string[] asmLines)
        {
            var result = new List<string>();

            foreach (var line in asmLines)
            {
                if (line == null)
                    continue;
                
                var commentIndex = line.IndexOf("//");
                var noComment = commentIndex >= 0 ? line.Substring(0, commentIndex) : line;

                var trimmed = noComment.Trim();

                while (trimmed.Contains(" "))
                    trimmed = trimmed.Replace(" ", "");
                
                if (trimmed == "")
                    continue;
                
                result.Add(trimmed);
            }

            return result.ToArray();
        }
    }
}
