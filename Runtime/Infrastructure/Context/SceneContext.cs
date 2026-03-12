using UnityEngine;

namespace CodeBase.Infrastructure
{
    [DefaultExecutionOrder(-10000)] 
    public class SceneContext : Context
    {
        private void Awake()
        {
            ProjectContextFactory.TryCreate();

            Install();           
        }
    }
}
