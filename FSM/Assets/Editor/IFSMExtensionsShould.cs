using NSubstitute;
using Paps.FSM;
using Paps.FSM.Extensions;
using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Tests
{
    public class IFSMExtensionsShould
    {
        [Test]
        public void GetState()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);

            var shouldBeState1 = fsm.GetState<IFSMState<int, int>, int, int>();

            Assert.AreEqual(state1, shouldBeState1);
        }

        [Test]
        public void GetStates()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);
            state2.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            var states = fsm.GetStates<IFSMState<int, int>, int, int>();

            Assert.IsTrue(states.Contains(state1) && states.Contains(state2));
        }

        [Test]
        public void ReturnCorrespondingValueWhenAskedIfContainsByReference()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);
            state2.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            Assert.IsTrue(fsm.ContainsStateByReference(state1));
        }

        [Test]
        public void ReturnStateById()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);
            state2.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            Assert.AreEqual(fsm.GetStateById<IFSMState<int, int>, int, int>(1), state1);
        }

        [Test]
        public void AddTimerState()
        {
            var stateAfter = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            stateAfter.StateMachine.Returns(fsm);

            fsm.AddState(2, stateAfter)
                .AddTimerState(1, 1000, stateId => fsm.Trigger(0))
                .AddTransitionWithValuesOf(new FSMTransition<int, int>(1, 0, 2));

            fsm.SetInitialState(1);

            fsm.Start();

            Thread.Sleep(1200);

            fsm.Update();

            stateAfter.Received().Enter();
        }

        [Test]
        public void AddEmpty()
        {
            var stateAfter = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            stateAfter.StateMachine.Returns(fsm);

            fsm.AddState(2, stateAfter)
                .AddEmpty(1)
                .AddTransitionWithValuesOf(new FSMTransition<int, int>(1, 0, 2));

            fsm.SetInitialState(1);

            fsm.Start();

            fsm.Trigger(0);

            stateAfter.Received().Enter();
        }

        [Test]
        public void AddWithEvents()
        {
            var stateAfter = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            stateAfter.StateMachine.Returns(fsm);

            Action enterEvent = Substitute.For<Action>();
            Action updateEvent = Substitute.For<Action>();
            Action exitEvent = Substitute.For<Action>();

            fsm.AddState(2, stateAfter)
                .AddWithEvents(1, enterEvent, updateEvent, exitEvent)
                .AddTransitionWithValuesOf(new FSMTransition<int, int>(1, 0, 2));

            fsm.SetInitialState(1);

            fsm.Start();

            enterEvent.Received().Invoke();

            fsm.Update();

            updateEvent.Received().Invoke();

            fsm.Trigger(0);

            stateAfter.Received().Enter();

            exitEvent.Received().Invoke();
        }

        [Test]
        public void AddTransitionFromAnyState()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();
            var state3 = Substitute.For<IFSMState<int, int>>();
            var state4 = Substitute.For<IFSMState<int, int>>();
            var stateTarget = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1)
                .AddState(2, state2)
                .AddState(3, state3)
                .AddState(4, state4)
                .AddState(5, stateTarget)
                .FromAny(0, 5);

            Assert.IsTrue(
                fsm.ContainsTransition(1, 0, 5) &&
                fsm.ContainsTransition(2, 0, 5) &&
                fsm.ContainsTransition(3, 0, 5) &&
                fsm.ContainsTransition(4, 0, 5) &&
                fsm.ContainsTransition(5, 0, 5)
                        );
        }

        [Test]
        public void AddTransitionFromAnyStateExceptTarget()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();
            var state3 = Substitute.For<IFSMState<int, int>>();
            var state4 = Substitute.For<IFSMState<int, int>>();
            var stateTarget = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1)
                .AddState(2, state2)
                .AddState(3, state3)
                .AddState(4, state4)
                .AddState(5, stateTarget)
                .FromAnyExceptTarget(0, 5);

            Assert.IsTrue(
                fsm.ContainsTransition(1, 0, 5) &&
                fsm.ContainsTransition(2, 0, 5) &&
                fsm.ContainsTransition(3, 0, 5) &&
                fsm.ContainsTransition(4, 0, 5)
                        );

            Assert.IsFalse(fsm.ContainsTransition(5, 0, 5));
        }

        [Test]
        public void ReturnTransitionsWithSpecificTrigger()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();
            var stateTarget = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1)
                .AddState(2, state2)
                .AddState(3, stateTarget)
                .FromAny(0, 3)
                .AddTransition(1, 1, 1);

            var transitions = fsm.GetTransitionsWithTrigger(0);

            Assert.IsFalse(HasAnyWithoutTrigger(transitions, 0));
            Assert.IsFalse(HasAnyWithTrigger(transitions, 1));

            bool HasAnyWithTrigger<TState, TTrigger>(IFSMTransition<TState, TTrigger>[] transitionArray, TTrigger trigger)
            {
                foreach (var transition in transitionArray)
                {
                    if (transition.Trigger.Equals(trigger))
                    {
                        return true;
                    }
                }

                return false;
            }

            bool HasAnyWithoutTrigger<TState, TTrigger>(IFSMTransition<TState, TTrigger>[] transitionArray, TTrigger trigger)
            {
                foreach (var transition in transitionArray)
                {
                    if (transition.Trigger.Equals(trigger) == false)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [Test]
        public void ReturnTransitionsWithSpecificStateFrom()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1)
                .AddState(2, state2)
                .AddTransition(1, 0, 1)
                .AddTransition(1, 1, 1)
                .AddTransition(2, 1, 1);

            var transitions = fsm.GetTransitionsWithStateFrom(1);

            Assert.IsFalse(HasAnyWithoutStateFrom(transitions, 1));
            Assert.IsFalse(HasAnyWithStateFrom(transitions, 2));

            bool HasAnyWithStateFrom<TState, TTrigger>(IFSMTransition<TState, TTrigger>[] transitionArray, TState stateFrom)
            {
                foreach (var transition in transitionArray)
                {
                    if (transition.StateFrom.Equals(stateFrom))
                    {
                        return true;
                    }
                }

                return false;
            }

            bool HasAnyWithoutStateFrom<TState, TTrigger>(IFSMTransition<TState, TTrigger>[] transitionArray, TState stateFrom)
            {
                foreach (var transition in transitionArray)
                {
                    if (transition.StateFrom.Equals(stateFrom) == false)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [Test]
        public void ReturnTransitionsWithSpecificStateTo()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1)
                .AddState(2, state2)
                .AddTransition(1, 0, 1)
                .AddTransition(1, 1, 1)
                .AddTransition(1, 1, 2);

            var transitions = fsm.GetTransitionsWithStateTo(1);

            Assert.IsFalse(HasAnyWithoutStateTo(transitions, 1));
            Assert.IsFalse(HasAnyWithStateTo(transitions, 2));

            bool HasAnyWithStateTo<TState, TTrigger>(IFSMTransition<TState, TTrigger>[] transitionArray, TState stateTo)
            {
                foreach (var transition in transitionArray)
                {
                    if (transition.StateTo.Equals(stateTo))
                    {
                        return true;
                    }
                }

                return false;
            }

            bool HasAnyWithoutStateTo<TState, TTrigger>(IFSMTransition<TState, TTrigger>[] transitionArray, TState stateTo)
            {
                foreach (var transition in transitionArray)
                {
                    if (transition.StateTo.Equals(stateTo) == false)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

    }
}
