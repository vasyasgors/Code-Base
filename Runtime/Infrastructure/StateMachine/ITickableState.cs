namespace CodeBase.Infractructure
{
    public interface ITickableState : IState
    {
        void Tick();
    }

}
