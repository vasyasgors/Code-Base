using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase
{
    public class RuntimeStaticBatching : MonoBehaviour
    {
        
        private void Start()
        {
            CombineMeshes();
        }

        private void CombineMeshes()
        {
            List<CombineInstance> combines = new List<CombineInstance>();

            foreach (Transform child in transform)
            {
                MeshRenderer mr = child.GetComponent<MeshRenderer>();
                MeshFilter mf = child.GetComponent<MeshFilter>();

                if (mr != null && mf != null && mf.sharedMesh != null)
                {
                    CombineInstance combine = new CombineInstance
                    {
                        mesh = mf.sharedMesh,
                        transform = child.localToWorldMatrix,
                        subMeshIndex = 0
                    };
                    combines.Add(combine);

                    // Скрываем оригинальный объект
                    mr.enabled = false;
                }
            }

            if (combines.Count > 0)
            {
                MeshFilter combinedFilter = GetComponent<MeshFilter>();
                MeshRenderer combinedRenderer = GetComponent<MeshRenderer>();

                if (combinedFilter == null)
                    combinedFilter = gameObject.AddComponent<MeshFilter>();
                if (combinedRenderer == null)
                    combinedRenderer = gameObject.AddComponent<MeshRenderer>();

                Mesh combinedMesh = new Mesh();
                combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                combinedMesh.CombineMeshes(combines.ToArray(), true, false);
                combinedMesh.RecalculateBounds();

                combinedFilter.sharedMesh = combinedMesh;
            }
        }
        


    }
}