namespace HackAssembler
{
    public interface ITextCleaner
    {
        string[] RemoveCommentsAndWhitespace(string[] lines);
    }
}