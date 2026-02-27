using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infractructure
{
    public abstract class StateMachine 
    {
        public event Action<Type> Entered;
        public event Action<Type> Exited;

        private Dictionary<Type, object> states;

        private object currentState;

        public object CurrentState => currentState;

        public StateMachine()
        {
            states = new Dictionary<Type, object>();
        }

        public void AddState<TState>(TState state) where TState : class, IState
        {
            states.Add(state.GetType(), state); 
        }

        public void RemoveState<TState>() where TState : class, IState
        {
            states.Remove(typeof(TState));
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IState
        {
            if (currentState != null && typeof(TState) == currentState.GetType()) return;

            SetState<TState>();

            if (currentState is IEnterableState<TPayload> enterableState) enterableState.Enter(payload);

            Entered?.Invoke(currentState.GetType());
        }

        public void Enter<TState>() where TState : class, IState
        {
            if (currentState != null && typeof(TState) == currentState.GetType()) return;

            SetState<TState>();

            if (currentState is IEnterableState enterableState) enterableState.Enter();

            Entered?.Invoke(currentState.GetType());
        }

        public void Exit<TState>() where TState : class, IState
        {
            if (currentState is IExitableState exitableState) exitableState.Exit();

            Exited?.Invoke(currentState.GetType());

            currentState = null;
        }

        public void UpdateTick()
        {
            if (currentState is ITickableState tickableState) tickableState.Tick();
        }

        private void SetState<TState>() where TState : class, IState
        {
            if (currentState is IExitableState exitableState) exitableState.Exit();

            TState state = states[typeof(TState)] as TState;

            currentState = state;
        }

    }

}
