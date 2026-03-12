namespace CodeBase.Infrastructure
{
    public class ProjectContext : Context
    {
        private static ProjectContext Instance;

        public static bool Initialized => Instance != null;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                Install();

                DontDestroyOnLoad(gameObject);

                return;
            }

            Destroy(gameObject);
        }
    }

}

