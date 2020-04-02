using NSubstitute;
using NUnit.Framework;
using Paps.StateMachines.Extensions.BehaviouralStates;
using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;

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
        public void Keep_Iterating_When_A_Previous_Index_Is_Removed()
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
        public void Keep_Iterating_When_A_Posterior_Index_Is_Removed()
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
        public void Throw_An_Exception_When_User_Tries_To_Enter_While_Iterating()
        {
            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();
            var stateBehaviour3 = Substitute.For<IStateBehaviour>();

            var state = new BehaviouralState(stateBehaviour1, stateBehaviour2, stateBehaviour3);

            stateBehaviour2.When(behaviour => behaviour.OnEnter())
                .Do(callbackInfo => state.Enter());

            Assert.Throws<InvalidOperationException>(() => state.Enter());
            Assert.Throws<InvalidOperationException>(() => state.Update());
            Assert.Throws<InvalidOperationException>(() => state.Exit());

            stateBehaviour1 = Substitute.For<IStateBehaviour>();
            stateBehaviour2 = Substitute.For<IStateBehaviour>();
            stateBehaviour3 = Substitute.For<IStateBehaviour>();

            state = new BehaviouralState(stateBehaviour1, stateBehaviour2, stateBehaviour3);

            Assert.DoesNotThrow(() => state.Enter());
        }

        [Test]
        public void Throw_An_Exception_When_User_Tries_To_Update_While_Iterating()
        {
            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();
            var stateBehaviour3 = Substitute.For<IStateBehaviour>();

            var state = new BehaviouralState(stateBehaviour1, stateBehaviour2, stateBehaviour3);

            stateBehaviour2.When(behaviour => behaviour.OnEnter())
                .Do(callbackInfo => state.Update());

            Assert.Throws<InvalidOperationException>(() => state.Enter());
            Assert.Throws<InvalidOperationException>(() => state.Update());
            Assert.Throws<InvalidOperationException>(() => state.Exit());

            stateBehaviour1 = Substitute.For<IStateBehaviour>();
            stateBehaviour2 = Substitute.For<IStateBehaviour>();
            stateBehaviour3 = Substitute.For<IStateBehaviour>();

            state = new BehaviouralState(stateBehaviour1, stateBehaviour2, stateBehaviour3);

            Assert.DoesNotThrow(() => state.Update());
        }

        [Test]
        public void Throw_An_Exception_When_User_Tries_To_Exit_While_Iterating()
        {
            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();
            var stateBehaviour3 = Substitute.For<IStateBehaviour>();

            var state = new BehaviouralState(stateBehaviour1, stateBehaviour2, stateBehaviour3);

            stateBehaviour2.When(behaviour => behaviour.OnEnter())
                .Do(callbackInfo => state.Exit());

            Assert.Throws<InvalidOperationException>(() => state.Enter());
            Assert.Throws<InvalidOperationException>(() => state.Update());
            Assert.Throws<InvalidOperationException>(() => state.Exit());

            stateBehaviour1 = Substitute.For<IStateBehaviour>();
            stateBehaviour2 = Substitute.For<IStateBehaviour>();
            stateBehaviour3 = Substitute.For<IStateBehaviour>();

            state = new BehaviouralState(stateBehaviour1, stateBehaviour2, stateBehaviour3);

            Assert.DoesNotThrow(() => state.Exit());
        }
    }
}