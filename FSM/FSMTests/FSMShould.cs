using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paps.FSM;
using NSubstitute;
using Paps.FSM.Extensions;

namespace FSMTests
{
    [TestClass]
    public class FSMShould
    {
        [TestMethod]
        public void AddAndRemoveStates()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            Assert.IsTrue(fsm.StateCount == 1 && fsm.ContainsState(1) && fsm.GetIdOf(state1) == 1);

            fsm.RemoveState(1);

            Assert.IsTrue(fsm.StateCount == 0 && fsm.ContainsState(1) == false);
        }

        [TestMethod]
        public void ThrowAnExceptionIfUserAddsStateWithExistingId()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            FSM<int, int> fsm = new FSM<int, int>();

            Assert.ThrowsException<StateIdAlreadyAddedException>(
                () =>
                {
                    fsm.AddState(1, state1);
                    fsm.AddState(1, state2);
                });
        }

        [TestMethod]
        public void ThrowAnExceptionIfUserAddsANullState()
        {
            FSM<int, int> fsm = new FSM<int, int>();

            Assert.ThrowsException<ArgumentNullException>(() => fsm.AddState(1, null));
        }

        [TestMethod]
        public void IterateOverStates()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            IFSMState<int, int> item1 = null;
            IFSMState<int, int> item2 = null;

            FSM<int, int> fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);
            state2.StateMachine.Returns(fsm);

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

        [TestMethod]
        public void RemoveStates()
        {
            var state = Substitute.For<IFSMState<int, int>>();

            FSM<int, int> fsm = new FSM<int, int>();

            state.StateMachine.Returns(fsm);

            fsm.AddState(1, state);

            Assert.IsTrue(fsm.StateCount == 1);

            fsm.RemoveState(fsm.GetIdOf(state));

            Assert.IsTrue(fsm.StateCount == 0);
        }

        [TestMethod]
        public void AddAndRemoveTransitions()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var transition = new FSMTransition<int, int>(1, 0, 2);

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.AddTransitionWithValuesOf(transition);

            Assert.IsTrue(fsm.TransitionCount == 1 && fsm.ContainsTransition(1, 0, 2));

            fsm.RemoveTransitionWithValuesOf(transition);

            Assert.IsTrue(fsm.TransitionCount == 0 && fsm.ContainsTransition(1, 0, 2) == false);
        }

        [TestMethod]
        public void ThrowAnExceptionIfUserTriesToAddOrRemoveTransitionWithNoAddedStates()
        {
            var transition1 = new FSMTransition<int, int>(1, 2, 3);

            var fsm = new FSM<int, int>();

            Assert.ThrowsException<StateNotAddedException>(() => fsm.AddTransitionWithValuesOf(transition1));

            Assert.ThrowsException<StateNotAddedException>(() => fsm.RemoveTransitionWithValuesOf(transition1));
        }

        [TestMethod]
        public void IterateOverTransitions()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();
            var state3 = Substitute.For<IFSMState<int, int>>();
            var state4 = Substitute.For<IFSMState<int, int>>();

            var transition1 = new FSMTransition<int, int>(1, 2, 3);
            var transition2 = new FSMTransition<int, int>(4, 5, 6);

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(3, state2);

            fsm.AddState(4, state3);
            fsm.AddState(6, state4);

            fsm.AddTransition(transition1.StateFrom, transition1.Trigger, transition1.StateTo);
            fsm.AddTransition(transition2.StateFrom, transition2.Trigger, transition2.StateTo);

            IFSMTransition<int, int> item1 = default;
            IFSMTransition<int, int> item2 = default;

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

        [TestMethod]
        public void RemoveTransitions()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var transition = new FSMTransition<int, int>(1, 2, 3);

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(3, state2);

            fsm.AddTransition(transition.StateFrom, transition.Trigger, transition.StateTo);

            Assert.IsTrue(fsm.TransitionCount == 1);

            fsm.RemoveTransition(transition.StateFrom, transition.Trigger, transition.StateTo);

            Assert.IsTrue(fsm.TransitionCount == 0);
        }

        [TestMethod]
        public void ThrowAnExceptionIfUserTriesToStartAndInitialStateIsNotSet()
        {
            var fsm = new FSM<int, int>();

            Assert.ThrowsException<InvalidInitialStateException>(() => fsm.Start());
        }

        [TestMethod]
        public void ThrowAnExceptionIfUserTriesToStartAndItsAlreadyStarted()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.Start();

            Assert.ThrowsException<FSMStartedException>(() => fsm.Start());
        }

        [TestMethod]
        public void ShowCorrespondingValueWhenAskedIfIsStarted()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);

            fsm.SetInitialState(fsm.GetIdOf(state1));

            Assert.IsFalse(fsm.IsStarted);

            fsm.Start();

            Assert.IsTrue(fsm.IsStarted);
        }

        [TestMethod]
        public void EnterInitialStartWhenStarted()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.Start();

            state1.Received().Enter();
        }

        [TestMethod]
        public void ReturnsCorrespondingValueWhenAskedIsInState()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.Start();

            Assert.IsTrue(fsm.IsInState(fsm.GetIdOf(state1)));
        }

        [TestMethod]
        public void ChangeStateWhenTriggeringAnExistingTransition()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);
            state2.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.AddTransition(fsm.GetIdOf(state1), 0, fsm.GetIdOf(state2));

            fsm.Start();

            Assert.IsTrue(fsm.IsInState(fsm.GetIdOf(state1)));

            fsm.Trigger(0);

            Assert.IsTrue(fsm.IsInState(fsm.GetIdOf(state2)));
        }

        [TestMethod]
        public void ThrowAnExceptionIfUserTriesToTriggerWhenFSMIsNotStarted()
        {
            var fsm = new FSM<int, int>();

            Assert.ThrowsException<FSMNotStartedException>(() => fsm.Trigger(0));
        }

        [TestMethod]
        public void ReturnCorrespondingValueWhenAskedIfContainsTransition()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(3, state2);

            fsm.AddTransition(1, 2, 3);

            Assert.IsTrue(fsm.ContainsTransition(1, 2, 3));
            Assert.IsFalse(fsm.ContainsTransition(4, 5, 6));
        }

        [TestMethod]
        public void ReturnCorrespondingValueWhenAskedIfContainsState()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);

            Assert.IsTrue(fsm.ContainsState(fsm.GetIdOf(state1)));
            Assert.IsFalse(fsm.ContainsState(2));
        }

        [TestMethod]
        public void ExitCurrentStateWhenStopped()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);
            state2.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.AddTransition(fsm.GetIdOf(state1), 0, fsm.GetIdOf(state2));

            fsm.Start();

            fsm.Trigger(0);

            fsm.Stop();

            state2.Received().Exit();
        }

        [TestMethod]
        public void ThrowAnExceptionIfUserTriesToUpdateAndItIsNotStarted()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);

            fsm.SetInitialState(fsm.GetIdOf(state1));

            Assert.ThrowsException<FSMNotStartedException>(fsm.Update);
        }

        [TestMethod]
        public void UpdateCurrentState()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.Start();

            fsm.Update();

            state1.Received().Update();
        }

        [TestMethod]
        public void RaiseStateChangedEventWhenHasSuccessfullyTransitioned()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var stateChangedEventHandler = Substitute.For<StateChanged<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.OnStateChanged += stateChangedEventHandler;

            state1.StateMachine.Returns(fsm);
            state2.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.SetInitialState(fsm.GetIdOf(state1));

            fsm.AddTransition(fsm.GetIdOf(state1), 0, fsm.GetIdOf(state2));

            fsm.Start();

            fsm.Trigger(0);

            stateChangedEventHandler
                .Received()
                .Invoke(1, 0, 2);
        }

        [TestMethod]
        public void AddAndRemoveGuardConditions()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.AddTransition(1, 0, 2);

            fsm.AddANDGuardConditionTo(1, 0, 2, TestGuardCondition);

            fsm.AddORGuardConditionTo(1, 0, 2, TestGuardCondition);

            Assert.IsTrue(fsm.ContainsANDGuardConditionOn(1, 0, 2, TestGuardCondition));
            Assert.IsTrue(fsm.ContainsORGuardConditionOn(1, 0, 2, TestGuardCondition));

            fsm.RemoveANDGuardConditionFrom(1, 0, 2, TestGuardCondition);

            fsm.RemoveORGuardConditionFrom(1, 0, 2, TestGuardCondition);

            Assert.IsFalse(fsm.ContainsANDGuardConditionOn(1, 0, 2, TestGuardCondition));
            Assert.IsFalse(fsm.ContainsORGuardConditionOn(1, 0, 2, TestGuardCondition));

            bool TestGuardCondition(int stateFrom, int trigger, int stateTo)
            {
                return true;
            }
        }
        
        [TestMethod]
        public void ThrowAnExceptionIfUserTriesToAddOrRemoveANullGuardCondition()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.AddTransition(1, 0, 2);

            Assert.ThrowsException<ArgumentNullException>(() => fsm.AddANDGuardConditionTo(1, 0, 2, null));

            Assert.ThrowsException<ArgumentNullException>(() => fsm.AddORGuardConditionTo(1, 0, 2, null));

            Assert.ThrowsException<ArgumentNullException>(() => fsm.RemoveANDGuardConditionFrom(1, 0, 2, null));

            Assert.ThrowsException<ArgumentNullException>(() => fsm.RemoveORGuardConditionFrom(1, 0, 2, null));

        }

        [TestMethod]
        public void ThrowAnExceptionIfUserTriesToAddOrRemoveGuardConditionOnANotAddedTransition()
        {
            var fsm = new FSM<int, int>();

            Assert.ThrowsException<TransitionNotAddedException>(() => fsm.AddANDGuardConditionTo(1, 2, 3, TestGuardCondition));

            Assert.ThrowsException<TransitionNotAddedException>(() => fsm.AddORGuardConditionTo(1, 2, 3, TestGuardCondition));

            Assert.ThrowsException<TransitionNotAddedException>(() => fsm.RemoveANDGuardConditionFrom(1, 2, 3, TestGuardCondition));

            Assert.ThrowsException<TransitionNotAddedException>(() => fsm.RemoveORGuardConditionFrom(1, 2, 3, TestGuardCondition));

            bool TestGuardCondition(int stateFrom, int trigger, int stateTo)
            {
                return true;
            }
        }

        [TestMethod]
        public void TransitionateIfAllANDGuardConditionsReturnTrue()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.AddTransition(1, 0, 2);

            Func<int, int, int, bool> guardCondition1 = Substitute.For<Func<int, int, int, bool>>();
            Func<int, int, int, bool> guardCondition2 = Substitute.For<Func<int, int, int, bool>>();

            guardCondition1.Invoke(1, 0, 2).Returns(true);
            guardCondition2.Invoke(1, 0, 2).Returns(true);

            fsm.AddANDGuardConditionTo(1, 0, 2, guardCondition1);

            fsm.AddANDGuardConditionTo(1, 0, 2, guardCondition2);

            fsm.SetInitialState(1);

            fsm.Start();

            fsm.Trigger(0);

            guardCondition1.Received().Invoke(1, 0, 2);
            guardCondition2.Received().Invoke(1, 0, 2);

            Assert.IsTrue(fsm.IsInState(2));

        }

        [TestMethod]
        public void NotTransitionateIfAnyANDGuardConditionReturnsFalse()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.AddTransition(1, 0, 2);

            Func<int, int, int, bool> guardCondition1 = Substitute.For<Func<int, int, int, bool>>();
            Func<int, int, int, bool> guardCondition2 = Substitute.For<Func<int, int, int, bool>>();
            Func<int, int, int, bool> guardCondition3 = Substitute.For<Func<int, int, int, bool>>();

            guardCondition1.Invoke(1, 0, 2).Returns(true);
            guardCondition2.Invoke(1, 0, 2).Returns(false);
            guardCondition3.Invoke(1, 0, 2).Returns(true);

            fsm.AddANDGuardConditionTo(1, 0, 2, guardCondition1);

            fsm.AddANDGuardConditionTo(1, 0, 2, guardCondition2);

            fsm.AddANDGuardConditionTo(1, 0, 2, guardCondition3);

            fsm.SetInitialState(1);

            fsm.Start();

            fsm.Trigger(0);

            guardCondition1.Received().Invoke(1, 0, 2);
            guardCondition2.Received().Invoke(1, 0, 2);
            guardCondition3.DidNotReceive().Invoke(1, 0, 2);

            Assert.IsFalse(fsm.IsInState(2));
        }

        [TestMethod]
        public void TransitionateIfAnyORGuardConditionReturnsTrue()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.AddTransition(1, 0, 2);

            Func<int, int, int, bool> guardCondition1 = Substitute.For<Func<int, int, int, bool>>();
            Func<int, int, int, bool> guardCondition2 = Substitute.For<Func<int, int, int, bool>>();
            Func<int, int, int, bool> guardCondition3 = Substitute.For<Func<int, int, int, bool>>();

            guardCondition1.Invoke(1, 0, 2).Returns(false);
            guardCondition2.Invoke(1, 0, 2).Returns(true);
            guardCondition3.Invoke(1, 0, 2).Returns(false);
            

            fsm.AddORGuardConditionTo(1, 0, 2, guardCondition1);

            fsm.AddORGuardConditionTo(1, 0, 2, guardCondition2);

            fsm.SetInitialState(1);

            fsm.Start();

            fsm.Trigger(0);

            guardCondition1.Received().Invoke(1, 0, 2);
            guardCondition2.Received().Invoke(1, 0, 2);
            guardCondition3.DidNotReceive().Invoke(1, 0, 2);

            Assert.IsTrue(fsm.IsInState(2));
        }

        [TestMethod]
        public void NotTransitionateIfAllORGuardConditionsReturnFalse()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.AddTransition(1, 0, 2);

            Func<int, int, int, bool> guardCondition1 = Substitute.For<Func<int, int, int, bool>>();
            Func<int, int, int, bool> guardCondition2 = Substitute.For<Func<int, int, int, bool>>();

            guardCondition1.Invoke(1, 0, 2).Returns(false);
            guardCondition2.Invoke(1, 0, 2).Returns(false);

            fsm.AddORGuardConditionTo(1, 0, 2, guardCondition1);

            fsm.AddORGuardConditionTo(1, 0, 2, guardCondition2);

            fsm.SetInitialState(1);

            fsm.Start();

            fsm.Trigger(0);

            guardCondition1.Received().Invoke(1, 0, 2);
            guardCondition2.Received().Invoke(1, 0, 2);

            Assert.IsFalse(fsm.IsInState(2));
        }

        [TestMethod]
        public void ReenterStateWhenStateToIsEqualToStateFrom()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);

            fsm.AddTransition(1, 0, 1);

            fsm.SetInitialState(1);

            fsm.Start();

            fsm.Trigger(0);

            state1.Received().Exit();
            state1.Received(2).Enter();
        }

        [TestMethod]
        public void RemoveTransitionsRelatedToAStateIdWhenItIsRemoved()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.AddTransition(1, 0, 1);
            fsm.AddTransition(1, 0, 2);
            fsm.AddTransition(2, 0, 1);

            fsm.RemoveState(2);

            Assert.IsFalse(fsm.ContainsTransition(1, 0, 2) && fsm.ContainsTransition(2, 0, 1));
        }

        [TestMethod]
        public void RemoveGuardConditionsRelatedToATransitionWhenItIsRemoved()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            fsm.AddState(1, state1);
            fsm.AddState(2, state2);

            fsm.AddTransition(1, 0, 2);

            fsm.AddANDGuardConditionTo(1, 0, 2, TestGuardCondition);
            fsm.AddORGuardConditionTo(1, 0, 2, TestGuardCondition);

            fsm.RemoveState(1);

            Assert.IsFalse(fsm.ContainsANDGuardConditionOn(1, 0, 2, TestGuardCondition));
            Assert.IsFalse(fsm.ContainsORGuardConditionOn(1, 0, 2, TestGuardCondition));

            bool TestGuardCondition(int a, int b, int c)
            {
                return true;
            }
        }

        [TestMethod]
        public void TransitionQueued()
        {
            var fsm = new FSM<int, int>();

            var transition1 = new FSMTransition<int, int>(1, 0, 2);
            var transition2 = new FSMTransition<int, int>(2, 0, 3);
            var transition3 = new FSMTransition<int, int>(3, 0, 4);
            var transition4 = new FSMTransition<int, int>(4, 0, 5);

            var state1 = Substitute.For<IFSMState<int, int>>();
            var state3 = Substitute.For<IFSMState<int, int>>();
            var state4 = Substitute.For<IFSMState<int, int>>();
            var state5 = Substitute.For<IFSMState<int, int>>();
            var state2 = new DelegateFSMState<int, int>
                (fsm, 
                () =>
                {
                    fsm.Trigger(transition2.Trigger);
                    fsm.Trigger(transition3.Trigger);
                    fsm.Trigger(transition4.Trigger);
                    state3.DidNotReceive().Enter();
                }, null, null);


            fsm.AddState(1, state1)
                .AddState(2, state2)
                .AddState(3, state3)
                .AddState(4, state4)
                .AddState(5, state5)
                .AddTransitionWithValuesOf(transition1)
                .AddTransitionWithValuesOf(transition2)
                .AddTransitionWithValuesOf(transition3)
                .AddTransitionWithValuesOf(transition4)
                .SetInitialState(1)
                .Start();

            fsm.Trigger(transition1.Trigger);

            Assert.IsTrue(fsm.IsInState(5));

        }
    }
}
