using NUnit.Framework;
using Paps.StateMachines;
using Paps.StateMachines.Extensions;
using NSubstitute;

namespace Tests.WithStructs
{
    public class GuardedStateMachineShould
    {
        [Test]
        public void Add_Guard_Conditions()
        {
            var fsm = new PlainStateMachine<int, int>();

            fsm.AddEmpty(1);

            var transition = new Transition<int, int>(1, 0, 1);
            IGuardCondition guardCondition = Substitute.For<IGuardCondition>();

            fsm.AddTransition(transition);

            fsm.AddGuardConditionTo(transition, guardCondition);

            Assert.That(fsm.ContainsGuardConditionOn(transition, guardCondition), "Contains guard condition on transition");
        }

        [Test]
        public void Remove_Guard_Conditions()
        {
            var fsm = new PlainStateMachine<int, int>();

            fsm.AddEmpty(1);

            var transition = new Transition<int, int>(1, 0, 1);
            IGuardCondition guardCondition = Substitute.For<IGuardCondition>();

            fsm.AddTransition(transition);

            fsm.AddGuardConditionTo(transition, guardCondition);

            Assert.That(fsm.RemoveGuardConditionFrom(transition, guardCondition), "Guard condition was removed");
            Assert.That(fsm.ContainsGuardConditionOn(transition, guardCondition) == false, "Does not contains guard condition");
        }

        [Test]
        public void Return_Guard_Conditions_Of_A_Specific_Transition()
        {
            var fsm = new PlainStateMachine<int, int>();

            fsm.AddEmpty(1);
            fsm.AddEmpty(2);

            var transition1 = new Transition<int, int>(1, 0, 2);
            var transition2 = new Transition<int, int>(2, 0, 1);

            IGuardCondition guardCondition1 = Substitute.For<IGuardCondition>();
            IGuardCondition guardCondition2 = Substitute.For<IGuardCondition>();
            IGuardCondition guardCondition3 = Substitute.For<IGuardCondition>();

            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);

            fsm.AddGuardConditionTo(transition1, guardCondition1);
            fsm.AddGuardConditionTo(transition1, guardCondition2);

            fsm.AddGuardConditionTo(transition2, guardCondition3);

            var guardConditions = fsm.GetGuardConditionsOf(transition1);

            Assert.Contains(guardCondition1, guardConditions);
            Assert.Contains(guardCondition2, guardConditions);
            AssertExtensions.DoesNotContains(guardCondition3, guardConditions);
        }
    }
}