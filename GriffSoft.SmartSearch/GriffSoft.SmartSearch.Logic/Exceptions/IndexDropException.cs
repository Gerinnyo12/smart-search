using System;

namespace GriffSoft.SmartSearch.Logic.Exceptions;
public class IndexDropException : Exception
{
    public IndexDropException(string message) : base(message)
    {
    }
}
