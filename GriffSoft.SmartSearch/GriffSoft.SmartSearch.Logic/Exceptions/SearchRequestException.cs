using System;

namespace GriffSoft.SmartSearch.Logic.Exceptions;
public class SearchRequestException : Exception
{
    public SearchRequestException(string message) : base(message)
    {
    }
}
