using System.Collections.Generic;
using System.IO;
using Xunit;

namespace HackAssembler.Tests
{
    public class HackAssemblerIntegrationTests
    {
        private const string Directory = "TestFiles";

        [Theory()]
        [InlineData("Add.asm", "Add.hack", "Add.cmp")]
        [InlineData("Max.asm", "Max.hack", "Max.cmp")]
        [InlineData("Rect.asm", "Rect.hack", "Rect.cmp")]
        [InlineData("Pong.asm", "Pong.hack", "Pong.cmp")]
        public void ProcessFile_WorksCorrectly(string testFileName,
            string outputFileName,
            string compareFileName)
        {
            testFileName = Path.Join(Directory, testFileName);
            outputFileName = Path.Join(Directory, outputFileName);
            compareFileName = Path.Join(Directory, compareFileName);

            new HackAssembler().ProcessFile(testFileName);

            var outputData = File.ReadAllLines(outputFileName);;
            var compareData = File.ReadAllLines(compareFileName);

            for (int i = 0; i < outputData.Length; i++)
            {
                Assert.Equal(compareData[i], outputData[i]);
            }
        }

        [Fact]
        public void RemoveCommentsAndWhitespace_WorksCorrectly() {
            var lines = new string[]
            {
                " foo ",
                "",
                "// some comment",
                "bar // another comment",
                ""
            };

            var result = new HackAssembler().RemoveCommentsAndWhitespace(lines);

            Assert.Equal(2, result.Length);
            Assert.Equal("foo", result[0]);
            Assert.Equal("bar", result[1]);
        }

        [Fact]
        public void GetLabels_AddsSymbolsToDictionary()
        {
            var lines = new string []
            {
                "@R1",
                "(foo)",
                "@bar",
                "0;JMP",
                "(bar)",
                "@R11"
            };
            var symbols = new Dictionary<string, int>();

            new HackAssembler().GetLabels(lines, symbols);

            Assert.Equal(1, symbols["foo"]);
            Assert.Equal(3, symbols["bar"]);
        }

        [Fact]
        public void GetVariables_AddSymbolsToDictionary()
        {
            var lines = new string []
            {
                "@baz",
                "M=1",
                "@2",
                "(foo)",
                "@bar",
                "D=M",
                "(bar)",
                "@bah",
                "M=0"
            };
            var symbols = new Dictionary<string, int>
            {
                { "foo", 3 },
                { "bar", 5 }
            };

            new HackAssembler().GetVariables(lines, symbols);

            Assert.Equal(16, symbols["baz"]);
            Assert.Equal(17, symbols["bah"]);
            Assert.Equal(5, symbols["bar"]);
        }

        [Fact]
        public void ReplaceSymbolsWithValues_ReturnsCorrectLines()
        {
            var lines = new string []
            {
                "@baz",
                "M=1",
                "(foo)",
                "@bar",
                "D=M",
                "(bar)",
                "@bah",
                "M=0"
            };
            var symbols = new Dictionary<string, int>
            {
                { "foo", 3 },
                { "bar", 5 },
                { "baz", 16 },
                { "bah", 17 }
            };

            var linesNoSymbols = new HackAssembler().ReplaceSymbolsWithValues(lines, symbols);

            Assert.Equal("@16", linesNoSymbols[0]);
            Assert.Equal("M=1", linesNoSymbols[1]);
            Assert.Equal("@5", linesNoSymbols[2]);
            Assert.Equal("D=M", linesNoSymbols[3]);
            Assert.Equal("@17", linesNoSymbols[4]);
            Assert.Equal("M=0", linesNoSymbols[5]);
        }
    }
}
