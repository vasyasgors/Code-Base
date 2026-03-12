using CodeBase.Infrastructure;

using System;

namespace CodeBase.Infrastructure
{
    public interface IGameStateSwither : IService
    {
        event Action<Type> Entered;
        event Action<Type> Exited;

        object CurrentState { get; }

        void AddState<TState>(TState state) where TState : class, IState;
        void RemoveState<TState>() where TState : class, IState;
        void Enter<TState>() where TState : class, IState;
        void Enter<TState, TPayload>(TPayload payload) where TState : class, IState;
        void Enter(Type type);
        void Enter<TPayload>(Type type);
        void Exit<TState>() where TState : class, IState;
        void UpdateTick();
    }
}
