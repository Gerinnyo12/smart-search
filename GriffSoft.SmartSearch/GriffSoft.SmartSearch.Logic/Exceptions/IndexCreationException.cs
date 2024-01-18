using System;

namespace GriffSoft.SmartSearch.Logic.Exceptions;
public class IndexCreationException : Exception
{
    public IndexCreationException(string message) : base(message)
    {
    }
}
