using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paps.FSM;
using NSubstitute;
using Paps.FSM.Extensions;
using System.Linq;

namespace FSMTests
{
    [TestClass]
    public class IFSMExtensionsShould
    {
        [TestMethod]
        public void GetState()
        {
            var state1 = Substitute.For<IFSMState<int, int>>();

            var fsm = new FSM<int, int>();

            state1.StateMachine.Returns(fsm);

            fsm.AddState(1, state1);

            var shouldBeState1 = fsm.GetState<IFSMState<int, int>, int, int>();

            Assert.AreEqual(state1, shouldBeState1);
        }

        [TestMethod]
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

        [TestMethod]
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
    }
}
