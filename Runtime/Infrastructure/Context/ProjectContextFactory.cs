using UnityEngine;

namespace CodeBase.Infractructure
{
    public class ProjectContextFactory : MonoBehaviour
    {
        private static readonly string ProjectContextResourcePath = "ProjectContext";

        public static void TryCreate()
        {
            if (ProjectContext.Initialized == true) return;

            ProjectContext prefab = TryGetPrefab();

            if (prefab != null)
            {
                GameObject.Instantiate(prefab);
            }
            else
            {
                Debug.Log("Project context not found!");
            }
        }

        private static ProjectContext TryGetPrefab()
        {
            var prefabs = Resources.LoadAll<ProjectContext>(ProjectContextResourcePath);

            if (prefabs.Length > 0)
            {
                return prefabs[0];
            }

            return null;
        }
    }

}


