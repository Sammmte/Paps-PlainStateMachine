using NSubstitute;
using NUnit.Framework;
using Paps.StateMachines;
using Paps.StateMachines.Extensions;
using System;
using System.Collections.Generic;
using Paps.StateMachines.Extensions.BehaviouralStates;
using System.Linq;

namespace Tests
{
    public abstract class PlainStateMachineShould<TState, TTrigger>
    {
        protected abstract PlainStateMachine<TState, TTrigger> NewStateMachine();

        protected abstract TState NewStateId();

        protected abstract TTrigger NewTrigger();

        protected abstract Transition<TState, TTrigger> NewTransition();

        protected abstract Transition<TState, TTrigger> NewTransition(TState stateFrom, TTrigger trigger, TState stateTo);

        [Test]
        public void Add_And_Remove_States()
        {
            var state1 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddState(stateId1, state1);

            Assert.IsTrue(fsm.StateCount == 1 && fsm.ContainsState(stateId1) && fsm.GetStateById(stateId1) == state1);

            fsm.RemoveState(stateId1);

            Assert.IsTrue(fsm.StateCount == 0 && fsm.ContainsState(stateId1) == false);
        }

        [Test]
        public void Throw_An_Exception_If_User_Adds_State_With_Existing_Id()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            Assert.Throws<StateIdAlreadyAddedException>(
                () =>
                {
                    fsm.AddState(stateId1, state1);
                    fsm.AddState(stateId1, state2);
                });
        }

        [Test]
        public void Throw_An_Exception_If_User_Adds_A_Null_State()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            Assert.Throws<ArgumentNullException>(() => fsm.AddState(stateId1, null));
        }

        [Test]
        public void Remove_States()
        {
            var state = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddState(stateId1, state);

            Assert.IsTrue(fsm.StateCount == 1);

            fsm.RemoveState(stateId1);

            Assert.IsTrue(fsm.StateCount == 0);
        }

        [Test]
        public void Add_And_Remove_Transitions()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddTransition(transition);

            Assert.IsTrue(fsm.TransitionCount == 1 && fsm.ContainsTransition(transition));

            Assert.IsTrue(fsm.RemoveTransition(transition));

            Assert.IsTrue(fsm.TransitionCount == 0 && fsm.ContainsTransition(transition) == false);
        }

        [Test]
        public void Remove_Transitions()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            fsm.AddTransition(transition);

            Assert.IsTrue(fsm.TransitionCount == 1);

            fsm.RemoveTransition(transition);

            Assert.IsTrue(fsm.TransitionCount == 0);
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Start_And_It_Is_Already_Started()
        {
            var state1 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddState(stateId1, state1);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.Throws<StateMachineStartedException>(() => fsm.Start());
        }

        [Test]
        public void Show_Corresponding_Value_When_Asked_If_Is_Started()
        {
            var state1 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddState(stateId1, state1);

            fsm.InitialState = stateId1;

            Assert.IsFalse(fsm.IsStarted);

            fsm.Start();

            Assert.IsTrue(fsm.IsStarted);
        }

        [Test]
        public void Enter_Initial_Start_When_Started()
        {
            var state1 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddState(stateId1, state1);

            fsm.InitialState = stateId1;

            fsm.Start();

            state1.Received().Enter();
        }

        [Test]
        public void Returns_Corresponding_Value_When_Asked_Is_In_State()
        {
            var state1 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddState(stateId1, state1);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.IsTrue(fsm.CurrentState.Equals(stateId1));
        }

        [Test]
        public void Change_State_When_Triggering_An_Existing_Transition()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            fsm.InitialState = stateId1;

            fsm.AddTransition(NewTransition(stateId1, trigger, stateId2));

            fsm.Start();

            Assert.IsTrue(fsm.CurrentState.Equals(stateId1));

            fsm.Trigger(trigger);

            Assert.IsTrue(fsm.CurrentState.Equals(stateId2));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Trigger_When_Is_Not_Started()
        {
            var fsm = NewStateMachine();

            var trigger = NewTrigger();

            Assert.Throws<StateMachineNotStartedException>(() => fsm.Trigger(trigger));
        }

        [Test]
        public void Return_Corresponding_Value_When_Asked_If_Contains_Transition()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            var transition1 = NewTransition(stateId1, trigger, stateId2);
            var transition2 = NewTransition();

            fsm.AddTransition(transition1);

            Assert.IsTrue(fsm.ContainsTransition(transition1));
            Assert.IsFalse(fsm.ContainsTransition(transition2));
        }

        [Test]
        public void Return_Corresponding_Value_When_Asked_If_Contains_State()
        {
            var state1 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            fsm.AddState(stateId1, state1);

            Assert.IsTrue(fsm.ContainsState(stateId1));
            Assert.IsFalse(fsm.ContainsState(stateId2));
        }

        [Test]
        public void Exit_Current_State_When_Stopped()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            fsm.InitialState = stateId1;

            fsm.AddTransition(NewTransition(stateId1, trigger, stateId2));

            fsm.Start();

            fsm.Trigger(trigger);

            fsm.Stop();

            state2.Received().Exit();
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Update_And_It_Is_Not_Started()
        {
            var state1 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddState(stateId1, state1);

            fsm.InitialState = stateId1;

            Assert.Throws<StateMachineNotStartedException>(fsm.Update);
        }

        [Test]
        public void Update_Current_State()
        {
            var state1 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddState(stateId1, state1);

            fsm.InitialState = stateId1;

            fsm.Start();

            fsm.Update();

            state1.Received().Update();
        }

        [Test]
        public void Raise_State_Changed_Event_When_Has_Successfully_Transitioned()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var stateChangedEventHandler = Substitute.For<StateChanged<TState, TTrigger>>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.OnStateChanged += stateChangedEventHandler;

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            fsm.InitialState = stateId1;

            fsm.AddTransition(NewTransition(stateId1, trigger, stateId2));

            fsm.Start();

            fsm.Trigger(trigger);

            stateChangedEventHandler
                .Received(1)
                .Invoke(stateId1, trigger, stateId2);
        }

        [Test]
        public void Raise_Before_State_Changes_Event_When_Has_Successfully_Transitioned()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var stateChangedEventHandler = Substitute.For<StateChanged<TState, TTrigger>>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.OnBeforeStateChanges += stateChangedEventHandler;

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            fsm.InitialState = stateId1;

            fsm.AddTransition(NewTransition(stateId1, trigger, stateId2));

            fsm.Start();

            fsm.Trigger(trigger);

            stateChangedEventHandler
                .Received(1)
                .Invoke(stateId1, trigger, stateId2);
        }

        [Test]
        public void Add_And_Remove_Guard_Conditions()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddTransition(transition);

            fsm.AddGuardConditionTo(transition, TestGuardCondition);

            Assert.IsTrue(fsm.ContainsGuardConditionOn(transition, TestGuardCondition));

            Assert.IsTrue(fsm.RemoveGuardConditionFrom(transition, TestGuardCondition));

            Assert.IsFalse(fsm.ContainsGuardConditionOn(transition, TestGuardCondition));

            bool TestGuardCondition()
            {
                return true;
            }
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Add_Or_Remove_A_Null_Guard_Condition()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddTransition(transition);

            Assert.Throws<ArgumentNullException>(() => fsm.AddGuardConditionTo(transition, null));

            Assert.Throws<ArgumentNullException>(() => fsm.RemoveGuardConditionFrom(transition, null));

        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Add_Or_Remove_Guard_Condition_On_A_Not_Added_Transition()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            var transition = NewTransition(stateId1, trigger, stateId2);

            Assert.Throws<TransitionNotAddedException>(() => fsm.AddGuardConditionTo(transition, TestGuardCondition));

            Assert.Throws<TransitionNotAddedException>(() => fsm.RemoveGuardConditionFrom(transition, TestGuardCondition));

            bool TestGuardCondition()
            {
                return true;
            }
        }

        [Test]
        public void Transition_If_All_Guard_Conditions_Return_True()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddTransition(transition);

            Func<bool> guardCondition1 = Substitute.For<Func<bool>>();
            Func<bool> guardCondition2 = Substitute.For<Func<bool>>();

            guardCondition1.Invoke().Returns(true);
            guardCondition2.Invoke().Returns(true);

            fsm.AddGuardConditionTo(transition, guardCondition1);

            fsm.AddGuardConditionTo(transition, guardCondition2);

            fsm.InitialState = stateId1;

            fsm.Start();

            fsm.Trigger(trigger);

            guardCondition1.Received().Invoke();
            guardCondition2.Received().Invoke();

            Assert.IsTrue(fsm.CurrentState.Equals(stateId2));

        }

        [Test]
        public void Not_Transition_If_Any_Guard_Condition_Returns_False()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddTransition(transition);

            Func<bool> guardCondition1 = Substitute.For<Func<bool>>();
            Func<bool> guardCondition2 = Substitute.For<Func<bool>>();
            Func<bool> guardCondition3 = Substitute.For<Func<bool>>();

            guardCondition1.Invoke().Returns(true);
            guardCondition2.Invoke().Returns(false);
            guardCondition3.Invoke().Returns(true);

            fsm.AddGuardConditionTo(transition, guardCondition1);

            fsm.AddGuardConditionTo(transition, guardCondition2);

            fsm.AddGuardConditionTo(transition, guardCondition3);

            fsm.InitialState = stateId1;

            fsm.Start();

            fsm.Trigger(trigger);

            guardCondition1.Received().Invoke();
            guardCondition2.Received().Invoke();
            guardCondition3.DidNotReceive().Invoke();

            Assert.IsFalse(fsm.CurrentState.Equals(stateId2));
        }

        [Test]
        public void Reenter_State_When_State_To_Is_Equal_To_State_From()
        {
            var state1 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddState(stateId1, state1);

            fsm.AddTransition(NewTransition(stateId1, trigger, stateId1));

            fsm.InitialState = stateId1;

            fsm.Start();

            fsm.Trigger(trigger);

            state1.Received().Exit();
            state1.Received(2).Enter();
        }

        [Test]
        public void Remove_Transitions_Related_To_A_State_Id_When_It_Is_Removed()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            var transition1 = NewTransition(stateId1, trigger, stateId1);
            var transition2 = NewTransition(stateId1, trigger, stateId2);
            var transition3 = NewTransition(stateId2, trigger, stateId1);

            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);
            fsm.AddTransition(transition3);

            fsm.RemoveState(stateId2);

            Assert.IsFalse(fsm.ContainsTransition(transition2) && fsm.ContainsTransition(transition3));
            Assert.IsTrue(fsm.ContainsTransition(transition1));
        }

        [Test]
        public void Remove_Guard_Conditions_Related_To_A_Transition_When_It_Is_Removed()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddTransition(transition);

            fsm.AddGuardConditionTo(transition, TestGuardCondition);
            fsm.AddGuardConditionTo(transition, TestGuardCondition2);

            fsm.RemoveState(stateId1);

            Assert.IsFalse(fsm.ContainsGuardConditionOn(transition, TestGuardCondition));
            Assert.IsFalse(fsm.ContainsGuardConditionOn(transition, TestGuardCondition2));

            bool TestGuardCondition()
            {
                return true;
            }

            bool TestGuardCondition2()
            {
                return true;
            }
        }

        [Test]
        public void Transition_Queued()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();
            var stateId4 = NewStateId();
            var stateId5 = NewStateId();

            var trigger = NewTrigger();

            var transition1 = NewTransition(stateId1, trigger, stateId2);
            var transition2 = NewTransition(stateId2, trigger, stateId3);
            var transition3 = NewTransition(stateId3, trigger, stateId4);
            var transition4 = NewTransition(stateId4, trigger, stateId5);

            var state1 = Substitute.For<IState>();
            var state3 = Substitute.For<IState>();
            var state4 = Substitute.For<IState>();
            var state5 = Substitute.For<IState>();
            var state2 = new DelegateState
                (
                    () =>
                    {
                        fsm.Trigger(transition2.Trigger);
                        fsm.Trigger(transition3.Trigger);
                        fsm.Trigger(transition4.Trigger);
                        state3.DidNotReceive().Enter();
                    },
                    null, null);


            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);
            fsm.AddState(stateId3, state3);
            fsm.AddState(stateId4, state4);
            fsm.AddState(stateId5, state5);
            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);
            fsm.AddTransition(transition3);
            fsm.AddTransition(transition4);
            fsm.InitialState = stateId1;
            fsm.Start();

            fsm.Trigger(transition1.Trigger);

            Assert.IsTrue(fsm.CurrentState.Equals(stateId5));
        }

        [Test]
        public void Throw_An_Exception_When_User_Tries_To_Transition_And_Guard_Conditions_Are_Not_Mutually_Exclusive()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            var trigger1 = NewTrigger();
            var trigger2 = NewTrigger();

            fsm.AddEmpty(stateId1);
            fsm.AddEmpty(stateId2);
            fsm.AddEmpty(stateId3);

            var transition1 = NewTransition(stateId1, trigger1, stateId1);
            var transition2 = NewTransition(stateId1, trigger1, stateId2);
            var transition3 = NewTransition(stateId1, trigger2, stateId2);
            var transition4 = NewTransition(stateId1, trigger2, stateId3);

            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);
            fsm.AddTransition(transition3);
            fsm.AddTransition(transition4);

            fsm.AddGuardConditionTo(transition1, () => true);
            fsm.AddGuardConditionTo(transition2, () => false);
            fsm.AddGuardConditionTo(transition3, () => true);
            fsm.AddGuardConditionTo(transition4, () => true);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.DoesNotThrow(() => fsm.Trigger(trigger1));
            Assert.Throws<MultipleValidTransitionsFromSameStateException>(() => fsm.Trigger(trigger2));
        }

        [Test]
        public void Throw_An_Exception_When_User_Tries_To_Transition_And_Multiple_Transitions_With_Same_Source_And_Trigger_Has_No_Guard_Conditions()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddEmpty(stateId1);
            fsm.AddEmpty(stateId2);
            fsm.AddEmpty(stateId3);

            var transition1 = NewTransition(stateId1, trigger, stateId1);
            var transition2 = NewTransition(stateId1, trigger, stateId2);

            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.Throws<MultipleValidTransitionsFromSameStateException>(() => fsm.Trigger(trigger));
        }

        [Test]
        public void Let_Transition_On_First_Enter()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            var trigger = NewTrigger();

            Action onEnter1 = () => fsm.Trigger(trigger);
            Action onEnter2Substitute = Substitute.For<Action>();
            Action onEnter2 = onEnter2Substitute + (() => fsm.Trigger(trigger));
            Action onEnter3 = Substitute.For<Action>();

            fsm.AddWithEvents(stateId1, onEnter1);
            fsm.AddWithEvents(stateId2, onEnter2);
            fsm.AddWithEvents(stateId3, onEnter3);

            var transition1 = NewTransition(stateId1, trigger, stateId2);
            var transition2 = NewTransition(stateId2, trigger, stateId3);

            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);

            fsm.InitialState = stateId1;

            fsm.Start();

            onEnter2Substitute.Received();
            onEnter3.Received();

            Assert.IsTrue(fsm.CurrentState.Equals(stateId3));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Start_On_First_Enter()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            Action onEnter = () => fsm.Start();

            fsm.AddWithEvents(stateId1, onEnter);

            fsm.InitialState = stateId1;

            Assert.Throws<StateMachineStartedException>(() => fsm.Start());
        }

        [Test]
        public void Let_User_Stop_It_On_First_Enter()
        {
            var fsm = NewStateMachine();

            Action onEnter = () => fsm.Stop();
            Action onExit = Substitute.For<Action>();

            var stateId1 = NewStateId();

            fsm.AddWithEvents(stateId1, onEnter, onExit);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.IsFalse(fsm.IsStarted);

            onExit.Received();
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Transition_On_Exit_Of_State_When_Stopped()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var trigger = NewTrigger();

            Action onEnter = () => fsm.Stop();
            Action onExit = () => fsm.Trigger(trigger);

            fsm.AddWithEvents(stateId1, onEnter, onExit);

            fsm.InitialState = stateId1;

            Assert.Throws<StateMachineStoppingException>(() => fsm.Start());
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Send_An_Event_And_Is_Not_Started()
        {
            var fsm = NewStateMachine();

            Assert.Throws<StateMachineNotStartedException>(() => fsm.SendEvent(Substitute.For<IEvent>()));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Comparer_Is_Null()
        {
            Assert.Throws<ArgumentNullException>(() => new PlainStateMachine<int, int>(null, null));

            var fsm = NewStateMachine();

            Assert.Throws<ArgumentNullException>(() => fsm.SetStateComparer(null));
            Assert.Throws<ArgumentNullException>(() => fsm.SetTriggerComparer(null));
        }

        [Test]
        public void Return_State_By_Id()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var fsm = NewStateMachine();

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            Assert.AreEqual(fsm.GetStateById(stateId1), state1);
            Assert.AreEqual(fsm.GetStateById(stateId2), state2);
        }

        [Test]
        public void Change_Equality_Comparer_After_Construction()
        {
            IEqualityComparer<TState> comparer = Substitute.For<IEqualityComparer<TState>>();

            comparer.Equals(Arg.Any<TState>(), Arg.Any<TState>()).Returns(true);

            var state1 = Substitute.For<IState>();

            var stateId1 = NewStateId();

            var fsm = NewStateMachine();

            fsm.SetStateComparer(comparer);

            fsm.AddState(stateId1, state1);

            comparer.Received(1).Equals(Arg.Any<TState>(), Arg.Any<TState>());
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Get_Current_State_While_Not_Started()
        {
            var fsm = NewStateMachine();

            Assert.Throws<StateMachineNotStartedException>(() => fsm.CurrentState.ToString());
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Remove_Current_State()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddEmpty(stateId1);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.Throws<InvalidOperationException>(() => fsm.RemoveState(stateId1));
        }

        [Test]
        public void Throw_An_Exception_If_Is_Empty_When_Started()
        {
            var fsm = NewStateMachine();

            Assert.Throws<EmptyStateMachineException>(() => fsm.Start());
        }

        [Test]
        public void Permit_Remove_State_While_Starting()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            fsm.AddWithEvents(stateId1, () => fsm.RemoveState(stateId2));
            fsm.AddEmpty(stateId2);

            fsm.InitialState = stateId1;

            Assert.DoesNotThrow(() => fsm.Start());
            Assert.IsFalse(fsm.ContainsState(stateId2));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Remove_Next_State_While_In_Transition()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddWithExitEvent(stateId1, () => fsm.RemoveState(stateId2));
            fsm.AddEmpty(stateId2);

            fsm.AddTransition(NewTransition(stateId1, trigger, stateId2));

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.Throws<ProtectedStateException>(() => fsm.Trigger(trigger));
            Assert.IsTrue(fsm.ContainsState(stateId2));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Remove_State_While_Evaluating_Guard_Conditions()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddEmpty(stateId1);
            fsm.AddEmpty(stateId2);

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddTransition(transition);

            IGuardCondition guardCondition = Substitute.For<IGuardCondition>();

            guardCondition.IsValid().Returns(true);

            guardCondition.When(g => g.IsValid()).Do(callback => fsm.RemoveState(stateId2));

            fsm.AddGuardConditionTo(transition, guardCondition);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.Throws<StateMachineEvaluatingTransitionsException>(() => fsm.Trigger(trigger));
            Assert.IsTrue(fsm.ContainsState(stateId2));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Remove_Transition_While_Evaluating_Transitions()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddEmpty(stateId1);
            fsm.AddEmpty(stateId2);

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddTransition(transition);

            IGuardCondition guardCondition = Substitute.For<IGuardCondition>();

            guardCondition.IsValid().Returns(true);

            guardCondition.When(g => g.IsValid()).Do(callback => fsm.RemoveTransition(transition));

            fsm.AddGuardConditionTo(transition, guardCondition);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.Throws<StateMachineEvaluatingTransitionsException>(() => fsm.Trigger(trigger));
            Assert.IsTrue(fsm.ContainsTransition(transition));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Remove_Guard_Condition_While_Evaluating_Transitions()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddEmpty(stateId1);
            fsm.AddEmpty(stateId2);

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddTransition(transition);

            IGuardCondition guardCondition = Substitute.For<IGuardCondition>();

            guardCondition.IsValid().Returns(true);

            guardCondition.When(g => g.IsValid()).Do(callback => fsm.RemoveGuardConditionFrom(transition, guardCondition));

            fsm.AddGuardConditionTo(transition, guardCondition);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.Throws<StateMachineEvaluatingTransitionsException>(() => fsm.Trigger(trigger));
            Assert.IsTrue(fsm.ContainsGuardConditionOn(transition, guardCondition));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Stop_While_In_Transition()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddWithExitEvent(stateId1, () => fsm.Stop());
            fsm.AddEmpty(stateId2);

            fsm.AddTransition(transition);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.Throws<StateMachineTransitioningException>(() => fsm.Trigger(trigger));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Stop_While_Evaluating_Transitions()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddEmpty(stateId1);
            fsm.AddEmpty(stateId2);

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddTransition(transition);

            IGuardCondition guardCondition = Substitute.For<IGuardCondition>();

            guardCondition.IsValid().Returns(true);

            guardCondition.When(g => g.IsValid()).Do(callback => fsm.Stop());

            fsm.AddGuardConditionTo(transition, guardCondition);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.Throws<StateMachineEvaluatingTransitionsException>(() => fsm.Trigger(trigger));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Stop_While_Stopping()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddWithExitEvent(stateId1, () => fsm.Stop());

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.Throws<StateMachineStoppingException>(() => fsm.Stop());
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Start_While_Stopping()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddWithExitEvent(stateId1, () => fsm.Start());

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.Throws<StateMachineStoppingException>(() => fsm.Stop());
        }
        
        [Test]
        public void Permit_Remove_Previous_State_While_New_Current_State_Enters()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddEmpty(stateId1);
            fsm.AddWithEnterEvent(stateId2, () => fsm.RemoveState(stateId1));

            fsm.AddTransition(NewTransition(stateId1, trigger, stateId2));

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.DoesNotThrow(() => fsm.Trigger(trigger));
            Assert.IsFalse(fsm.ContainsState(stateId1));
        }

        [Test]
        public void Permit_Remove_Transition_While_In_Transition()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddEmpty(stateId1);
            fsm.AddWithEnterEvent(stateId2, () => fsm.RemoveTransition(transition));

            fsm.AddTransition(transition);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.DoesNotThrow(() => fsm.Trigger(trigger));
            Assert.IsFalse(fsm.ContainsTransition(transition));
        }

        [Test]
        public void Permit_Remove_Transition_While_In_Queued_Transition()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();
            var stateId3 = NewStateId();

            var trigger = NewTrigger();

            var transition1 = NewTransition(stateId1, trigger, stateId2);
            var transition2 = NewTransition(stateId2, trigger, stateId3);

            fsm.AddEmpty(stateId1);
            fsm.AddWithEnterEvent(stateId2, 
            () => 
            {
                fsm.RemoveTransition(transition1);
                fsm.Trigger(trigger);
            });
            fsm.AddEmpty(stateId3);

            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.DoesNotThrow(() => fsm.Trigger(trigger));
            Assert.IsFalse(fsm.ContainsTransition(transition1));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Set_An_Initial_State_That_Was_Not_Added()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            Assert.Throws<StateIdNotAddedException>(() => fsm.InitialState = stateId1);
        }

        [Test]
        public void Set_Initial_State_Automatically_When_The_First_State_Is_Added()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var stateObj = Substitute.For<IState>();

            fsm.AddState(stateId1, stateObj);

            Assert.AreEqual(stateId1, fsm.InitialState);

            fsm.AddState(stateId2, stateObj);

            Assert.AreNotEqual(stateId2, fsm.InitialState);
        }

        [Test]
        public void Leave_Initial_State_With_Default_Type_Value_If_The_Last_State_Is_Removed()
        {
            var fsm = NewStateMachine();

            var stateId = NewStateId();

            var stateObj = Substitute.For<IState>();

            fsm.AddState(stateId, stateObj);

            fsm.RemoveState(stateId);

            Assert.AreEqual(default(TState), fsm.InitialState);
        }

        [Test]
        public void Permit_Change_Initial_State_If_The_Input_State_Id_Was_Added()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var stateObj = Substitute.For<IState>();

            fsm.AddState(stateId1, stateObj);
            fsm.AddState(stateId2, stateObj);

            Assert.DoesNotThrow(() => fsm.InitialState = stateId2);
            Assert.AreEqual(stateId2, fsm.InitialState);
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Add_Guard_Conditions_When_Evaluating_Transitions()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddEmpty(stateId1);
            fsm.AddEmpty(stateId2);

            var transition = NewTransition(stateId1, trigger, stateId2);

            fsm.AddTransition(transition);

            IGuardCondition guardCondition1 = Substitute.For<IGuardCondition>();
            IGuardCondition guardCondition2 = Substitute.For<IGuardCondition>();

            guardCondition1.IsValid().Returns(true);

            guardCondition1.When(g => g.IsValid()).Do(callback => fsm.AddGuardConditionTo(transition, guardCondition2));

            fsm.AddGuardConditionTo(transition, guardCondition1);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.Throws<StateMachineEvaluatingTransitionsException>(() => fsm.Trigger(trigger));
            Assert.IsFalse(fsm.ContainsGuardConditionOn(transition, guardCondition2));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Add_Transitions_When_Evaluating_Transitions()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddEmpty(stateId1);
            fsm.AddEmpty(stateId2);

            var transition1 = NewTransition(stateId1, trigger, stateId2);
            var transition2 = NewTransition(stateId2, trigger, stateId1);

            fsm.AddTransition(transition1);

            IGuardCondition guardCondition = Substitute.For<IGuardCondition>();

            guardCondition.IsValid().Returns(true);

            guardCondition.When(g => g.IsValid()).Do(callback => fsm.AddTransition(transition2));

            fsm.AddGuardConditionTo(transition1, guardCondition);

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.Throws<StateMachineEvaluatingTransitionsException>(() => fsm.Trigger(trigger));
            Assert.IsFalse(fsm.ContainsTransition(transition2));
        }

        [Test]
        public void Permit_Add_Transitions_While_In_Transition()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            var transition1 = NewTransition(stateId1, trigger, stateId2);
            var transition2 = NewTransition(stateId2, trigger, stateId1);

            fsm.AddWithExitEvent(stateId1, () => fsm.AddTransition(transition2));
            fsm.AddEmpty(stateId2);

            fsm.AddTransition(transition1);

            fsm.Start();

            Assert.DoesNotThrow(() => fsm.Trigger(trigger));

            Assert.That(fsm.ContainsTransition(transition2), "Contains transition 2");
        }

        [Test]
        public void Permit_Add_Guard_Conditions_While_In_Transition()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            var transition = NewTransition(stateId1, trigger, stateId2);

            var guardCondition = Substitute.For<IGuardCondition>();

            fsm.AddWithExitEvent(stateId1, () => fsm.AddGuardConditionTo(transition, guardCondition));
            fsm.AddEmpty(stateId2);

            fsm.AddTransition(transition);

            fsm.Start();

            Assert.DoesNotThrow(() => fsm.Trigger(trigger));
            Assert.That(fsm.ContainsGuardConditionOn(transition, guardCondition), "Contains guard condition");
        }

        [Test]
        public void Return_Null_If_There_Is_No_Guard_Conditions_Related_To_An_Existing_Transition()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            var transition = NewTransition(stateId1, trigger, stateId2);
            
            fsm.AddEmpty(stateId1);
            fsm.AddEmpty(stateId2);
            
            fsm.AddTransition(transition);
            
            Assert.IsNull(fsm.GetGuardConditionsOf(transition));
        }

        [Test]
        public void Add_And_Remove_Event_Handler_To_States()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            fsm.AddEmpty(stateId1);

            IStateEventHandler eventHandler = Substitute.For<IStateEventHandler>();

            fsm.SubscribeEventHandlerTo(stateId1, eventHandler);

            Assert.IsTrue(fsm.HasEventHandlerOn(stateId1, eventHandler));

            Assert.IsTrue(fsm.UnsubscribeEventHandlerFrom(stateId1, eventHandler));

            Assert.IsFalse(fsm.HasEventHandlerOn(stateId1, eventHandler));
        }

        [Test]
        public void Send_Event_To_Event_Receivers()
        {
            var fsm = NewStateMachine();

            IState state1 = Substitute.For<IState>();
            IState state2 = Substitute.For<IState>();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            IStateEventHandler eventHandler1 = Substitute.For<IStateEventHandler>();
            IStateEventHandler eventHandler2 = Substitute.For<IStateEventHandler>();

            IEvent stateEvent = Substitute.For<IEvent>();

            stateEvent.GetEventData().Returns(10);

            eventHandler1.HandleEvent(stateEvent).Returns(true);

            fsm.AddState(stateId1, state1);
            fsm.AddState(stateId2, state2);

            fsm.SubscribeEventHandlerTo(stateId1, eventHandler1);
            fsm.SubscribeEventHandlerTo(stateId2, eventHandler2);

            fsm.AddTransition(NewTransition(stateId1, trigger, stateId2));

            fsm.InitialState = stateId1;

            fsm.Start();

            Assert.IsTrue(fsm.SendEvent(stateEvent));

            eventHandler1.Received().HandleEvent(stateEvent);

            fsm.Trigger(trigger);

            Assert.IsFalse(fsm.SendEvent(stateEvent));

            eventHandler2.Received().HandleEvent(stateEvent);
        }

        [Test]
        public void Send_Events_To_Event_Receivers_And_Stop_When_Any_Returns_True()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            IState state1 = Substitute.For<IState>();

            IStateEventHandler eventHandler1 = Substitute.For<IStateEventHandler>();
            IStateEventHandler eventHandler2 = Substitute.For<IStateEventHandler>();
            IStateEventHandler eventHandler3 = Substitute.For<IStateEventHandler>();

            IEvent stateEvent = Substitute.For<IEvent>();

            stateEvent.GetEventData().Returns(10);

            eventHandler1.HandleEvent(stateEvent).Returns(false);
            eventHandler2.HandleEvent(stateEvent).Returns(true);
            eventHandler3.HandleEvent(stateEvent).Returns(false);

            fsm.AddState(stateId1, state1);

            fsm.SubscribeEventHandlerTo(stateId1, eventHandler1);
            fsm.SubscribeEventHandlerTo(stateId1, eventHandler2);
            fsm.SubscribeEventHandlerTo(stateId1, eventHandler3);

            fsm.InitialState = stateId1;

            fsm.Start();

            fsm.SendEvent(stateEvent);

            eventHandler1.Received(1).HandleEvent(stateEvent);
            eventHandler2.Received(1).HandleEvent(stateEvent);
            eventHandler3.DidNotReceive().HandleEvent(stateEvent);
        }

        [Test]
        public void Return_Event_Handlers_Of_A_Specific_State()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var stateObj = Substitute.For<IState>();

            var eventHandler1 = Substitute.For<IStateEventHandler>();
            var eventHandler2 = Substitute.For<IStateEventHandler>();
            var eventHandler3 = Substitute.For<IStateEventHandler>();

            fsm.AddState(stateId1, stateObj);
            fsm.AddState(stateId2, stateObj);

            fsm.SubscribeEventHandlerTo(stateId1, eventHandler1);
            fsm.SubscribeEventHandlerTo(stateId1, eventHandler2);

            fsm.SubscribeEventHandlerTo(stateId2, eventHandler3);

            var eventHandlers = fsm.GetEventHandlersOf(stateId1);

            Assert.Contains(eventHandler1, eventHandlers);
            Assert.Contains(eventHandler2, eventHandlers);
            AssertExtensions.DoesNotContains(eventHandler3, eventHandlers);
        }

        [Test]
        public void Do_Not_Throw_An_Exception_If_User_Asks_If_Has_Event_Handler_On_State_And_There_Is_None()
        {
            var fsm = NewStateMachine();

            var stateId = NewStateId();

            var stateObj = Substitute.For<IState>();

            fsm.AddState(stateId, stateObj);

            Assert.DoesNotThrow(() => fsm.HasEventHandler(stateId, someEvent => false));
        }

        [Test]
        public void Remove_Event_Handlers_Related_To_A_Removed_State()
        {
            var fsm = NewStateMachine();

            var stateId = NewStateId();

            var stateObj = Substitute.For<IState>();
            var eventHandler = Substitute.For<IStateEventHandler>();

            fsm.AddState(stateId, stateObj);

            fsm.SubscribeEventHandlerTo(stateId, eventHandler);

            fsm.RemoveState(stateId);

            Assert.That(() => fsm.HasEventHandlerOn(stateId, eventHandler) == false, "Event was removed the moment the state was removed");
        }

        [Test]
        public void Add_Guard_Conditions()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddEmpty(stateId1);

            var transition = NewTransition(stateId1, trigger, stateId1);
            IGuardCondition guardCondition = Substitute.For<IGuardCondition>();

            fsm.AddTransition(transition);

            fsm.AddGuardConditionTo(transition, guardCondition);

            Assert.That(fsm.ContainsGuardConditionOn(transition, guardCondition), "Contains guard condition on transition");
        }

        [Test]
        public void Remove_Guard_Conditions()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddEmpty(stateId1);

            var transition = NewTransition(stateId1, trigger, stateId1);
            IGuardCondition guardCondition = Substitute.For<IGuardCondition>();

            fsm.AddTransition(transition);

            fsm.AddGuardConditionTo(transition, guardCondition);

            Assert.That(fsm.RemoveGuardConditionFrom(transition, guardCondition), "Guard condition was removed");
            Assert.That(fsm.ContainsGuardConditionOn(transition, guardCondition) == false, "Does not contains guard condition");
        }

        [Test]
        public void Return_Guard_Conditions_Of_A_Specific_Transition()
        {
            var fsm = NewStateMachine();

            var stateId1 = NewStateId();
            var stateId2 = NewStateId();

            var trigger = NewTrigger();

            fsm.AddEmpty(stateId1);
            fsm.AddEmpty(stateId2);

            var transition1 = NewTransition(stateId1, trigger, stateId2);
            var transition2 = NewTransition(stateId2, trigger, stateId1);

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
