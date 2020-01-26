using NSubstitute;
using NUnit.Framework;
using Paps.FSM;
using Paps.FSM.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Paps.FSM.Extensions.BehaviouralStates;

namespace Tests
{
    public class FSMExtensionsForBehaviouralStatesShould
    {
        private class TestStateBehaviour : IStateBehaviour
        {
            public void OnEnter()
            {

            }

            public void OnExit()
            {

            }

            public void OnUpdate()
            {

            }
        }

        [Test]
        public void Add_Behavioural_States()
        {
            var fsm = new FSM<int, int>();
            
            var behaviouralState1 = fsm.AddBehaviouralState(1);

            Assert.IsTrue(fsm.ContainsState(1));
            Assert.IsTrue(fsm.GetStateById(1) == behaviouralState1);
            
            IStateBehaviour stateBehaviour1 = Substitute.For<IStateBehaviour>();
            IStateBehaviour stateBehaviour2 = Substitute.For<IStateBehaviour>();

            var behaviouralState2 = fsm.AddBehaviouralState(2, stateBehaviour1, stateBehaviour2);

            Assert.IsTrue(fsm.ContainsState(2));
            Assert.IsTrue(fsm.GetStateById(2) == behaviouralState2);
            Assert.IsTrue(fsm.ContainsBehaviour(stateBehaviour1));
            Assert.IsTrue(fsm.ContainsBehaviour(stateBehaviour2));
            Assert.IsTrue(fsm.BehaviourCount() == 2);
            Assert.IsTrue(fsm.BehaviourCountOf(2) == 2);
        }

        [Test]
        public void Add_Behaviours_To_Behavioural_States_After_Creation()
        {
            var fsm = new FSM<int, int>();

            var stateBehaviour1 = Substitute.For<IStateBehaviour>();

            fsm.AddBehaviouralState(1);

            fsm.AddBehaviourTo(1, stateBehaviour1);

            Assert.IsTrue(fsm.ContainsBehaviour(stateBehaviour1));
            Assert.IsTrue(fsm.ContainsBehaviourOn(1, stateBehaviour1));
            Assert.IsTrue(fsm.BehaviourCount() == 1);
            Assert.IsTrue(fsm.BehaviourCountOf(1) == 1);

            var stateBehaviour2 = fsm.AddBehaviourTo<TestStateBehaviour, int, int>(1);

            Assert.IsTrue(fsm.ContainsBehaviour(stateBehaviour2));
            Assert.IsTrue(fsm.ContainsBehaviourOn(1, stateBehaviour2));
            Assert.IsTrue(fsm.BehaviourCount() == 2);
            Assert.IsTrue(fsm.BehaviourCountOf(1) == 2);

            var stateBehaviour3 = Substitute.For<IStateBehaviour>();
            var stateBehaviour4 = Substitute.For<IStateBehaviour>();

            fsm.AddBehavioursTo(1, stateBehaviour3, stateBehaviour4);

            Assert.IsTrue(fsm.ContainsBehaviour(stateBehaviour3));
            Assert.IsTrue(fsm.ContainsBehaviourOn(1, stateBehaviour3));
            Assert.IsTrue(fsm.ContainsBehaviour(stateBehaviour4));
            Assert.IsTrue(fsm.ContainsBehaviourOn(1, stateBehaviour4));
            Assert.IsTrue(fsm.BehaviourCount() == 4);
            Assert.IsTrue(fsm.BehaviourCountOf(1) == 4);
        }

        [Test]
        public void Do_Nothing_If_User_Tries_To_Add_The_Same_Behaviour_On_The_Same_State_Twice()
        {
            var fsm = new FSM<int, int>();

            var stateBehaviour1 = Substitute.For<IStateBehaviour>();

            fsm.AddBehaviouralState(1);

            fsm.AddBehaviourTo(1, stateBehaviour1);
            Assert.DoesNotThrow(() => fsm.AddBehaviourTo(1, stateBehaviour1));
            Assert.IsTrue(fsm.BehaviourCount() == 1);
            Assert.IsTrue(fsm.BehaviourCountOf(1) == 1);
        }

        [Test]
        public void Remove_Behaviours_From_Behavioural_States()
        {
            var fsm = new FSM<int, int>();

            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();

            fsm.AddBehaviouralState(1, stateBehaviour1, stateBehaviour2);

            fsm.RemoveBehaviourFrom(1, stateBehaviour1);

            Assert.IsTrue(fsm.BehaviourCount() == 1);
            Assert.IsTrue(fsm.BehaviourCountOf(1) == 1);
            Assert.IsFalse(fsm.ContainsBehaviour(stateBehaviour1));
            Assert.IsFalse(fsm.ContainsBehaviourOn(1, stateBehaviour1));
            Assert.IsTrue(fsm.ContainsBehaviour(stateBehaviour2));
            Assert.IsTrue(fsm.ContainsBehaviourOn(1, stateBehaviour2));

            fsm.RemoveBehaviourFrom(1, stateBehaviour2);

            Assert.IsTrue(fsm.BehaviourCount() == 0);
            Assert.IsTrue(fsm.BehaviourCountOf(1) == 0);
            Assert.IsFalse(fsm.ContainsBehaviour(stateBehaviour2));
            Assert.IsFalse(fsm.ContainsBehaviourOn(1, stateBehaviour2));
        }

        [Test]
        public void Return_Behaviour_Of_Specific_Type_From_Any_State()
        {
            var fsm = new FSM<int, int>();

            var stateBehaviour = new TestStateBehaviour();

            fsm.AddBehaviouralState(1, stateBehaviour);

            Assert.AreEqual(stateBehaviour, fsm.GetBehaviour<TestStateBehaviour, int, int>());
        }
        
        [Test]
        public void Return_Behaviour_Of_Specific_Type_From_Specific_State()
        {
            var fsm = new FSM<int, int>();

            var stateBehaviour = new TestStateBehaviour();

            fsm.AddBehaviouralState(1, stateBehaviour);

            Assert.AreEqual(stateBehaviour, fsm.GetBehaviourOf<TestStateBehaviour, int, int>(1));
        }

        [Test]
        public void Return_Behaviours_Of_Specific_Type_From_All_Behavioural_States()
        {
            var fsm = new FSM<int, int>();

            var stateBehaviour1 = new TestStateBehaviour();
            var stateBehaviour2 = new TestStateBehaviour();
            var stateBehaviour3 = Substitute.For<IStateBehaviour>();

            fsm.AddBehaviouralState(1, stateBehaviour1, stateBehaviour2, stateBehaviour3);

            var behaviors = fsm.GetBehaviours<TestStateBehaviour, int, int>();

            Assert.IsTrue(behaviors.Length == 2);
            Assert.IsTrue(behaviors.Contains(stateBehaviour1));
            Assert.IsTrue(behaviors.Contains(stateBehaviour2));
        }

        [Test]
        public void Return_Behaviours_Of_Specific_Type_From_Specific_Behavioural_State()
        {
            var fsm = new FSM<int, int>();

            var stateBehaviour1 = new TestStateBehaviour();
            var stateBehaviour2 = new TestStateBehaviour();
            var stateBehaviour3 = Substitute.For<IStateBehaviour>();

            fsm.AddBehaviouralState(1, stateBehaviour1, stateBehaviour2, stateBehaviour3);

            var behaviors = fsm.GetBehavioursOf<TestStateBehaviour, int, int>(1);

            Assert.IsTrue(behaviors.Length == 2);
            Assert.IsTrue(behaviors.Contains(stateBehaviour1));
            Assert.IsTrue(behaviors.Contains(stateBehaviour2));
        }

        [Test]
        public void Iterate_Over_All_Behaviours()
        {
            var fsm = new FSM<int, int>();

            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();

            fsm.AddBehaviouralState(1, stateBehaviour1, stateBehaviour2);
            
            var behaviours = new List<IStateBehaviour>();

            behaviours.Add(stateBehaviour1);
            behaviours.Add(stateBehaviour2);

            fsm.ForeachBehaviour(behaviour => 
            {
                behaviours.Remove(behaviour);
                return false;
            });

            Assert.IsTrue(behaviours.Count == 0);
        }

        [Test]
        public void Iterate_Over_All_Behaviours_Of_Specific_State()
        {
            var fsm = new FSM<int, int>();

            var stateBehaviour1 = Substitute.For<IStateBehaviour>();
            var stateBehaviour2 = Substitute.For<IStateBehaviour>();

            fsm.AddBehaviouralState(1, stateBehaviour1, stateBehaviour2);

            var behaviours = new List<IStateBehaviour>();

            behaviours.Add(stateBehaviour1);
            behaviours.Add(stateBehaviour2);

            fsm.ForeachBehaviourOn(1, behaviour =>
            {
                Assert.IsFalse(behaviours.Count == 0);
                behaviours.Remove(behaviour);
                return false;
            });

            Assert.IsTrue(behaviours.Count == 0);
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Add_A_Null_Behaviour()
        {
            var fsm = new FSM<int, int>();

            var stateBehaviour1 = Substitute.For<IStateBehaviour>();

            Assert.Throws<ArgumentNullException>(() => fsm.AddBehaviouralState(1, stateBehaviour1, null));
            
            fsm.AddBehaviouralState(1);

            Assert.Throws<ArgumentNullException>(() => fsm.AddBehaviourTo(1, null));
            Assert.Throws<ArgumentNullException>(() => fsm.AddBehavioursTo(1, stateBehaviour1, null));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Remove_A_Null_Behaviour()
        {
            var fsm = new FSM<int, int>();

            Assert.Throws<ArgumentNullException>(() => fsm.RemoveBehaviourFrom(1, null));
        }

        [Test]
        public void Throw_An_Exception_If_User_Asks_If_Contains_Behaviour_With_A_Null_Behaviour()
        {
            var fsm = new FSM<int, int>();

            Assert.Throws<ArgumentNullException>(() => fsm.ContainsBehaviour(null));
            Assert.Throws<ArgumentNullException>(() => fsm.ContainsBehaviourOn(1, null));
        }

        [Test]
        public void Throw_An_Exception_If_User_Tries_To_Iterate_Over_Behaviours_With_A_Null_Delegate()
        {
            var fsm = new FSM<int, int>();

            Assert.Throws<ArgumentNullException>(() => fsm.ForeachBehaviour(null));
            Assert.Throws<ArgumentNullException>(() => fsm.ForeachBehaviourOn(1, null));
        }
    }
}