using System;
using System.Collections.Generic;
using UnityEngine;


namespace CodeBase.Infrastructure
{
    public class ConfigsProvider : MonoBehaviour, IConfigsProvider
    {
        [Header("Configs (Must be unique)")]
        [SerializeField] private ScriptableObject[] configs;

        private Dictionary<Type, ScriptableObject> configDictionary;

        public void Load()
        {
            configDictionary = new Dictionary<Type, ScriptableObject>();
            configDictionary.Clear();

            for (int i = 0; i < configs.Length; i++)
            {
                Type configType = configs[i].GetType();
                if (configDictionary.ContainsKey(configType))
                {
                    Debug.LogWarning($"Duplicate config type {configType} found in configs array!");
                    continue;
                }
                configDictionary[configType] = configs[i];
            }
        }

        public TConfig GetConfig<TConfig>() where TConfig : ScriptableObject
        {
            if (configDictionary == null)
            {
                Load();
            }

            Type configType = typeof(TConfig);
            if (configDictionary.TryGetValue(configType, out ScriptableObject config))
            {
                return config as TConfig;
            }

            throw new NullReferenceException($"Type {configType} not found in loaded configs!");
        }
    }
}

