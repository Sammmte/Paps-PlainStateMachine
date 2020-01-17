using NSubstitute;
using NUnit.Framework;
using Paps.FSM;
using Paps.FSM.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests
{
    public class FSMShould
    {
        [Test]
        public void AddAndRemoveStates()
        {
            var state1 = Substitute.For<IState>();

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            Assert.IsTrue(fsm.StateCount == 1 && fsm.ContainsState(1) && fsm.GetStateById(1) == state1);

            fsm.RemoveState(1);

            Assert.IsTrue(fsm.StateCount == 0 && fsm.ContainsState(1) == false);
        }

        [Test]
        public void ThrowAnExceptionIfUserAddsStateWithExistingId()
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
        public void ThrowAnExceptionIfUserAddsANullState()
        {
            FSM<int, int> fsm = new FSM<int, int>();

            Assert.Throws<ArgumentNullException>(() => fsm.AddState(1, null));
        }

        [Test]
        public void IterateOverStates()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            IState item1 = null;
            IState item2 = null;

            FSM<int, int> fsm = new FSM<int, int>();

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

        [Test]
        public void RemoveStates()
        {
            var state = Substitute.For<IState>();

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddState(1, state);

            Assert.IsTrue(fsm.StateCount == 1);

            fsm.RemoveState(1);

            Assert.IsTrue(fsm.StateCount == 0);
        }

        [Test]
        public void AddAndRemoveTransitions()
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
        public void ThrowAnExceptionIfUserTriesToAddOrRemoveTransitionWithNoAddedStates()
        {
            var transition1 = new Transition<int, int>(1, 2, 3);

            var fsm = new FSM<int, int>();

            Assert.Throws<StateIdNotAddedException>(() => fsm.AddTransition(transition1));

            Assert.Throws<StateIdNotAddedException>(() => fsm.RemoveTransition(transition1));
        }

        [Test]
        public void IterateOverTransitions()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();
            var state3 = Substitute.For<IState>();
            var state4 = Substitute.For<IState>();

            var transition1 = new Transition<int, int>(1, 2, 3);
            var transition2 = new Transition<int, int>(4, 5, 6);

            FSM<int, int> fsm = new FSM<int, int>();

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
        public void RemoveTransitions()
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
        public void ThrowAnExceptionIfUserTriesToStartAndInitialStateIsNotSet()
        {
            var fsm = new FSM<int, int>();

            Assert.Throws<InvalidInitialStateException>(() => fsm.Start());
        }

        [Test]
        public void ThrowAnExceptionIfUserTriesToStartAndItsAlreadyStarted()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.InitialState = 1;

            fsm.Start();

            Assert.Throws<StateMachineStartedException>(() => fsm.Start());
        }

        [Test]
        public void ShowCorrespondingValueWhenAskedIfIsStarted()
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
        public void EnterInitialStartWhenStarted()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.InitialState = 1;

            fsm.Start();

            state1.Received().Enter();
        }

        [Test]
        public void ReturnsCorrespondingValueWhenAskedIsInState()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.InitialState = 1;

            fsm.Start();

            Assert.IsTrue(fsm.CurrentState == 1);
        }

        [Test]
        public void ChangeStateWhenTriggeringAnExistingTransition()
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
        public void ThrowAnExceptionIfUserTriesToTriggerWhenFSMIsNotStarted()
        {
            var fsm = new FSM<int, int>();

            Assert.Throws<StateMachineNotStartedException>(() => fsm.Trigger(0));
        }

        [Test]
        public void ReturnCorrespondingValueWhenAskedIfContainsTransition()
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
        public void ReturnCorrespondingValueWhenAskedIfContainsState()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            Assert.IsTrue(fsm.ContainsState(1));
            Assert.IsFalse(fsm.ContainsState(2));
        }

        [Test]
        public void ExitCurrentStateWhenStopped()
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
        public void ThrowAnExceptionIfUserTriesToUpdateAndItIsNotStarted()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.InitialState = 1;

            Assert.Throws<StateMachineNotStartedException>(fsm.Update);
        }

        [Test]
        public void UpdateCurrentState()
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
        public void RaiseStateChangedEventWhenHasSuccessfullyTransitioned()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var stateChangedEventHandler = Substitute.For<StateChange<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.OnStateChanged += stateChangedEventHandler;

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.InitialState = 1;

            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.Start();

            fsm.Trigger(0);

            stateChangedEventHandler
                .Received()
                .Invoke(fsm);
        }

        [Test]
        public void RaiseBeforeStateChangesEventWhenHasSuccessfullyTransitioned()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var stateChangedEventHandler = Substitute.For<StateChange<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.OnBeforeStateChanges += stateChangedEventHandler;

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.InitialState = 1;

            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.Start();

            fsm.Trigger(0);

            stateChangedEventHandler
                .Received()
                .Invoke(fsm);
        }

        [Test]
        public void AddAndRemoveGuardConditions()
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
        public void ThrowAnExceptionIfUserTriesToAddOrRemoveANullGuardCondition()
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
        public void ThrowAnExceptionIfUserTriesToAddOrRemoveGuardConditionOnANotAddedTransition()
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
        public void TransitionIfAllGuardConditionsReturnTrue()
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
        public void NotTransitionIfAnyGuardConditionReturnsFalse()
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
        public void ReenterStateWhenStateToIsEqualToStateFrom()
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
        public void RemoveTransitionsRelatedToAStateIdWhenItIsRemoved()
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
        public void RemoveGuardConditionsRelatedToATransitionWhenItIsRemoved()
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
        public void TransitionQueued()
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
        public void ThrowAnExceptionWhenUserTriesToTransitionAndGuardConditionsAreNotMutuallyExclusive()
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
        public void ThrowAnExceptionWhenUserTriesToTransitionAndMultipleTransitionsWithSameSourceAndTriggerHasNoGuardConditions()
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
        public void LetTransitionOnFirstEnter()
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
        public void ThrowAnExceptionIfUserTriesToStartOnFirstEnter()
        {
            var fsm = new FSM<int, int>();

            Action onEnter = () => fsm.Start();

            fsm.AddWithEvents(1, onEnter);

            fsm.InitialState = 1;

            Assert.Throws<StateMachineStartedException>(() => fsm.Start());
        }

        [Test]
        public void LetUserStopItOnFirstEnter()
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
        public void ThrowAnExceptionIfUserTriesToTransitionOnExitOfStateWhenStopped()
        {
            var fsm = new FSM<int, int>();

            Action onEnter = () => fsm.Stop();
            Action onExit = () => fsm.Trigger(0);

            fsm.AddWithEvents(1, onEnter, onExit);

            fsm.InitialState = 1;

            Assert.Throws<StateMachineExitingException>(() => fsm.Start());
        }

        [Test]
        public void SendEventToEventReceivers()
        {
            var fsm = new FSM<int, int>();

            IState state1 = Substitute.For<IState>();
            IState state2 = Substitute.For<IState>();

            IEvent stateEvent = Substitute.For<IEvent>();

            stateEvent.GetEventData().Returns(10);

            state1.HandleEvent(stateEvent).Returns(true);

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.AddTransition(new Transition<int, int>(1, 0, 2));

            fsm.InitialState = 1;

            fsm.Start();

            Assert.IsTrue(fsm.SendEvent(stateEvent));

            state1.Received().HandleEvent(stateEvent);

            fsm.Trigger(0);

            Assert.IsFalse(fsm.SendEvent(stateEvent));

            state2.Received().HandleEvent(stateEvent);
        }

        [Test]
        public void ThrowAnExceptionIfUserTriesToSendAnEventAndStateMachineIsNotStarted()
        {
            var fsm = new FSM<int, int>();

            Assert.Throws<StateMachineNotStartedException>(() => fsm.SendEvent(Substitute.For<IEvent>()));
        }

        [Test]
        public void ThrowAnExceptionIfUserTriesToComparerIsNull()
        {
            FSM<int, int> fsm = null;

            IEqualityComparer<int> comparer = null;

            Assert.Throws<ArgumentNullException>(() => new FSM<int, int>(comparer, comparer));

            fsm = new FSM<int, int>();

            Assert.Throws<ArgumentNullException>(() => fsm.SetStateComparer(comparer));
            Assert.Throws<ArgumentNullException>(() => fsm.SetTriggerComparer(comparer));
        }

        [Test]
        public void UseCustomEqualityComparer()
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
        public void ReturnStateById()
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
        public void ChangeEqualityComparerAfterConstruction()
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
        public void ThrowAnExceptionIfUserTriesToGetCurrentStateWhileNotStarted()
        {
            var fsm = new FSM<int, int>();

            Assert.Throws<StateMachineNotStartedException>(() => fsm.CurrentState.ToString());
        }

        [Test]
        public void ThrowAnExceptionIfUserTriesToRemoveCurrentState()
        {
            var fsm = new FSM<int, int>();

            fsm.AddEmpty(1);

            fsm.InitialState = 1;

            fsm.Start();

            Assert.Throws<InvalidOperationException>(() => fsm.RemoveState(1));
        }

        [Test]
        public void PreventSideEffectsIfAnExceptionIsThrownWhenEntering()
        {
            var fsm = new FSM<int, int>();

            IState state1 = Substitute.For<IState>();
            
            fsm.AddState(1, state1);
            
            state1.When(state => state.Enter())
                .Do(callbackInfo => throw new Exception());

            fsm.InitialState = 1;

            Assert.Throws<Exception>(fsm.Start);
            Assert.IsFalse(fsm.IsStarted);
        }

        [Test]
        public void PreventSideEffectsIfAnExceptionIsThrownWhenExiting()
        {
            var fsm = new FSM<int, int>();

            IState state1 = Substitute.For<IState>();
            
            fsm.AddState(1, state1);
            
            state1.When(state => state.Exit())
                .Do(callbackInfo => throw new Exception());

            fsm.InitialState = 1;

            fsm.Start();

            Assert.Throws<Exception>(fsm.Stop);
            Assert.IsTrue(fsm.IsStarted);
        }
    }
}
