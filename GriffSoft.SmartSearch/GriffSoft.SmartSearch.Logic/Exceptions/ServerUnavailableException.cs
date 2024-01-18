using System;

namespace GriffSoft.SmartSearch.Logic.Exceptions;
public class ServerUnavailableException : Exception
{
    public ServerUnavailableException(string message) : base(message)
    {
    }
}
