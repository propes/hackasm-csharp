using Xunit;

namespace HackAssembler.Tests
{
    public class TextCleanerTests
    {
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

            var result = new TextCleaner().RemoveCommentsAndWhitespace(lines);

            Assert.Equal(2, result.Length);
            Assert.Equal("foo", result[0]);
            Assert.Equal("bar", result[1]);
        }
    }
}
