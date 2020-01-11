﻿using NSubstitute;
using NUnit.Framework;
using Paps.FSM;
using Paps.FSM.Extensions;
using System;
using System.Linq;
using System.Threading;

namespace Tests
{
    public class FSMExtensionsShould
    {
        [Test]
        public void GetState()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            var shouldBeState1 = fsm.GetState<IState, int, int>();

            Assert.AreEqual(state1, shouldBeState1);
        }

        [Test]
        public void GetStates()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            var states = fsm.GetStates<IState, int, int>();

            Assert.IsTrue(states.Contains(state1) && states.Contains(state2));
        }

        [Test]
        public void ReturnCorrespondingValueWhenAskedIfContainsByReference()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            Assert.IsTrue(fsm.ContainsStateByReference(state1));
        }

        

        [Test]
        public void AddTimerState()
        {
            var stateAfter = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(2, stateAfter);
            fsm.AddTimerState(1, 1000, stateId => fsm.Trigger(0));
            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.InitialState = 1;

            fsm.Start();

            Thread.Sleep(1200);

            fsm.Update();

            stateAfter.Received().Enter();
        }

        [Test]
        public void AddEmpty()
        {
            var stateAfter = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(2, stateAfter);
            fsm.AddEmpty(1);
            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.InitialState = 1;

            fsm.Start();

            fsm.Trigger(0);

            stateAfter.Received().Enter();
        }

        [Test]
        public void AddWithEvents()
        {
            var stateAfter = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            Action enterEvent = Substitute.For<Action>();
            Action updateEvent = Substitute.For<Action>();
            Action exitEvent = Substitute.For<Action>();

            fsm.AddState(2, stateAfter);
            fsm.AddWithEvents(1, enterEvent, updateEvent, exitEvent);
            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.InitialState = 1;

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
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();
            var state3 = Substitute.For<IState>();
            var state4 = Substitute.For<IState>();
            var stateTarget = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddState(3, state3);
            fsm.AddState(4, state4);
            fsm.AddState(5, stateTarget);
            fsm.FromAny(0, 5);

            Assert.IsTrue(
                fsm.ContainsTransition(new Transition<int, int>(1, 0, 5)) &&
                fsm.ContainsTransition(new Transition<int, int>(2, 0, 5)) &&
                fsm.ContainsTransition(new Transition<int, int>(3, 0, 5)) &&
                fsm.ContainsTransition(new Transition<int, int>(4, 0, 5)) &&
                fsm.ContainsTransition(new Transition<int, int>(5, 0, 5))
                        );
        }

        [Test]
        public void AddTransitionFromAnyStateExceptTarget()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();
            var state3 = Substitute.For<IState>();
            var state4 = Substitute.For<IState>();
            var stateTarget = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddState(3, state3);
            fsm.AddState(4, state4);
            fsm.AddState(5, stateTarget);
            fsm.FromAnyExceptTarget(0, 5);

            Assert.IsTrue(
                fsm.ContainsTransition(new Transition<int, int>(1, 0, 5)) &&
                fsm.ContainsTransition(new Transition<int, int>(2, 0, 5)) &&
                fsm.ContainsTransition(new Transition<int, int>(3, 0, 5)) &&
                fsm.ContainsTransition(new Transition<int, int>(4, 0, 5)));

            Assert.IsFalse(fsm.ContainsTransition(new Transition<int, int>(5, 0, 5)));
        }

        [Test]
        public void ReturnTransitionsWithSpecificTrigger()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();
            var stateTarget = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddState(3, stateTarget);
            fsm.FromAny(0, 3);
            fsm.AddTransition(new Transition<int, int>(1, 1, 1));

            var transitions = fsm.GetTransitionsWithTrigger(0);

            Assert.IsTrue(HasAnyWithTrigger(transitions, 0));
            Assert.IsFalse(HasAnyWithTrigger(transitions, 1));

            bool HasAnyWithTrigger<TState, TTrigger>(Transition<TState, TTrigger>[] transitionArray, TTrigger trigger)
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
        }

        [Test]
        public void ReturnTransitionsWithSpecificStateFrom()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddTransition(new Transition<int, int>(1, 0, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 1));
            fsm.AddTransition(new Transition<int, int>(2, 1, 1));

            var transitions = fsm.GetTransitionsWithStateFrom(1);

            Assert.IsTrue(HasAnyWithStateFrom(transitions, 1));
            Assert.IsFalse(HasAnyWithStateFrom(transitions, 2));

            bool HasAnyWithStateFrom<TState, TTrigger>(Transition<TState, TTrigger>[] transitionArray, TState stateFrom)
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
        }

        [Test]
        public void ReturnTransitionsWithSpecificStateTo()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddTransition(new Transition<int, int>(1, 0, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 2));

            var transitions = fsm.GetTransitionsWithStateTo(1);

            Assert.IsTrue(HasAnyWithStateTo(transitions, 1));
            Assert.IsFalse(HasAnyWithStateTo(transitions, 2));

            bool HasAnyWithStateTo<TState, TTrigger>(Transition<TState, TTrigger>[] transitionArray, TState stateTo)
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
        }

        [Test]
        public void ReturnTransitionWithRelatedState()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddTransition(new Transition<int, int>(1, 0, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 1));
            fsm.AddTransition(new Transition<int, int>(2, 0, 2));

            var transitions = fsm.GetTransitionsRelatedTo(1);

            Assert.IsTrue(HasAnyWithState(transitions, 1));
            Assert.IsFalse(HasAnyWithState(transitions, 2));

            bool HasAnyWithState<TState, TTrigger>(Transition<TState, TTrigger>[] transitionArray, TState stateId)
            {
                foreach (var transition in transitionArray)
                {
                    if (transition.StateTo.Equals(stateId) || transition.StateFrom.Equals(stateId))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        [Test]
        public void ReturnIfHasTransitionWithSpecificStateFrom()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddTransition(new Transition<int, int>(1, 0, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 1));
            fsm.AddTransition(new Transition<int, int>(2, 1, 1));

            Assert.IsTrue(fsm.ContainsTransitionWithStateFrom(1));
            Assert.IsFalse(fsm.ContainsTransitionWithStateFrom(3));
        }

        [Test]
        public void ReturnIfHasTransitionWithSpecificStateTo()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddTransition(new Transition<int, int>(1, 0, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 2));

            Assert.IsTrue(fsm.ContainsTransitionWithStateTo(1));
            Assert.IsFalse(fsm.ContainsTransitionWithStateTo(3));
        }

        [Test]
        public void ReturnIfHasTransitionWithSpecificTrigger()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddTransition(new Transition<int, int>(1, 0, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 2));

            Assert.IsTrue(fsm.ContainsTransitionWithTrigger(1));
            Assert.IsFalse(fsm.ContainsTransitionWithTrigger(2));
        }

        [Test]
        public void ReturnIfHasTransitionRelatedToSpecificState()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddTransition(new Transition<int, int>(1, 0, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 2));

            Assert.IsTrue(fsm.ContainsTransitionRelatedTo(1));
            Assert.IsTrue(fsm.ContainsTransitionRelatedTo(2));
            Assert.IsFalse(fsm.ContainsTransitionRelatedTo(0));
        }

        [Test]
        public void RemoveAllTransitions()
        {
            var fsm = new FSM<int, int>();

            fsm.AddEmpty(1);
            fsm.AddEmpty(2);
            fsm.AddEmpty(3);
            fsm.FromAny(0, 1);
            fsm.FromAny(0, 2);
            fsm.FromAny(0, 3);

            fsm.RemoveAllTransitions();

            Assert.IsTrue(fsm.TransitionCount == 0);
        }

        [Test]
        public void RemoveAllTransitionsRelatedToSpecificState()
        {
            var fsm = new FSM<int, int>();

            fsm.AddEmpty(1);
            fsm.AddEmpty(2);
            fsm.AddEmpty(3);
            fsm.FromAny(0, 1);
            fsm.FromAny(0, 2);
            fsm.FromAny(0, 3);

            fsm.RemoveAllTransitionsRelatedTo(1);

            Assert.IsFalse(fsm.ContainsTransitionRelatedTo(1));
            Assert.IsTrue(fsm.ContainsTransitionRelatedTo(2));
            Assert.IsTrue(fsm.ContainsTransitionRelatedTo(3));
        }

        
    }
}
