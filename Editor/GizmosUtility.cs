using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmosUtility  
{
    public static void DrawBoxCollider(BoxCollider boxCollider, Color color)
    { 
        if (boxCollider != null)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(boxCollider.transform.position, boxCollider.transform.rotation, boxCollider.transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            Gizmos.color = new Color(color.r, color.g, color.b, 0.3f);
      
            Gizmos.DrawCube(boxCollider.center, boxCollider.size);

            Gizmos.color = new Color(color.r, color.g, color.b, 1);
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);

        }
    }
}
