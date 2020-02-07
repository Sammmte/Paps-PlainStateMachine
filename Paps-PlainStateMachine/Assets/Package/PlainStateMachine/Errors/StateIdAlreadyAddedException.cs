using System;

namespace Paps.StateMachines
{
    public class StateIdAlreadyAddedException : Exception
    {
        public StateIdAlreadyAddedException()
        {

        }

        public StateIdAlreadyAddedException(string stateId) : base("State id " + stateId + " is already added to state machine")
        {

        }
    }
}
