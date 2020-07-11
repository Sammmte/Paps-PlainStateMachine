using Paps.StateMachines;

namespace Tests.WithStructs
{
    public class PlainStateMachineWithStructsShould : PlainStateMachineShould<int, int>
    {
        private int _stateInt;
        private int _triggerInt;

        protected override int NewStateId()
        {
            return _stateInt++;
        }

        protected override PlainStateMachine<int, int> NewStateMachine()
        {
            return new PlainStateMachine<int, int>();
        }

        protected override Transition<int, int> NewTransition()
        {
            return new Transition<int, int>(NewStateId(), NewTrigger(), NewStateId());
        }

        protected override Transition<int, int> NewTransition(int stateFrom, int trigger, int stateTo)
        {
            return new Transition<int, int>(stateFrom, trigger, stateTo);
        }

        protected override int NewTrigger()
        {
            return _triggerInt++;
        }
    }
}
