using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HackAssembler
{
    public class HackAssembler
    {
        private Dictionary<string, int> symbols = new Dictionary<string, int>()
        {
            { "R0", 0 },
            { "R1", 1 },
            { "R2", 2 },
            { "R3", 3 },
            { "R4", 4 },
            { "R5", 5 },
            { "R6", 6 },
            { "R7", 7 },
            { "R8", 8 },
            { "R9", 9 },
            { "R10", 10 },
            { "R11", 11 },
            { "R12", 12 },
            { "R13", 13 },
            { "R14", 14 },
            { "R15", 15 },
            { "SCREEN", 16384 },
            { "KBD", 24576 },
            { "SP", 0 },
            { "LCL", 1 },
            { "ARG", 2 },
            { "THIS", 3 },
            { "THAT", 4 }
        };

        private readonly ICodeFinder codeFinder;

        public HackAssembler(ICodeFinder codeFinder) {
            this.codeFinder = codeFinder;
        }

        public void ProcessFile(string path)
        {
            var lines = File.ReadAllLines(path);

            var cleanedLines = RemoveCommentsAndWhitespace(lines);

            GetLabels(cleanedLines, symbols);

            GetVariables(cleanedLines, symbols);

            var linesNoSymbols = ReplaceSymbolsWithValues(cleanedLines, symbols);

            var machineLines = TranslateAssemblyToMachineCode(linesNoSymbols);

            File.WriteAllLines(
                Path.ChangeExtension(path, ".hack"),
                machineLines);
        }

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

        public void GetLabels(string[] lines, Dictionary<string, int> symbols)
        {
            int lineCounter = 0;
            foreach (var line in lines)
            {
                if (line[0] == '(' && line[line.Length - 1] == ')')
                {
                    var label = line.Substring(1, line.Length - 2);
                    symbols.Add(label, lineCounter);
                    continue;
                }

                lineCounter++;
            }
        }

        public void GetVariables(string[] lines, Dictionary<string, int> symbols)
        {
            var memAddress = 16;
            foreach (var line in lines)
            {
                if (line[0] == '@')
                {
                    var key = line.Substring(1, line.Length - 1);
                    if (!int.TryParse(key, out int result))
                    {
                        if (!symbols.ContainsKey(key))
                        {
                            symbols.Add(key, memAddress);
                            memAddress++;
                        }
                    }
                }
            }
        }

        public string[] ReplaceSymbolsWithValues(string[] lines, Dictionary<string, int> symbols)
        {
            var linesNoSymbols = new List<string>();

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line[0] == '@')
                {
                    var key = line.Substring(1, line.Length - 1);
                    if (symbols.ContainsKey(key))
                    {
                        linesNoSymbols.Add("@" + symbols[key]);
                    }
                    else
                    {
                        linesNoSymbols.Add(line);
                    }
                }
                else if (line[0] == '(' && line[line.Length - 1] == ')')
                {
                }
                else
                {
                    linesNoSymbols.Add(line);
                }
            }

            return linesNoSymbols.ToArray();
        }

        private string[] TranslateAssemblyToMachineCode(string[] lines)
        {
            var translatedLines = new List<string>();

            foreach (var line in lines)
            {
                translatedLines.Add(TranslateAssemblyToMachineCode(line));
            }

            return translatedLines.ToArray();
        }

        private string TranslateAssemblyToMachineCode(string line)
        {
            if (line[0] == '@')
            {
                return TranslateAddressInstruction(line);
            }

            return TranslateCommandInstruction(line);
        }

        private string TranslateAddressInstruction(string line)
        {
            var addressString = line.Substring(1, line.Length - 1);
            var addressValue = Convert.ToInt32(addressString);
            var binaryString = Convert.ToString(addressValue, 2);

            return binaryString.PadLeft(16, '0');
        }

        private string TranslateCommandInstruction(string line)
        {
            var commandLine = ParseCommandLine(line);

            return BuildBinaryString(commandLine);
        }

        private string BuildBinaryString(CommandLine command)
        {
            var builder = new StringBuilder();
            builder.Append("111");

            builder.Append(codeFinder.getCode("comp", command.Comp));

            if (command.Dest != null)
            {
                builder.Append(codeFinder.getCode("dest", command.Dest));
            }
            else
            {
                builder.Append("000");
            }

            if (command.Jump != null)
            {
                builder.Append(codeFinder.getCode("jump", command.Jump));
            }
            else
            {
                builder.Append("000");
            }

            return builder.ToString();
        }

        private CommandLine ParseCommandLine(string line)
        {
            var command = new CommandLine();
            string remainder = null;

            var args = line.Split('=');
            if (args.Length > 1)
            {
                command.Dest = args[0];
                remainder = args[1];
            }
            else
            {
                remainder = args[0];
            }

            var args2 = remainder.Split(';');
            command.Comp = args2[0];
            if (args2.Length > 1)
            {
                command.Jump = args2[1];
            }

            return command;
        }
    }

    public class CommandLine
    {
        public string Dest { get; set;}
        public string Comp { get; set;}
        public string Jump { get; set;}
    }
}