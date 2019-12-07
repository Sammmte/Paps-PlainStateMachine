using System;

namespace Paps.FSM
{
    public class StateIdNotAddedException : Exception
    {
        public StateIdNotAddedException(string stateIdString) : base("No state with id " + stateIdString + " was added to state machine")
        {

        }

        public StateIdNotAddedException()
        {

        }
    }
}
