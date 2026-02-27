using UnityEngine;

namespace CodeBase.Infractructure
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
