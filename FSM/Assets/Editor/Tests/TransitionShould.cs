using NUnit.Framework;
using Paps.FSM;

namespace Tests
{
    public class TransitionShould
    {
        [Test]
        public void HasSameHashCodeThatOtherWithSameValues()
        {
            var transition1 = new Transition<int, int>(1, 2, 3);
            var transition2 = new Transition<int, int>(1, 2, 3);

            Assert.AreEqual(transition1.GetHashCode(), transition2.GetHashCode());
            Assert.AreEqual(transition1, transition2);
        }

        [Test]
        public void ReturnTrueWhenCallingEqualsWithOtherWithSameValues()
        {
            var transition1 = new Transition<int, int>(1, 2, 3);
            var transition2 = new Transition<int, int>(1, 2, 3);

            Assert.IsTrue(transition1.Equals(transition2));
        }
    }
}
