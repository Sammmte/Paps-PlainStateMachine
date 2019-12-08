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

            fsm.AddTransition(1, 0, 1);

            fsm.AddGuardConditionTo(1, 0, 1, TestPredicate);

            Assert.IsTrue(fsm.ContainsGuardConditionOn(1, 0, 1, TestPredicate));

            fsm.RemoveGuardConditionFrom(1, 0, 1, TestPredicate);

            Assert.IsFalse(fsm.ContainsGuardConditionOn(1, 0, 1, TestPredicate));

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

            fsm.AddTransition(1, 0, 1);

            fsm.AddGuardConditionTo(1, 0, 1, TestPredicate);

            Assert.IsTrue(fsm.ContainsGuardConditionOn(1, 0, 1, TestPredicate));

            fsm.RemoveGuardConditionFrom(1, 0, 1, TestPredicate);

            Assert.IsFalse(fsm.ContainsGuardConditionOn(1, 0, 1, TestPredicate));

            bool TestPredicate(int stateFrom, int trigger, int stateTo)
            {
                return true;
            }
        }
    }
}