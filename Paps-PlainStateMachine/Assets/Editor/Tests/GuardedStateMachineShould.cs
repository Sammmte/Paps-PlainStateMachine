using NUnit.Framework;
using Paps.StateMachines;
using Paps.StateMachines.Extensions;

namespace Tests
{
    public class FSMWithGuardConditionsShould
    {
        [Test]
        public void Add_Remove_And_Tell_If_Contains_Predicate_Guard_Condition()
        {
            var fsm = new PlainStateMachine<int, int>();

            fsm.AddEmpty(1);

            var transition = new Transition<int, int>(1, 0, 1);

            fsm.AddTransition(transition);

            fsm.AddGuardConditionTo(transition, TestPredicate);

            Assert.IsTrue(fsm.ContainsGuardConditionOn(transition, TestPredicate));

            fsm.RemoveGuardConditionFrom(transition, TestPredicate);

            Assert.IsFalse(fsm.ContainsGuardConditionOn(transition, TestPredicate));

            bool TestPredicate()
            {
                return true;
            }
        }
    }
}