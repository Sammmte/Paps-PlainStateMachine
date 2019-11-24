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

            Assert.IsTrue(item1.GetStateId() == 1 && item2.GetStateId() == 2);
        }

        [TestMethod]
        public void RemoveStates()
        {
            var state = Substitute.For<IFSMState<int, int>>();

            FSM<int, int> fsm = new FSM<int, int>();

            state.StateMachine.Returns(fsm);

            fsm.AddState(1, state);

            Assert.IsTrue(fsm.StateCount == 1);

            fsm.RemoveState(state.GetStateId());

            Assert.IsTrue(fsm.StateCount == 0);
        }

        [TestMethod]
        public void IterateOverTransitions()
        {
            var transition1 = new FSMTransition<int, int>(1,2,3);
            var transition2 = new FSMTransition<int, int>(4,5,6);

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddTransition(transition1.StateFrom, transition1.Trigger, transition1.StateTo);
            fsm.AddTransition(transition2.StateFrom, transition2.Trigger, transition2.StateTo);

            FSMTransition<int, int> item1 = default;
            FSMTransition<int, int> item2 = default;

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
            var transition = new FSMTransition<int, int>(1, 2, 3);

            FSM<int, int> fsm = new FSM<int, int>();

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
        public void ThrowAnExceptionIfUserTriesToStartAndInitialStateIsInvalid()
        {
            var fsm = new FSM<int, int>();

            fsm.SetInitialState(1);

            Assert.ThrowsException<InvalidInitialStateException>(() => fsm.Start());
        }

        [TestMethod]
        public void ThrowAnExceptionIfUserTriesToStartAndItsAlreadyStarted()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);

            fsm.SetInitialState(state1.GetStateId());

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

            fsm.SetInitialState(state1.GetStateId());

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

            fsm.SetInitialState(state1.GetStateId());

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

            fsm.SetInitialState(state1.GetStateId());

            fsm.Start();

            Assert.IsTrue(fsm.IsInState(state1.GetStateId()));
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

            fsm.SetInitialState(state1.GetStateId());

            fsm.AddTransition(state1.GetStateId(), 0, state2.GetStateId());

            fsm.Start();

            Assert.IsTrue(fsm.IsInState(state1.GetStateId()));

            fsm.Trigger(0);

            Assert.IsTrue(fsm.IsInState(state2.GetStateId()));
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
            var fsm = new FSM<int, int>();

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

            Assert.IsTrue(fsm.ContainsState(state1.GetStateId()));
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

            fsm.SetInitialState(state1.GetStateId());

            fsm.AddTransition(state1.GetStateId(), 0, state2.GetStateId());

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

            fsm.SetInitialState(state1.GetStateId());

            Assert.ThrowsException<FSMNotStartedException>(fsm.Update);
        }

        [TestMethod]
        public void UpdateCurrentState()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);

            fsm.SetInitialState(state1.GetStateId());

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

            fsm.SetInitialState(state1.GetStateId());

            fsm.AddTransition(state1.GetStateId(), 0, state2.GetStateId());

            fsm.Start();

            fsm.Trigger(0);

            stateChangedEventHandler
                .Received()
                .Invoke(1, 0, 2);
        }
    }
}
