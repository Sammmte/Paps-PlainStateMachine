using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paps.FSM;
using NSubstitute;

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

            foreach(IFSMState<int, int> state in fsm)
            {
                if(cont == 1)
                {
                    item1 = state;
                }
                else
                {
                    item2 = state;
                }

                cont++;
            }

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
    }
}
