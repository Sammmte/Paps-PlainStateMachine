using NSubstitute;
using NUnit.Framework;
using Paps.FSM;
using Paps.FSM.Extensions;
using System;
using System.Collections.Generic;

namespace Tests
{
    public class FSMWithStructsShould
    {
        [Test]
        public void Add_And_Remove_States()
        {
            var state1 = Substitute.For<IState>();

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            Assert.IsTrue(fsm.StateCount == 1 && fsm.ContainsState(1) && fsm.GetStateById(1) == state1);

            fsm.RemoveState(1);

            Assert.IsTrue(fsm.StateCount == 0 && fsm.ContainsState(1) == false);
        }

        [Test]
        public void Throw_An_Exception_If_User_Adds_State_With_Existing_Id()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            FSM<int, int> fsm = new FSM<int, int>();

            Assert.Throws<StateIdAlreadyAddedException>(
                () =>
                {
                    fsm.AddState(1, state1);
                    fsm.AddState(1, state2);
                });
        }

        [Test]
        public void Throw_An_Exception_If_User_Adds_A_Null_State()
        {
            FSM<int, int> fsm = new FSM<int, int>();

            Assert.Throws<ArgumentNullException>(() => fsm.AddState(1, null));
        }

        [Test]
        public void Remove_States()
        {
            var state = Substitute.For<IState>();

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddState(1, state);

            Assert.IsTrue(fsm.StateCount == 1);

            fsm.RemoveState(1);

            Assert.IsTrue(fsm.StateCount == 0);
        }

        [Test]
        public void Add_And_Remove_Transitions()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var transition = new Transition<int, int>(1, 0, 2);

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.AddTransition(transition);

            Assert.IsTrue(fsm.TransitionCount == 1 && fsm.ContainsTransition(transition));

            fsm.RemoveTransition(transition);

            Assert.IsTrue(fsm.TransitionCount == 0 && fsm.ContainsTransition(transition) == false);
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Add_Or_Remove_Transition_With_No_Added_States()
        {
            var transition1 = new Transition<int, int>(1, 2, 3);

            var fsm = new FSM<int, int>();

            Assert.Throws<StateIdNotAddedException>(() => fsm.AddTransition(transition1));

            Assert.Throws<StateIdNotAddedException>(() => fsm.RemoveTransition(transition1));
        }

        [Test]
        public void Remove_Transitions()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var transition = new Transition<int, int>(1, 2, 3);

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(3, state2);

            fsm.AddTransition(transition);

            Assert.IsTrue(fsm.TransitionCount == 1);

            fsm.RemoveTransition(transition);

            Assert.IsTrue(fsm.TransitionCount == 0);
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Start_With_At_Least_One_State_And_Does_Not_Contains_The_Initial_State()
        {
            var fsm = new FSM<int, int>();

            IState state = Substitute.For<IState>();

            fsm.AddState(1, state);

            Assert.Throws<InvalidInitialStateException>(() => fsm.Start());
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Start_And_It_Is_Already_Started()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.InitialState = 1;

            fsm.Start();

            Assert.Throws<StateMachineStartedException>(() => fsm.Start());
        }

        [Test]
        public void Show_Corresponding_Value_When_Asked_If_Is_Started()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.InitialState = 1;

            Assert.IsFalse(fsm.IsStarted);

            fsm.Start();

            Assert.IsTrue(fsm.IsStarted);
        }

        [Test]
        public void Enter_Initial_Start_When_Started()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.InitialState = 1;

            fsm.Start();

            state1.Received().Enter();
        }

        [Test]
        public void Returns_Corresponding_Value_When_Asked_Is_In_State()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.InitialState = 1;

            fsm.Start();

            Assert.IsTrue(fsm.CurrentState == 1);
        }

        [Test]
        public void Change_State_When_Triggering_An_Existing_Transition()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.InitialState = 1;

            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.Start();

            Assert.IsTrue(fsm.CurrentState == 1);

            fsm.Trigger(0);

            Assert.IsTrue(fsm.CurrentState == 2);
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Trigger_When_Is_Not_Started()
        {
            var fsm = new FSM<int, int>();

            Assert.Throws<StateMachineNotStartedException>(() => fsm.Trigger(0));
        }

        [Test]
        public void Return_Corresponding_Value_When_Asked_If_Contains_Transition()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(3, state2);

            var transition1 = new Transition<int, int>(1, 2, 3);
            var transition2 = new Transition<int, int>(4, 5, 6);

            fsm.AddTransition(transition1);

            Assert.IsTrue(fsm.ContainsTransition(transition1));
            Assert.IsFalse(fsm.ContainsTransition(transition2));
        }

        [Test]
        public void Return_Corresponding_Value_When_Asked_If_Contains_State()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            Assert.IsTrue(fsm.ContainsState(1));
            Assert.IsFalse(fsm.ContainsState(2));
        }

        [Test]
        public void Exit_Current_State_When_Stopped()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.InitialState = 1;

            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.Start();

            fsm.Trigger(0);

            fsm.Stop();

            state2.Received().Exit();
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Update_And_It_Is_Not_Started()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.InitialState = 1;

            Assert.Throws<StateMachineNotStartedException>(fsm.Update);
        }

        [Test]
        public void Update_Current_State()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.InitialState = 1;

            fsm.Start();

            fsm.Update();

            state1.Received().Update();
        }

        [Test]
        public void Raise_State_Changed_Event_When_Has_Successfully_Transitioned()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var stateChangedEventHandler = Substitute.For<StateChanged<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.OnStateChanged += stateChangedEventHandler;

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.InitialState = 1;

            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.Start();

            fsm.Trigger(0);

            stateChangedEventHandler
                .Received(1)
                .Invoke(1, 0, 2);
        }

        [Test]
        public void Raise_Before_State_Changes_Event_When_Has_Successfully_Transitioned()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var stateChangedEventHandler = Substitute.For<StateChanged<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.OnBeforeStateChanges += stateChangedEventHandler;

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.InitialState = 1;

            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.Start();

            fsm.Trigger(0);

            stateChangedEventHandler
                .Received(1)
                .Invoke(1, 0, 2);
        }

        [Test]
        public void Add_And_Remove_Guard_Conditions()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            var transition = new Transition<int, int>(1, 0, 2);

            fsm.AddTransition(transition);

            fsm.AddGuardConditionTo(transition, TestGuardCondition);

            Assert.IsTrue(fsm.ContainsGuardConditionOn(transition, TestGuardCondition));

            fsm.RemoveGuardConditionFrom(transition, TestGuardCondition);

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

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            var transition = new Transition<int, int>(1, 0, 2);

            fsm.AddTransition(transition);

            Assert.Throws<ArgumentNullException>(() => fsm.AddGuardConditionTo(transition, null));

            Assert.Throws<ArgumentNullException>(() => fsm.RemoveGuardConditionFrom(transition, null));

        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Add_Or_Remove_Guard_Condition_On_A_Not_Added_Transition()
        {
            var fsm = new FSM<int, int>();

            var transition = new Transition<int, int>(1, 2, 3);

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

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            var transition = new Transition<int, int>(1, 0, 2);

            fsm.AddTransition(transition);

            Func<bool> guardCondition1 = Substitute.For<Func<bool>>();
            Func<bool> guardCondition2 = Substitute.For<Func<bool>>();

            guardCondition1.Invoke().Returns(true);
            guardCondition2.Invoke().Returns(true);

            fsm.AddGuardConditionTo(transition, guardCondition1);

            fsm.AddGuardConditionTo(transition, guardCondition2);

            fsm.InitialState = 1;

            fsm.Start();

            fsm.Trigger(0);

            guardCondition1.Received().Invoke();
            guardCondition2.Received().Invoke();

            Assert.IsTrue(fsm.CurrentState == 2);

        }

        [Test]
        public void Not_Transition_If_Any_Guard_Condition_Returns_False()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            var transition = new Transition<int, int>(1, 0, 2);

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

            fsm.InitialState = 1;

            fsm.Start();

            fsm.Trigger(0);

            guardCondition1.Received().Invoke();
            guardCondition2.Received().Invoke();
            guardCondition3.DidNotReceive().Invoke();

            Assert.IsFalse(fsm.CurrentState == 2);
        }

        [Test]
        public void Reenter_State_When_State_To_Is_Equal_To_State_From()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.AddTransition(new Transition<int, int>(1, 0, 1));

            fsm.InitialState = 1;

            fsm.Start();

            fsm.Trigger(0);

            state1.Received().Exit();
            state1.Received(2).Enter();
        }

        [Test]
        public void Remove_Transitions_Related_To_A_State_Id_When_It_Is_Removed()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            var transition1 = new Transition<int, int>(1, 0, 1);
            var transition2 = new Transition<int, int>(1, 0, 2);
            var transition3 = new Transition<int, int>(2, 0, 1);

            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);
            fsm.AddTransition(transition3);

            fsm.RemoveState(2);

            Assert.IsFalse(fsm.ContainsTransition(transition2) && fsm.ContainsTransition(transition3));
            Assert.IsTrue(fsm.ContainsTransition(transition1));
        }

        [Test]
        public void Remove_Guard_Conditions_Related_To_A_Transition_When_It_Is_Removed()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            var transition = new Transition<int, int>(1, 0, 2);

            fsm.AddTransition(transition);

            fsm.AddGuardConditionTo(transition, TestGuardCondition);
            fsm.AddGuardConditionTo(transition, TestGuardCondition2);

            fsm.RemoveState(1);

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
            var fsm = new FSM<int, int>();

            var transition1 = new Transition<int, int>(1, 0, 2);
            var transition2 = new Transition<int, int>(2, 0, 3);
            var transition3 = new Transition<int, int>(3, 0, 4);
            var transition4 = new Transition<int, int>(4, 0, 5);

            var state1 = Substitute.For<IState>();
            var state3 = Substitute.For<IState>();
            var state4 = Substitute.For<IState>();
            var state5 = Substitute.For<IState>();
            var state2 = new DelegateState<int, int>
                (
                    () =>
                    {
                        fsm.Trigger(transition2.Trigger);
                        fsm.Trigger(transition3.Trigger);
                        fsm.Trigger(transition4.Trigger);
                        state3.DidNotReceive().Enter();
                    },
                    null, null);


            fsm.AddState(1, state1);
            fsm.AddState(2, state2);
            fsm.AddState(3, state3);
            fsm.AddState(4, state4);
            fsm.AddState(5, state5);
            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);
            fsm.AddTransition(transition3);
            fsm.AddTransition(transition4);
            fsm.InitialState = 1;
            fsm.Start();

            fsm.Trigger(transition1.Trigger);

            Assert.IsTrue(fsm.CurrentState == 5);
        }

        [Test]
        public void Throw_An_Exception_When_User_Tries_To_Transition_And_Guard_Conditions_Are_Not_Mutually_Exclusive()
        {
            var fsm = new FSM<int, int>();

            fsm.AddEmpty(1);
            fsm.AddEmpty(2);
            fsm.AddEmpty(3);

            var transition1 = new Transition<int, int>(1, 0, 1);
            var transition2 = new Transition<int, int>(1, 0, 2);
            var transition3 = new Transition<int, int>(1, 1, 2);
            var transition4 = new Transition<int, int>(1, 1, 3);

            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);
            fsm.AddTransition(transition3);
            fsm.AddTransition(transition4);

            fsm.AddGuardConditionTo(transition1, () => true);
            fsm.AddGuardConditionTo(transition2, () => false);
            fsm.AddGuardConditionTo(transition3, () => true);
            fsm.AddGuardConditionTo(transition4, () => true);

            fsm.InitialState = 1;

            fsm.Start();

            Assert.DoesNotThrow(() => fsm.Trigger(0));
            Assert.Throws<MultipleValidTransitionsFromSameStateException>(() => fsm.Trigger(1));
        }

        [Test]
        public void Throw_An_Exception_When_User_Tries_To_Transition_And_Multiple_Transitions_With_Same_Source_And_Trigger_Has_No_Guard_Conditions()
        {
            var fsm = new FSM<int, int>();

            fsm.AddEmpty(1);
            fsm.AddEmpty(2);
            fsm.AddEmpty(3);

            var transition1 = new Transition<int, int>(1, 0, 1);
            var transition2 = new Transition<int, int>(1, 0, 2);

            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);

            fsm.InitialState = 1;

            fsm.Start();

            Assert.Throws<MultipleValidTransitionsFromSameStateException>(() => fsm.Trigger(0));
        }

        [Test]
        public void Let_Transition_On_First_Enter()
        {
            var fsm = new FSM<int, int>();

            Action onEnter1 = () => fsm.Trigger(0);
            Action onEnter2Substitute = Substitute.For<Action>();
            Action onEnter2 = onEnter2Substitute + (() => fsm.Trigger(0));
            Action onEnter3 = Substitute.For<Action>();

            fsm.AddWithEvents(1, onEnter1);
            fsm.AddWithEvents(2, onEnter2);
            fsm.AddWithEvents(3, onEnter3);

            var transition1 = new Transition<int, int>(1, 0, 2);
            var transition2 = new Transition<int, int>(2, 0, 3);

            fsm.AddTransition(transition1);
            fsm.AddTransition(transition2);

            fsm.InitialState = 1;

            fsm.Start();

            onEnter2Substitute.Received();
            onEnter3.Received();

            Assert.IsTrue(fsm.CurrentState == 3);
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Start_On_First_Enter()
        {
            var fsm = new FSM<int, int>();

            Action onEnter = () => fsm.Start();

            fsm.AddWithEvents(1, onEnter);

            fsm.InitialState = 1;

            Assert.Throws<StateMachineStartedException>(() => fsm.Start());
        }

        [Test]
        public void Let_User_Stop_It_On_First_Enter()
        {
            var fsm = new FSM<int, int>();

            Action onEnter = () => fsm.Stop();
            Action onExit = Substitute.For<Action>();

            fsm.AddWithEvents(1, onEnter, onExit);

            fsm.InitialState = 1;

            fsm.Start();

            Assert.IsFalse(fsm.IsStarted);

            onExit.Received();
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Transition_On_Exit_Of_State_When_Stopped()
        {
            var fsm = new FSM<int, int>();

            Action onEnter = () => fsm.Stop();
            Action onExit = () => fsm.Trigger(0);

            fsm.AddWithEvents(1, onEnter, onExit);

            fsm.InitialState = 1;

            Assert.Throws<StateMachineStoppingException>(() => fsm.Start());
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Send_An_Event_And_Is_Not_Started()
        {
            var fsm = new FSM<int, int>();

            Assert.Throws<StateMachineNotStartedException>(() => fsm.SendEvent(Substitute.For<IEvent>()));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Comparer_Is_Null()
        {
            FSM<int, int> fsm = null;

            IEqualityComparer<int> comparer = null;

            Assert.Throws<ArgumentNullException>(() => new FSM<int, int>(comparer, comparer));

            fsm = new FSM<int, int>();

            Assert.Throws<ArgumentNullException>(() => fsm.SetStateComparer(comparer));
            Assert.Throws<ArgumentNullException>(() => fsm.SetTriggerComparer(comparer));
        }

        [Test]
        public void Use_Custom_Equality_Comparer()
        {
            IEqualityComparer<int> comparer = Substitute.For<IEqualityComparer<int>>();

            comparer.Equals(1, 1).Returns(true);
            comparer.Equals(2, 2).Returns(true);
            comparer.Equals(1, 2).Returns(false);
            comparer.Equals(2, 1).Returns(false);
            comparer.Equals(0, 0).Returns(true);
            comparer.Equals(1, 0).Returns(false);
            comparer.Equals(2, 0).Returns(false);
            comparer.Equals(0, 1).Returns(false);
            comparer.Equals(0, 2).Returns(false);

            var fsm = new FSM<int, int>(comparer, comparer);

            fsm.AddEmpty(1);
            fsm.AddEmpty(2);

            fsm.InitialState = 1;

            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.Start();

            fsm.Trigger(0);

            comparer.Received().Equals(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void Return_State_By_Id()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            Assert.AreEqual(fsm.GetStateById(1), state1);
            Assert.AreEqual(fsm.GetStateById(2), state2);
        }

        [Test]
        public void Change_Equality_Comparer_After_Construction()
        {
            IEqualityComparer<int> comparer = Substitute.For<IEqualityComparer<int>>();

            comparer.Equals(1, 1).Returns(true);
            comparer.Equals(2, 2).Returns(true);
            comparer.Equals(1, 2).Returns(false);
            comparer.Equals(2, 1).Returns(false);
            comparer.Equals(0, 0).Returns(true);
            comparer.Equals(1, 0).Returns(false);
            comparer.Equals(2, 0).Returns(false);
            comparer.Equals(0, 1).Returns(false);
            comparer.Equals(0, 2).Returns(false);

            var fsm = new FSM<int, int>();

            fsm.SetStateComparer(comparer);
            fsm.SetTriggerComparer(comparer);

            fsm.AddEmpty(1);
            fsm.AddEmpty(2);

            fsm.InitialState = 1;

            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.Start();

            fsm.Trigger(0);

            comparer.Received().Equals(Arg.Any<int>(), Arg.Any<int>());
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Get_Current_State_While_Not_Started()
        {
            var fsm = new FSM<int, int>();

            Assert.Throws<StateMachineNotStartedException>(() => fsm.CurrentState.ToString());
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Remove_Current_State()
        {
            var fsm = new FSM<int, int>();

            fsm.AddEmpty(1);

            fsm.InitialState = 1;

            fsm.Start();

            Assert.Throws<InvalidOperationException>(() => fsm.RemoveState(1));
        }

        [Test]
        public void Throw_An_Exception_If_Is_Empty_When_Started()
        {
            var fsm = new FSM<int, int>();

            Assert.Throws<EmptyStateMachineException>(() => fsm.Start());
        }
    }
}
