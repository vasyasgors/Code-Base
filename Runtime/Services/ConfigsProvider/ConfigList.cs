using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public abstract class ConfigList<TConfig> : ScriptableObject
    {
       public TConfig[] Configs;

    }



}





