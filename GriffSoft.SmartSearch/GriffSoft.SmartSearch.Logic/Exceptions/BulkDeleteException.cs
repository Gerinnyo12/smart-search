using System;

namespace GriffSoft.SmartSearch.Logic.Exceptions;
public class BulkDeleteException : Exception
{
    public BulkDeleteException(string message) : base(message)
    {
    }
}
