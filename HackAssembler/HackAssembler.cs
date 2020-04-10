using System;
using System.Collections.Generic;
using System.IO;

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
        private readonly ITextCleaner textCleaner;

        public HackAssembler(
                ICodeFinder codeFinder,
                ITextCleaner textCleaner) {
            this.codeFinder = codeFinder;
            this.textCleaner = textCleaner;
        }

        public void ProcessFile(string path)
        {
            var lines = File.ReadAllLines(path);

            var cleanedLines = this.textCleaner.RemoveCommentsAndWhitespace(lines);

            GetLabels(cleanedLines, symbols);

            GetVariables(cleanedLines, symbols);

            var linesNoSymbols = ReplaceSymbolsWithValues(cleanedLines, symbols);

            var machineLines = TranslateAssemblyToMachineCode(linesNoSymbols);

            File.WriteAllLines(
                Path.ChangeExtension(path, ".hack"),
                machineLines);
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
                if (IsAddressLine(line))
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
                if (IsAddressLine(line))
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
            if (IsAddressLine(line))
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
            var command = ParseCommandLine(line);

            return command.ToBinaryString();
        }

        private Command ParseCommandLine(string line)
        {
            var command = new Command(this.codeFinder);
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
        
        private bool IsAddressLine(string line)
        {
            return line[0] == '@';
        }
    }
}