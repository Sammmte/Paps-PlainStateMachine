using Paps.StateMachines;
using System;

namespace Tests.WithClasses
{
    public class PlainStateMachineWithClassesShould : PlainStateMachineShould<string, string>
    {
        protected override string NewStateId()
        {
            return Guid.NewGuid().ToString();
        }

        protected override PlainStateMachine<string, string> NewStateMachine()
        {
            return new PlainStateMachine<string, string>();
        }

        protected override Transition<string, string> NewTransition()
        {
            return new Transition<string, string>(NewStateId(), NewTrigger(), NewStateId());
        }

        protected override Transition<string, string> NewTransition(string stateFrom, string trigger, string stateTo)
        {
            return new Transition<string, string>(stateFrom, trigger, stateTo);
        }

        protected override string NewTrigger()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
