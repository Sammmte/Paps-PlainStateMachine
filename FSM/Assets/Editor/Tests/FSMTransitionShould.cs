using NUnit.Framework;
using Paps.FSM;

namespace Tests
{
    public class FSMTransitionShould
    {
        [Test]
        public void HasSameHashCodeThatOtherWithSameValues()
        {
            var transition1 = new FSMTransition<int, int>(1, 2, 3);
            var transition2 = new FSMTransition<int, int>(1, 2, 3);

            Assert.AreEqual(transition1.GetHashCode(), transition2.GetHashCode());
            Assert.AreEqual(transition1, transition2);
        }

        [Test]
        public void ReturnTrueWhenCallingEqualsWithOther()
        {
            var transition1 = new FSMTransition<int, int>(1, 2, 3);
            var transition2 = new FSMTransition<int, int>(1, 2, 3);

            Assert.IsTrue(transition1.Equals(transition2));
        }
    }
}
