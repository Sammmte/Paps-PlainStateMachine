using System.Collections.Generic;

namespace Paps.StateMachines.Extensions.BehaviouralStates
{
    public interface IBehaviouralState : IState, IEnumerable<IStateBehaviour>
    {
        int BehaviourCount { get; }

        void AddBehaviour(IStateBehaviour stateBehaviour);

        bool RemoveBehaviour(IStateBehaviour stateBehaviour);

        bool ContainsBehaviour(IStateBehaviour stateBehaviour);

        T GetBehaviour<T>();

        T[] GetBehaviours<T>();
    }
} 