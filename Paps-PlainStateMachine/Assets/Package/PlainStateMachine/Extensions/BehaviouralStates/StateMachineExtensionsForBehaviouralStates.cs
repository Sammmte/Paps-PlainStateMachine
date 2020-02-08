using System;
using System.Collections.Generic;
using System.Linq;

namespace Paps.StateMachines.Extensions.BehaviouralStates
{
    public static class StateMachineExtensions
    {
        public static BehaviouralState AddBehaviouralState<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId)
        {
            var state = new BehaviouralState();
            fsm.AddState(stateId, state);
            return state;
        }

        public static BehaviouralState AddBehaviouralState<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId, params IStateBehaviour[] behaviours)
        {
            var state = new BehaviouralState(behaviours);
            fsm.AddState(stateId, state);
            return state;
        }

        public static void AddBehaviourTo<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId, IStateBehaviour behaviour)
        {
            if (behaviour == null) throw new ArgumentNullException(nameof(behaviour));

            IState stateObj = fsm.GetStateById(stateId);

            if (stateObj is BehaviouralState cast)
            {
                cast.AddBehaviour(behaviour);
            }
            else throw new InvalidOperationException("State object with id " + stateId + " is not of type " + typeof(BehaviouralState).Name);
        }

        public static TBehaviour AddBehaviourTo<TBehaviour, TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId) where TBehaviour : IStateBehaviour, new()
        {
            IState stateObj = fsm.GetStateById(stateId);

            if (stateObj is BehaviouralState cast)
            {
                var behaviour = new TBehaviour();
                cast.AddBehaviour(behaviour);
                return behaviour;
            }
            else throw new InvalidOperationException("State object with id " + stateId + " is not of type " + typeof(BehaviouralState).Name);
        }

        public static void AddBehavioursTo<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId, params IStateBehaviour[] behaviours)
        {
            if (behaviours.Any(behaviour => behaviour == null)) throw new ArgumentNullException("some behaviour parameter is null");

            IState stateObj = fsm.GetStateById(stateId);

            if (stateObj is BehaviouralState cast)
            {
                for (int i = 0; i < behaviours.Length; i++)
                {
                    cast.AddBehaviour(behaviours[i]);
                }
            }
            else throw new InvalidOperationException("State object with id " + stateId + " is not of type " + typeof(BehaviouralState).Name);
        }

        public static void RemoveBehaviourFrom<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId, IStateBehaviour behaviour)
        {
            if (behaviour == null) throw new ArgumentNullException(nameof(behaviour));

            IState stateObj = fsm.GetStateById(stateId);

            if (stateObj is BehaviouralState cast)
            {
                cast.RemoveBehaviour(behaviour);
            }
            else throw new InvalidOperationException("State object with id " + stateId + " is not of type " + typeof(BehaviouralState).Name);
        }

        public static TBehaviour GetBehaviour<TBehaviour, TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm)
        {
            BehaviouralState[] stateObjects = fsm.GetStates<BehaviouralState, TState, TTrigger>();

            for (int i = 0; i < stateObjects.Length; i++)
            {
                TBehaviour behaviour = stateObjects[i].GetBehaviour<TBehaviour>();

                if (behaviour != null) return behaviour;
            }

            return default;
        }

        public static TBehaviour GetBehaviourOf<TBehaviour, TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId)
        {
            IState stateObj = fsm.GetStateById(stateId);

            if (stateObj is BehaviouralState cast)
            {
                return cast.GetBehaviour<TBehaviour>();
            }
            else throw new InvalidOperationException("State object with id " + stateId + " is not of type " + typeof(BehaviouralState).Name);
        }

        public static TBehaviour[] GetBehaviours<TBehaviour, TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm)
        {
            BehaviouralState[] stateObjects = fsm.GetStates<BehaviouralState, TState, TTrigger>();

            if (stateObjects != null)
            {
                List<TBehaviour> behaviours = new List<TBehaviour>();

                for (int i = 0; i < stateObjects.Length; i++)
                {
                    TBehaviour[] candidates = stateObjects[i].GetBehaviours<TBehaviour>();

                    if (candidates != null) behaviours.AddRange(candidates);
                }

                if (behaviours.Count > 0) return behaviours.ToArray();
            }

            return null;
        }

        public static TBehaviour[] GetBehavioursOf<TBehaviour, TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId)
        {
            IState stateObj = fsm.GetStateById(stateId);

            if (stateObj is BehaviouralState cast)
            {
                return cast.GetBehaviours<TBehaviour>();
            }
            else throw new InvalidOperationException("State object with id " + stateId + " is not of type " + typeof(BehaviouralState).Name);
        }

        public static bool ContainsBehaviour<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, IStateBehaviour behaviour)
        {
            if (behaviour == null) throw new ArgumentNullException(nameof(behaviour));

            BehaviouralState[] stateObjects = fsm.GetStates<BehaviouralState, TState, TTrigger>();

            for (int i = 0; i < stateObjects.Length; i++)
            {
                if (stateObjects[i].ContainsBehaviour(behaviour)) return true;
            }

            return false;
        }

        public static bool ContainsBehaviourOn<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId, IStateBehaviour behaviour)
        {
            if (behaviour == null) throw new ArgumentNullException(nameof(behaviour));

            IState state = fsm.GetStateById(stateId);

            if (state is BehaviouralState cast)
            {
                return cast.ContainsBehaviour(behaviour);
            }

            return false;
        }

        public static void ForeachBehaviour<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, ReturnTrueToFinishIteration<IStateBehaviour> finishable)
        {
            if (finishable == null) throw new ArgumentNullException(nameof(finishable));

            IStateBehaviour[] behaviours = fsm.GetBehaviours<IStateBehaviour, TState, TTrigger>();

            for (int i = 0; i < behaviours.Length; i++)
            {
                if (finishable(behaviours[i])) return;
            }
        }

        public static void ForeachBehaviourOn<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId, ReturnTrueToFinishIteration<IStateBehaviour> finishable)
        {
            if (finishable == null) throw new ArgumentNullException(nameof(finishable));

            IState stateObj = fsm.GetStateById(stateId);

            if (stateObj is BehaviouralState cast)
            {
                foreach (var behaviour in cast)
                {
                    if (finishable(behaviour)) return;
                }
            }
            else throw new InvalidOperationException("State object with id " + stateId + " is not of type " + typeof(BehaviouralState).Name);
        }

        public static int BehaviourCount<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm)
        {
            var behaviours = GetBehaviours<IStateBehaviour, TState, TTrigger>(fsm);

            if (behaviours == null) return 0;

            return behaviours.Length;
        }

        public static int BehaviourCountOf<TState, TTrigger>(this IStateMachine<TState, TTrigger> fsm, TState stateId)
        {
            IState stateObj = fsm.GetStateById(stateId);

            if (stateObj is BehaviouralState cast)
            {
                return cast.BehaviourCount;
            }
            else throw new InvalidOperationException("State object with id " + stateId + " is not of type " + typeof(BehaviouralState).Name);
        }
    }
}