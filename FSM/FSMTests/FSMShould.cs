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

            state1.StateId.Returns(1);
            state2.StateId.Returns(1);

            FSM<int, int> fsm = new FSM<int, int>();

            Assert.ThrowsException<StateIdAlreadyAddedException>(
                () => 
                {
                    fsm.AddState(state1);
                    fsm.AddState(state2);
                });
        }

        [TestMethod]
        public void ThrowAnExceptionIfUserAddsANullState()
        {
            FSM<int, int> fsm = new FSM<int, int>();

            Assert.ThrowsException<ArgumentNullException>(() => fsm.AddState(null));
        }

        [TestMethod]
        public void IterateOverStates()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            state1.StateId.Returns(1);
            state2.StateId.Returns(2);

            IFSMState<int, int> item1 = null;
            IFSMState<int, int> item2 = null;

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddState(state1);
            fsm.AddState(state2);

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
                }
                );

            Assert.IsTrue(item1.StateId == 1 && item2.StateId == 2);
        }

        [TestMethod]
        public void RemoveStates()
        {
            var state = Substitute.For<IFSMState<int, int>>();

            FSM<int, int> fsm = new FSM<int, int>();

            fsm.AddState(state);

            Assert.IsTrue(fsm.StateCount == 1);

            fsm.RemoveState(state);

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

            state1.StateId.Returns(1);

            var fsm = new FSM<int, int>();

            fsm.AddState(state1);

            fsm.SetInitialState(state1.StateId);

            fsm.Start();

            Assert.ThrowsException<FSMStartedException>(() => fsm.Start());
        }

        [TestMethod]
        public void ShowCorrespondingValueWhenAskedIfIsStarted()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateId.Returns(1);

            fsm.AddState(state1);

            fsm.SetInitialState(state1.StateId);

            Assert.IsFalse(fsm.IsStarted);

            fsm.Start();

            Assert.IsTrue(fsm.IsStarted);
        }
        
        [TestMethod]
        public void EnterInitialStartWhenStarted()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            
            var fsm = new FSM<int, int>();

            fsm.AddState(state1);

            fsm.SetInitialState(state1.StateId);

            fsm.Start();

            state1.Received().Enter();
        }

        [TestMethod]
        public void ReturnsCorrespondingValueWhenAskedIsInState()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateId.Returns(1);

            fsm.AddState(state1);

            fsm.SetInitialState(state1.StateId);

            fsm.Start();

            Assert.IsTrue(fsm.IsInState(state1.StateId));
        }
        
        [TestMethod]
        public void ChangeStateWhenTriggeringAnExistingTransition()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateId.Returns(1);
            state2.StateId.Returns(2);

            fsm.AddState(state1);
            fsm.AddState(state2);

            fsm.SetInitialState(state1.StateId);

            fsm.AddTransition(state1.StateId, 0, state2.StateId);

            fsm.Start();

            Assert.IsTrue(fsm.IsInState(state1.StateId));

            fsm.Trigger(0);

            Assert.IsTrue(fsm.IsInState(state2.StateId));
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

            state1.StateId.Returns(1);

            fsm.AddState(state1);

            Assert.IsTrue(fsm.ContainsState(state1.StateId));
            Assert.IsFalse(fsm.ContainsState(2));
        }

        [TestMethod]
        public void ExitCurrentStateWhenStopped()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();
            var state2 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateId.Returns(1);
            state2.StateId.Returns(2);

            fsm.AddState(state1);
            fsm.AddState(state2);

            fsm.SetInitialState(state1.StateId);

            fsm.AddTransition(state1.StateId, 0, state2.StateId);

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

            state1.StateId.Returns(1);

            fsm.AddState(state1);

            fsm.SetInitialState(state1.StateId);

            Assert.ThrowsException<FSMNotStartedException>(fsm.Update);
        }

        [TestMethod]
        public void UpdateCurrentState()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateId.Returns(1);

            fsm.AddState(state1);

            fsm.SetInitialState(state1.StateId);

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

            state1.StateId.Returns(1);
            state2.StateId.Returns(2);

            fsm.AddState(state1);
            fsm.AddState(state2);

            fsm.SetInitialState(state1.StateId);

            fsm.AddTransition(state1.StateId, 0, state2.StateId);

            fsm.Start();

            fsm.Trigger(0);

            stateChangedEventHandler
                .Received()
                .Invoke(1, 0, 2);
        }
    }
}
