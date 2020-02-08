using System;

namespace Paps.StateMachines
{
    public class ProtectedStateException : Exception
    {
        public ProtectedStateException()
        {
        }

        public ProtectedStateException(string message) : base(message)
        {
        }
    }
}