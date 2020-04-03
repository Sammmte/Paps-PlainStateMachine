using NSubstitute;
using NUnit.Framework;
using Paps.StateMachines.Extensions.BehaviouralStates;

namespace Tests
{
    public class BehaviouralStateShould
    {
        [Test]
        public void Do_Not_Add_Repeated_State_Behaviours_On_Construction()
        {
            var stateBehaviour = Substitute.For<IStateBehaviour>();

            var state = new BehaviouralState(stateBehaviour, stateBehaviour);

            Assert.That(state.BehaviourCount == 1, "Only one state behaviour has been added");
        }

        [Test]
        public void Keep_Iterating_When_A_Previous_Index_Is_Removed_On_Enter()
        {
            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();
            var stateBehaviour3 = Substitute.For<IStateBehaviour>();

            var state = new BehaviouralState(stateBehaviour1, stateBehaviour2, stateBehaviour3);

            stateBehaviour2.When(behaviour => behaviour.OnEnter())
                .Do(callbackInfo => state.RemoveBehaviour(stateBehaviour1));

            state.Enter();

            stateBehaviour1.Received(1).OnEnter();
            stateBehaviour2.Received(1).OnEnter();
            stateBehaviour3.Received(1).OnEnter();
        }

        [Test]
        public void Keep_Iterating_When_A_Posterior_Index_Is_Removed_On_Enter()
        {
            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();
            var stateBehaviour3 = Substitute.For<IStateBehaviour>();

            var state = new BehaviouralState(stateBehaviour1, stateBehaviour2, stateBehaviour3);

            stateBehaviour2.When(behaviour => behaviour.OnEnter())
                .Do(callbackInfo => state.RemoveBehaviour(stateBehaviour3));

            state.Enter();

            stateBehaviour1.Received(1).OnEnter();
            stateBehaviour2.Received(1).OnEnter();
            stateBehaviour3.DidNotReceive().OnEnter();
        }

        [Test]
        public void Keep_Iterating_When_A_Previous_Index_Is_Removed_On_Update()
        {
            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();
            var stateBehaviour3 = Substitute.For<IStateBehaviour>();

            var state = new BehaviouralState(stateBehaviour1, stateBehaviour2, stateBehaviour3);

            stateBehaviour2.When(behaviour => behaviour.OnUpdate())
                .Do(callbackInfo => state.RemoveBehaviour(stateBehaviour1));

            state.Update();

            stateBehaviour1.Received(1).OnUpdate();
            stateBehaviour2.Received(1).OnUpdate();
            stateBehaviour3.Received(1).OnUpdate();
        }

        [Test]
        public void Keep_Iterating_When_A_Posterior_Index_Is_Removed_On_Update()
        {
            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();
            var stateBehaviour3 = Substitute.For<IStateBehaviour>();

            var state = new BehaviouralState(stateBehaviour1, stateBehaviour2, stateBehaviour3);

            stateBehaviour2.When(behaviour => behaviour.OnUpdate())
                .Do(callbackInfo => state.RemoveBehaviour(stateBehaviour3));

            state.Update();

            stateBehaviour1.Received(1).OnUpdate();
            stateBehaviour2.Received(1).OnUpdate();
            stateBehaviour3.DidNotReceive().OnUpdate();
        }

        [Test]
        public void Keep_Iterating_When_A_Previous_Index_Is_Removed_On_Exit()
        {
            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();
            var stateBehaviour3 = Substitute.For<IStateBehaviour>();

            var state = new BehaviouralState(stateBehaviour1, stateBehaviour2, stateBehaviour3);

            stateBehaviour2.When(behaviour => behaviour.OnExit())
                .Do(callbackInfo => state.RemoveBehaviour(stateBehaviour1));

            state.Exit();

            stateBehaviour1.Received(1).OnExit();
            stateBehaviour2.Received(1).OnExit();
            stateBehaviour3.Received(1).OnExit();
        }

        [Test]
        public void Keep_Iterating_When_A_Posterior_Index_Is_Removed_On_Exit()
        {
            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();
            var stateBehaviour3 = Substitute.For<IStateBehaviour>();

            var state = new BehaviouralState(stateBehaviour1, stateBehaviour2, stateBehaviour3);

            stateBehaviour2.When(behaviour => behaviour.OnExit())
                .Do(callbackInfo => state.RemoveBehaviour(stateBehaviour3));

            state.Exit();

            stateBehaviour1.Received(1).OnExit();
            stateBehaviour2.Received(1).OnExit();
            stateBehaviour3.DidNotReceive().OnExit();
        }
    }
}