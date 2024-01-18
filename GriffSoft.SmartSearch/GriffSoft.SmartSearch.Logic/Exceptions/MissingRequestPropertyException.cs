using System;

namespace GriffSoft.SmartSearch.Logic.Exceptions;
internal class MissingRequestPropertyException : Exception
{
    public MissingRequestPropertyException(string message) : base(message)
    {
    }
}
