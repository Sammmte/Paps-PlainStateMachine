using NSubstitute;
using NUnit.Framework;
using Paps.FSM;
using Paps.FSM.Extensions;
using System;

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

            Assert.IsTrue(fsm.StateCount == 1 && fsm.ContainsState(1) && fsm.GetIdOf(state1) == 1);

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
                        item1 = state;
                    }
                    else
                    {
                        item2 = state;
                    }

                    cont++;

                    return false;
                }
                );

            Assert.IsTrue(fsm.GetIdOf(item1) == 1 && fsm.GetIdOf(item2) == 2);
        }

        [Test]
        public void RemoveStates()
        {
            var state = Substitute.For<IState>();

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddState(1, state);

            Assert.IsTrue(fsm.StateCount == 1);

            fsm.RemoveState(fsm.GetIdOf(state));

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

            fsm.AddTransitionWithValuesOf(transition);

            Assert.IsTrue(fsm.TransitionCount == 1 && fsm.ContainsTransition(1, 0, 2));

            fsm.RemoveTransitionWithValuesOf(transition);

            Assert.IsTrue(fsm.TransitionCount == 0 && fsm.ContainsTransition(1, 0, 2) == false);
        }

        [Test]
        public void ThrowAnExceptionIfUserTriesToAddOrRemoveTransitionWithNoAddedStates()
        {
            var transition1 = new Transition<int, int>(1, 2, 3);

            var fsm = new FSM<int, int>();

            Assert.Throws<StateIdNotAddedException>(() => fsm.AddTransitionWithValuesOf(transition1));

            Assert.Throws<StateIdNotAddedException>(() => fsm.RemoveTransitionWithValuesOf(transition1));
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

            fsm.AddTransition(transition1.StateFrom, transition1.Trigger, transition1.StateTo);
            fsm.AddTransition(transition2.StateFrom, transition2.Trigger, transition2.StateTo);

            ITransition<int, int> item1 = default;
            ITransition<int, int> item2 = default;

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

            fsm.AddTransition(transition.StateFrom, transition.Trigger, transition.StateTo);

            Assert.IsTrue(fsm.TransitionCount == 1);

            fsm.RemoveTransition(transition.StateFrom, transition.Trigger, transition.StateTo);

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

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.Start();

            Assert.Throws<FSMStartedException>(() => fsm.Start());
        }

        [Test]
        public void ShowCorrespondingValueWhenAskedIfIsStarted()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.SetInitialState(fsm.GetIdOf(state1));

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

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.Start();

            state1.Received().Enter();
        }

        [Test]
        public void ReturnsCorrespondingValueWhenAskedIsInState()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.Start();

            Assert.IsTrue(fsm.IsInState(fsm.GetIdOf(state1)));
        }

        [Test]
        public void ChangeStateWhenTriggeringAnExistingTransition()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.AddTransition(fsm.GetIdOf(state1), 0, fsm.GetIdOf(state2));

            fsm.Start();

            Assert.IsTrue(fsm.IsInState(fsm.GetIdOf(state1)));

            fsm.Trigger(0);

            Assert.IsTrue(fsm.IsInState(fsm.GetIdOf(state2)));
        }

        [Test]
        public void ThrowAnExceptionIfUserTriesToTriggerWhenFSMIsNotStarted()
        {
            var fsm = new FSM<int, int>();

            Assert.Throws<FSMNotStartedException>(() => fsm.Trigger(0));
        }

        [Test]
        public void ReturnCorrespondingValueWhenAskedIfContainsTransition()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(3, state2);

            fsm.AddTransition(1, 2, 3);

            Assert.IsTrue(fsm.ContainsTransition(1, 2, 3));
            Assert.IsFalse(fsm.ContainsTransition(4, 5, 6));
        }

        [Test]
        public void ReturnCorrespondingValueWhenAskedIfContainsState()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            Assert.IsTrue(fsm.ContainsState(fsm.GetIdOf(state1)));
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

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.AddTransition(fsm.GetIdOf(state1), 0, fsm.GetIdOf(state2));

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

            fsm.SetInitialState(fsm.GetIdOf(state1));

            Assert.Throws<FSMNotStartedException>(fsm.Update);
        }

        [Test]
        public void UpdateCurrentState()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.SetInitialState(fsm.GetIdOf(state1));

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

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.AddTransition(fsm.GetIdOf(state1), 0, fsm.GetIdOf(state2));

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

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.AddTransition(fsm.GetIdOf(state1), 0, fsm.GetIdOf(state2));

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

            fsm.AddTransition(1, 0, 2);

            fsm.AddGuardConditionTo(1, 0, 2, TestGuardCondition);

            Assert.IsTrue(fsm.ContainsGuardConditionOn(1, 0, 2, TestGuardCondition));

            fsm.RemoveGuardConditionFrom(1, 0, 2, TestGuardCondition);

            Assert.IsFalse(fsm.ContainsGuardConditionOn(1, 0, 2, TestGuardCondition));

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

            fsm.AddTransition(1, 0, 2);

            Assert.Throws<ArgumentNullException>(() => fsm.AddGuardConditionTo(1, 0, 2, null));

            Assert.Throws<ArgumentNullException>(() => fsm.RemoveGuardConditionFrom(1, 0, 2, null));

        }

        [Test]
        public void ThrowAnExceptionIfUserTriesToAddOrRemoveGuardConditionOnANotAddedTransition()
        {
            var fsm = new FSM<int, int>();

            Assert.Throws<TransitionNotAddedException>(() => fsm.AddGuardConditionTo(1, 2, 3, TestGuardCondition));

            Assert.Throws<TransitionNotAddedException>(() => fsm.RemoveGuardConditionFrom(1, 2, 3, TestGuardCondition));

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

            fsm.AddTransition(1, 0, 2);

            Func<bool> guardCondition1 = Substitute.For<Func<bool>>();
            Func<bool> guardCondition2 = Substitute.For<Func<bool>>();

            guardCondition1.Invoke().Returns(true);
            guardCondition2.Invoke().Returns(true);

            fsm.AddGuardConditionTo(1, 0, 2, guardCondition1);

            fsm.AddGuardConditionTo(1, 0, 2, guardCondition2);

            fsm.SetInitialState(1);

            fsm.Start();

            fsm.Trigger(0);

            guardCondition1.Received().Invoke();
            guardCondition2.Received().Invoke();

            Assert.IsTrue(fsm.IsInState(2));

        }

        [Test]
        public void NotTransitionIfAnyANDGuardConditionReturnsFalse()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.AddTransition(1, 0, 2);

            Func<bool> guardCondition1 = Substitute.For<Func<bool>>();
            Func<bool> guardCondition2 = Substitute.For<Func<bool>>();
            Func<bool> guardCondition3 = Substitute.For<Func<bool>>();

            guardCondition1.Invoke().Returns(true);
            guardCondition2.Invoke().Returns(false);
            guardCondition3.Invoke().Returns(true);

            fsm.AddGuardConditionTo(1, 0, 2, guardCondition1);

            fsm.AddGuardConditionTo(1, 0, 2, guardCondition2);

            fsm.AddGuardConditionTo(1, 0, 2, guardCondition3);

            fsm.SetInitialState(1);

            fsm.Start();

            fsm.Trigger(0);

            guardCondition1.Received().Invoke();
            guardCondition2.Received().Invoke();
            guardCondition3.DidNotReceive().Invoke();

            Assert.IsFalse(fsm.IsInState(2));
        }

        [Test]
        public void ReenterStateWhenStateToIsEqualToStateFrom()
        {
            var state1 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.AddTransition(1, 0, 1);

            fsm.SetInitialState(1);

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

            fsm.AddTransition(1, 0, 1);
            fsm.AddTransition(1, 0, 2);
            fsm.AddTransition(2, 0, 1);

            fsm.RemoveState(2);

            Assert.IsFalse(fsm.ContainsTransition(1, 0, 2) && fsm.ContainsTransition(2, 0, 1));
        }

        [Test]
        public void RemoveGuardConditionsRelatedToATransitionWhenItIsRemoved()
        {
            var state1 = Substitute.For<IState>();
            var state2 = Substitute.For<IState>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.AddTransition(1, 0, 2);

            fsm.AddGuardConditionTo(1, 0, 2, TestGuardCondition);
            fsm.AddGuardConditionTo(1, 0, 2, TestGuardCondition2);

            fsm.RemoveState(1);

            Assert.IsFalse(fsm.ContainsGuardConditionOn(1, 0, 2, TestGuardCondition));
            Assert.IsFalse(fsm.ContainsGuardConditionOn(1, 0, 2, TestGuardCondition2));

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
            fsm.AddTransitionWithValuesOf(transition1);
            fsm.AddTransitionWithValuesOf(transition2);
            fsm.AddTransitionWithValuesOf(transition3);
            fsm.AddTransitionWithValuesOf(transition4);
            fsm.SetInitialState(1);
            fsm.Start();

            fsm.Trigger(transition1.Trigger);

            Assert.IsTrue(fsm.IsInState(5));
        }

        [Test]
        public void ThrowAnExceptionWhenUserTriesToTransitionAndGuardConditionsAreNotMutuallyExclusive()
        {
            var fsm = new FSM<int, int>();
            
            fsm.AddEmpty(1);
            fsm.AddEmpty(2);
            fsm.AddEmpty(3);

            fsm.AddTransition(1, 0, 1);
            fsm.AddTransition(1, 0, 2);
            fsm.AddTransition(1, 1, 2);
            fsm.AddTransition(1, 1, 3);

            fsm.AddGuardConditionTo(1, 0, 1, () => true);
            fsm.AddGuardConditionTo(1, 0, 2, () => false);

            fsm.AddGuardConditionTo(1, 1, 2, () => true);
            fsm.AddGuardConditionTo(1, 1, 3, () => true);

            fsm.SetInitialState(1);

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

            fsm.AddTransition(1, 0, 1);
            fsm.AddTransition(1, 0, 2);

            fsm.SetInitialState(1);

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

            fsm.AddTransition(1, 0, 2);
            fsm.AddTransition(2, 0, 3);

            fsm.SetInitialState(1);

            fsm.Start();

            onEnter2Substitute.Received();
            onEnter3.Received();

            Assert.IsTrue(fsm.IsInState(3));
        }

        [Test]
        public void ThrowAnExceptionIfUserTriesToStartOnFirstEnter()
        {
            var fsm = new FSM<int, int>();

            Action onEnter = () => fsm.Start();

            fsm.AddWithEvents(1, onEnter);

            fsm.SetInitialState(1);

            Assert.Throws<FSMStartedException>(() => fsm.Start());
        }

        [Test]
        public void LetUserStopItOnFirstEnter()
        {
            var fsm = new FSM<int, int>();

            Action onEnter = () => fsm.Stop();
            Action onExit = Substitute.For<Action>();

            fsm.AddWithEvents(1, onEnter, onExit);

            fsm.SetInitialState(1);

            fsm.Start();

            Assert.IsFalse(fsm.IsStarted);

            onExit.Received();
        }

        [Test]
        public void BeStoppedWhenCallingTheExitOfStateAfterUserRequestedToStop()
        {
            var fsm = new FSM<int, int>();

            Action onEnter = () => fsm.Stop();
            Action onExit = () => Assert.IsFalse(fsm.IsStarted);

            fsm.AddWithEvents(1, onEnter, onExit);

            fsm.SetInitialState(1);

            fsm.Start();
        }

        [Test]
        public void ThrowAnExceptionIfUserTriesToTransitionOnExitOfStateWhenStopped()
        {
            var fsm = new FSM<int, int>();

            Action onEnter = () => fsm.Stop();
            Action onExit = () => fsm.Trigger(0);

            fsm.AddWithEvents(1, onEnter, onExit);

            fsm.SetInitialState(1);

            Assert.Throws<FSMNotStartedException>(() => fsm.Start());
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

            fsm.AddTransition(1, 0, 2);

            fsm.SetInitialState(1);

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

            Assert.Throws<FSMNotStartedException>(() => fsm.SendEvent(Substitute.For<IEvent>()));
        }
    }
}
