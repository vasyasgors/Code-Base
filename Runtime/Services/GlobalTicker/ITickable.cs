namespace CodeBase.Infrastructure
{
    public interface ITickable
    {
        void Tick();
    }

    public interface IFixedTickable
    {
        void FixedTick();
    }
}