using NUnit.Framework;
using Paps.StateMachines;
using System.Collections.Generic;

namespace Tests
{
    public class TransitionShould
    {
        [Test]
        public void Has_Same_Hash_Code_That_Other_With_Same_Values()
        {
            var transition1 = new Transition<int, int>(1, 2, 3);
            var transition2 = new Transition<int, int>(1, 2, 3);

            Assert.AreEqual(transition1.GetHashCode(), transition2.GetHashCode());
            Assert.AreEqual(transition1, transition2);
        }

        [Test]
        public void Return_True_When_Calling_Equals_With_Other_With_Same_Values()
        {
            var transition1 = new Transition<int, int>(1, 2, 3);
            var transition2 = new Transition<int, int>(1, 2, 3);

            Assert.IsTrue(transition1.Equals(transition2));
        }

        [Test]
        public void Work_With_Hashsets()
        {
            var transition = new Transition<int, int>(1, 2, 3);

            HashSet<Transition<int, int>> hashset = new HashSet<Transition<int, int>>();

            hashset.Add(transition);

            Assert.IsTrue(hashset.Contains(transition));
        }
    }
}
