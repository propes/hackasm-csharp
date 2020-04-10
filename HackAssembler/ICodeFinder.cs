using System.Collections.Generic;

namespace HackAssembler
{
    public interface ICodeFinder
    {
        string getCode(string tableName, string codeName);
    }
}