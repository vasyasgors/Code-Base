using CodeBase.Infractructure;

namespace CodeBase.Infractructure
{
    public interface IGlobalTicker : IService
    {
        void AddTickable(ITickable tickable);
        void RemoveTickable(ITickable tickable);

        void AddFixedTickable(IFixedTickable fixedTickable);
        void RemoveFixedTickable(IFixedTickable fixedTickable);

        void ClearTickable();

        void ClearFixedTickable();
    }
}