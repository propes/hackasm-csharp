namespace HackAssembler
{
    public class CommandParser : ICommandParser
    {
        private readonly ICodeFinder codeFinder;

        public CommandParser(ICodeFinder codeFinder)
        {
            this.codeFinder = codeFinder;
        }

        public Command Parse(string line)
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
    }
}