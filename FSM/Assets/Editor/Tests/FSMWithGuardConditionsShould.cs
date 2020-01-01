using NUnit.Framework;
using Paps.FSM;
using Paps.FSM.Extensions;

namespace Tests
{
    public class FSMWithGuardConditionsShould
    {
        [Test]
        public void AddRemoveAndTellIfContainsPredicateGuardCondition()
        {
            var fsm = new FSM<int, int>();

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

        [Test]
        public void AddRemoveAndTellIfContainsPredicateWithParametersGuardCondition()
        {
            var fsm = new FSM<int, int>();

            fsm.AddEmpty(1);

            var transition = new Transition<int, int>(1, 0, 1);

            fsm.AddTransition(transition);

            fsm.AddGuardConditionTo(transition, TestPredicate);

            Assert.IsTrue(fsm.ContainsGuardConditionOn(transition, TestPredicate));

            fsm.RemoveGuardConditionFrom(transition, TestPredicate);

            Assert.IsFalse(fsm.ContainsGuardConditionOn(transition, TestPredicate));

            bool TestPredicate(int stateFrom, int trigger, int stateTo)
            {
                return true;
            }
        }
    }
}