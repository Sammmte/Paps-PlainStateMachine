using System;

namespace Paps.StateMachines
{
    public class TransitionNotAddedException : Exception
    {
        public TransitionNotAddedException(string stateFrom, string trigger, string stateTo) 
            : base("Transition with values { StateFrom: " + stateFrom + " Trigger: " + trigger + " StateTo: " + stateTo + " } was not added to state machine")
        {

        }

        public TransitionNotAddedException()
        {

        }
    }
}
