using System.Text;

namespace HackAssembler
{
    public class Command
    {
        private readonly ICodeFinder codeFinder;

        public string Dest { get; set;}
        public string Comp { get; set;}
        public string Jump { get; set;}

        public Command(ICodeFinder codeFinder)
        {
            this.codeFinder = codeFinder;
        }

        public string ToBinaryString()
        {
            var builder = new StringBuilder();
            builder.Append("111");

            builder.Append(codeFinder.getCode("comp", this.Comp));

            if (this.Dest != null)
            {
                builder.Append(codeFinder.getCode("dest", this.Dest));
            }
            else
            {
                builder.Append("000");
            }

            if (this.Jump != null)
            {
                builder.Append(codeFinder.getCode("jump", this.Jump));
            }
            else
            {
                builder.Append("000");
            }

            return builder.ToString();
        }
    }
}