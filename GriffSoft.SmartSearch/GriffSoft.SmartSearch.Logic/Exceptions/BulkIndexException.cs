using System;

namespace GriffSoft.SmartSearch.Logic.Exceptions;
public class BulkIndexException : Exception
{
    public BulkIndexException(string message) : base(message)
    {
    }
}
