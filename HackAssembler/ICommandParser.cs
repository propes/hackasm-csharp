namespace HackAssembler
{
    public interface ICommandParser
    {
        Command Parse(string line);
    }
}