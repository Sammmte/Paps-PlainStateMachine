using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Paps.FSM;
using NSubstitute;

namespace FSMTests
{
    [TestClass]
    public class FSMTransitionShould
    {
        [TestMethod]
        public void HasSameHashCodeThatOtherWithSameValues()
        {
            var transition1 = new FSMTransition<int, int>(1, 2, 3);
            var transition2 = new FSMTransition<int, int>(1, 2, 3);

            Assert.AreEqual(transition1.GetHashCode(), transition2.GetHashCode());
            Assert.AreEqual(transition1, transition2);
        }

        [TestMethod]
        public void ReturnTrueWhenCallingEqualsWithOther()
        {
            var transition1 = new FSMTransition<int, int>(1, 2, 3);
            var transition2 = new FSMTransition<int, int>(1, 2, 3);

            Assert.IsTrue(transition1.Equals(transition2));
        }
    }
}
