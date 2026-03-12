namespace CodeBase.Infrastructure
{
    public interface IExitableState : IState
    {
        void Exit();
    }

}
