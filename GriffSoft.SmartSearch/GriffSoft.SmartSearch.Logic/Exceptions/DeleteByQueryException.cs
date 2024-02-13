using System;

namespace GriffSoft.SmartSearch.Logic.Exceptions;
public class DeleteByQueryException : Exception
{
    public DeleteByQueryException(string message) : base(message)
    {
    }
}
