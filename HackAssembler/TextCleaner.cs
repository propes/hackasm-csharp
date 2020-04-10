using System.Collections.Generic;

namespace HackAssembler
{
    public class TextCleaner : ITextCleaner
    {
        public string[] RemoveCommentsAndWhitespace(string[] lines)
        {
            List<string> cleanedLines = new List<string>();

            foreach (var line in lines)
            {
                var cleaned = line.Replace(" ", "");
                var i = cleaned.IndexOf("//");
                if (i >= 0)
                {
                    cleaned = cleaned.Substring(0, i);
                }

                if (cleaned != string.Empty)
                {
                    cleanedLines.Add(cleaned);
                }
            }

            return cleanedLines.ToArray();
        }
    }
}