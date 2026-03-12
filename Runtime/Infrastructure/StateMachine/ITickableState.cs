namespace CodeBase.Infrastructure
{
    public interface ITickableState : IState
    {
        void Tick();
    }

}
