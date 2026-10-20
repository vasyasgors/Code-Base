using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure.Editor
{
    [CreateAssetMenu(fileName = "Package Manager Presets", menuName = "Configs/Package Manager Presets")]
    public class PackageManagerPresets : ScriptableObject
    {
        public List<PackageCollection> collections = new List<PackageCollection>();
    }
}
