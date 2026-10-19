using CodeBase.Infrastructure;

namespace CodeBase
{
    public class LocalLocalization : ILocalization
    {
        public string GetLanguageCode()
        {
            return "en";
        }

    }

}

