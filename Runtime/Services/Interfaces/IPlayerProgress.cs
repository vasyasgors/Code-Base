namespace CodeBase.Infrastructure
{
    public interface IPlayerProgressProvider : IService
    {
        PlayerProgress Progress { get; set; }
        PlayerProgress DefaultProgress { get; }
    }
}


