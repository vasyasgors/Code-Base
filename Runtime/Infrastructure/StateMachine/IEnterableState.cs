namespace CodeBase.Infractructure
{
    public interface IEnterableState : IState
    {
        void Enter();
    }

    public interface IEnterableState<T1> : IState
    {
        void Enter(T1 payload);
    }


}

