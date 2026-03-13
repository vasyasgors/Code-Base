using System.Collections.Generic;
using UnityEditor;

using UnityEngine;

namespace CodeBase.Infrastructure
{ 
    public abstract class ConfigListEditorAbstract : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            SerializedProperty serializedProperty = serializedObject.FindProperty("Configs");

            if (GUILayout.Button("Append Id") == true)
            {

                for(int i = 0; i < serializedProperty.arraySize; i++)
                {
                    Object obj = serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue;

                    if (obj == null) continue;

                    SerializedObject serializedObject = new SerializedObject(obj);


                    if (serializedObject.FindProperty("ID").intValue <= -1)
                    {
                        serializedObject.FindProperty("ID").intValue = i;

                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                    }
                }

                AssetDatabase.SaveAssets();

            }

            if (GUILayout.Button("Full update id") == true)
            {

                for (int i = 0; i < serializedProperty.arraySize; i++)
                {
                    SerializedObject serializedObject = new SerializedObject(serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue);

                    serializedObject.FindProperty("ID").intValue = i;

                    serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(serializedProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                    
                }

                AssetDatabase.SaveAssets();

            }
       


        }

    }


}





