using NSubstitute;
using NUnit.Framework;
using Paps.StateMachines;
using Paps.StateMachines.Extensions;
using System;
using System.Linq;
using System.Threading;

namespace Tests.WithStructs
{
    public class StateMachineExtensionsShould
    {
        [Test]
        public void Get_State()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

            fsm.AddState(1, state1);

            var shouldBeState1 = fsm.GetState<IState, int, int>();

            Assert.AreEqual(state1, shouldBeState1);
        }

        [Test]
        public void Get_States()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            var states = fsm.GetStates<IState, int, int>();

            Assert.IsTrue(states.Contains(state1) && states.Contains(state2));
        }

        [Test]
        public void Return_Corresponding_Value_When_Asked_If_Contains_By_Reference()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            Assert.IsTrue(fsm.ContainsStateByReference(state1));
        }
        
        [Test]
        public void Add_Timer_State()
        {
            var stateAfter = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

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
        public void Add_Empty()
        {
            var stateAfter = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

            fsm.AddState(2, stateAfter);
            fsm.AddEmpty(1);
            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.InitialState = 1;

            fsm.Start();

            fsm.Trigger(0);

            stateAfter.Received().Enter();
        }

        [Test]
        public void Add_With_Events()
        {
            var stateAfter = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

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
        public void Add_Transition_From_Any_State()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();
            var state3 = Substitute.For<IState>();
            var state4 = Substitute.For<IState>();
            var stateTarget = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

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
        public void Add_Transition_From_Any_State_Except_Target()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();
            var state3 = Substitute.For<IState>();
            var state4 = Substitute.For<IState>();
            var stateTarget = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

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
        public void Return_Transitions_With_Specific_Trigger()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();
            var stateTarget = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

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
        public void Return_Transitions_With_Specific_State_From()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

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
        public void Return_Transitions_With_Specific_State_To()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

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
        public void Return_Transition_With_Related_State()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

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
        public void Return_If_Has_Transition_With_Specific_State_From()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddTransition(new Transition<int, int>(1, 0, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 1));
            fsm.AddTransition(new Transition<int, int>(2, 1, 1));

            Assert.IsTrue(fsm.ContainsTransitionWithStateFrom(1));
            Assert.IsFalse(fsm.ContainsTransitionWithStateFrom(3));
        }

        [Test]
        public void Return_If_Has_Transition_With_Specific_State_To()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddTransition(new Transition<int, int>(1, 0, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 2));

            Assert.IsTrue(fsm.ContainsTransitionWithStateTo(1));
            Assert.IsFalse(fsm.ContainsTransitionWithStateTo(3));
        }

        [Test]
        public void Return_If_Has_Transition_With_Specific_Trigger()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddTransition(new Transition<int, int>(1, 0, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 1));
            fsm.AddTransition(new Transition<int, int>(1, 1, 2));

            Assert.IsTrue(fsm.ContainsTransitionWithTrigger(1));
            Assert.IsFalse(fsm.ContainsTransitionWithTrigger(2));
        }

        [Test]
        public void Return_If_Has_Transition_Related_To_Specific_State()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

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
        public void Remove_All_Transitions()
        {
            var fsm = new PlainStateMachine<int, int>();

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
        public void Remove_All_Transitions_Related_To_Specific_State()
        {
            var fsm = new PlainStateMachine<int, int>();

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

        [Test]
        public void Return_Corresponding_Value_When_User_Asks_If_Contains_All_States()
        {
            var fsm = new PlainStateMachine<int, int>();

            fsm.AddEmpty(1);
            fsm.AddEmpty(2);
            fsm.AddEmpty(3);

            Assert.IsTrue(fsm.ContainsAll(1, 2, 3));
            Assert.IsFalse(fsm.ContainsAll(1, 2, 3, 4));
        }

        [Test]
        public void Add_Multiple_States()
        {
            IState state1 = Substitute.For<IState>();
            IState state2 = Substitute.For<IState>();

            var fsm = new PlainStateMachine<int, int>();

            fsm.AddStates((1, state1), (2, state2));
        }

        [Test]
        public void Throw_An_Exception_If_A_State_Is_Already_Added_When_Adding_Multiple_States_Without_Side_Effects()
        {
            var fsm = new PlainStateMachine<int, int>();

            IState state1 = Substitute.For<IState>();
            IState state2 = Substitute.For<IState>();

            fsm.AddState(2, state2);

            Assert.Throws<StateIdAlreadyAddedException>(() => fsm.AddStates((1, state1), (2, state2)));
            Assert.IsFalse(fsm.ContainsState(1));
        }

        [Test]
        public void Add_Multiple_Empty_States()
        {
            var fsm = new PlainStateMachine<int, int>();

            fsm.AddEmptyStates(1, 2, 3, 4);

            Assert.IsTrue(fsm.ContainsAll(1, 2, 3, 4));
        }

        [Test]
        public void Configure_With_States_As_Triggers_With_No_Reentrant()
        {
            var fsm = new PlainStateMachine<int, int>();

            fsm.AddEmptyStates(1, 2, 3);

            fsm.ConfigureWithStatesAsTriggersWithNoReentrant();

            Assert.IsTrue(
                fsm.ContainsTransition(new Transition<int, int>(1, 2, 2)) &&
                fsm.ContainsTransition(new Transition<int, int>(1, 3, 3)) &&
                fsm.ContainsTransition(new Transition<int, int>(2, 1, 1)) &&
                fsm.ContainsTransition(new Transition<int, int>(2, 3, 3)) &&
                fsm.ContainsTransition(new Transition<int, int>(3, 1, 1)) &&
                fsm.ContainsTransition(new Transition<int, int>(3, 2, 2))
                );
        }

        [Test]
        public void Configure_With_States_As_Triggers()
        {
            var fsm = new PlainStateMachine<int, int>();

            fsm.AddEmptyStates(1, 2, 3);

            fsm.ConfigureWithStatesAsTriggers();

            Assert.IsTrue(
                fsm.ContainsTransition(new Transition<int, int>(1, 1, 1)) &&
                fsm.ContainsTransition(new Transition<int, int>(1, 2, 2)) &&
                fsm.ContainsTransition(new Transition<int, int>(1, 3, 3)) &&
                fsm.ContainsTransition(new Transition<int, int>(2, 2, 2)) &&
                fsm.ContainsTransition(new Transition<int, int>(2, 1, 1)) &&
                fsm.ContainsTransition(new Transition<int, int>(2, 3, 3)) &&
                fsm.ContainsTransition(new Transition<int, int>(3, 3, 3)) &&
                fsm.ContainsTransition(new Transition<int, int>(3, 1, 1)) &&
                fsm.ContainsTransition(new Transition<int, int>(3, 2, 2))
                );
        }

        [Test]
        public void Add_Composite_States()
        {
            var fsm = new PlainStateMachine<int, int>();

            IState state1 = Substitute.For<IState>();
            IState state2 = Substitute.For<IState>();

            fsm.AddComposite(1, state1, state2);

            Assert.IsTrue(fsm.ContainsState(1));

            fsm.InitialState = 1;

            fsm.Start();

            state1.Received(1).Enter();
            state2.Received(1).Enter();

            fsm.Update();

            state1.Received(1).Update();
            state2.Received(1).Update();

            fsm.Stop();

            state1.Received(1).Exit();
            state2.Received(1).Exit();
        }

        [Test]
        public void Add_Multiple_Event_Handlers()
        {
            var fsm = new PlainStateMachine<int, int>();

            IStateEventHandler eventHandler1 = Substitute.For<IStateEventHandler>();
            IStateEventHandler eventHandler2 = Substitute.For<IStateEventHandler>();

            fsm.AddEmpty(1);

            fsm.SubscribeEventHandlersTo(1, eventHandler1, eventHandler2);

            Assert.IsTrue(fsm.HasEventHandlerOn(1, eventHandler1));
            Assert.IsTrue(fsm.HasEventHandlerOn(1, eventHandler2));
        }

        [Test]
        public void Add_And_Remove_Event_Handlers_From_Delegates()
        {
            var fsm = new PlainStateMachine<int, int>();

            fsm.AddEmpty(1);

            Func<IEvent, bool> method1 = Substitute.For<Func<IEvent, bool>>();
            Func<IEvent, bool> method2 = Substitute.For<Func<IEvent, bool>>();

            IEvent stateEvent = Substitute.For<IEvent>();

            method1.Invoke(stateEvent).Returns(false);
            method2.Invoke(stateEvent).Returns(true);

            fsm.SubscribeEventHandlerTo(1, method1);
            fsm.SubscribeEventHandlerTo(1, method2);

            Assert.IsTrue(fsm.HasEventHandler(1, method1));
            Assert.IsTrue(fsm.HasEventHandler(1, method2));

            fsm.InitialState = 1;

            fsm.Start();

            Assert.IsTrue(fsm.SendEvent(stateEvent));

            method1.Received(1).Invoke(stateEvent);
            method2.Received(1).Invoke(stateEvent);

            fsm.UnsubscribeEventHandlerFrom(1, method1);
            fsm.UnsubscribeEventHandlerFrom(1, method2);

            Assert.IsFalse(fsm.HasEventHandler(1, method1));
            Assert.IsFalse(fsm.HasEventHandler(1, method2));
        }

        [Test]
        public void Add_Multiple_Event_Handlers_From_Methods()
        {
            var fsm = new PlainStateMachine<int, int>();

            fsm.AddEmpty(1);

            Func<IEvent, bool> method1 = Substitute.For<Func<IEvent, bool>>();
            Func<IEvent, bool> method2 = Substitute.For<Func<IEvent, bool>>();

            fsm.SubscribeEventHandlersTo(1, method1, method2);

            Assert.IsTrue(fsm.HasEventHandler(1, method1));
            Assert.IsTrue(fsm.HasEventHandler(1, method2));
        }

        [Test]
        public void Iterate_Over_Transitions()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();
            var state3 = Substitute.For<IState>();
            var state4 = Substitute.For<IState>();

            var transition1 = new Transition<int, int>(1, 2, 3);
            var transition2 = new Transition<int, int>(4, 5, 6);

            PlainStateMachine<int, int> fsm = new PlainStateMachine<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(3, state2);

            fsm.AddState(4, state3);
            fsm.AddState(6, state4);

            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);

            Transition<int, int> item1 = default;
            Transition<int, int> item2 = default;

            int cont = 1;

            fsm.ForeachTransition(
                transition =>
                {
                    if (cont == 1)
                    {
                        item1 = transition;
                    }
                    else
                    {
                        item2 = transition;
                    }

                    cont++;

                    return false;
                }
                );

            Assert.IsTrue(item1.Equals(transition1) && item2.Equals(transition2));
        }

        [Test]
        public void Iterate_Over_States()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            IState item1 = null;
            IState item2 = null;

            PlainStateMachine<int, int> fsm = new PlainStateMachine<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            int cont = 1;

            fsm.ForeachState(
                state =>
                {
                    if (cont == 1)
                    {
                        item1 = fsm.GetStateById(state);
                    }
                    else
                    {
                        item2 = fsm.GetStateById(state);
                    }

                    cont++;

                    return false;
                }
                );

            Assert.IsTrue(fsm.GetStateById(1) == item1 && fsm.GetStateById(2) == item2);
        }
    }
}
